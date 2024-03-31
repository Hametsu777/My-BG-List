using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyBGList.Constants;
using MyBGList.Data;
using MyBGList.Swagger;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);
// Removes all registered logging providers, adds the console logging provider, adds the debug logging provider.
// The AddApplicationInsights line activates the Application Insights logging provider for MyBGList web API.
builder.Logging
    .ClearProviders()
    .AddSimpleConsole()
    .AddDebug()
    .AddApplicationInsights(telemetry => telemetry.ConnectionString = builder
    .Configuration["Azure:ApplicationInsights:ConnectionString"], loggerOptions => { });


// Figure out how to configure serilog for postgresql. Have to use Serilog.sinks.postgres.alternative. The regular version is no longer
// maintained.
//builder.Host.UseSerilog((ctx, lc) =>
//{
//    lc.ReadFrom.Configuration(ctx.Configuration);
//    lc.WriteTo.PostgreSQL(connectionString: ctx.Configuration.GetConnectionString("DefaultConnection"), sinkOptions: new PostgreSQLSink
//    {

//    });
//});




// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Customizing model binding errors.
builder.Services.AddControllers(options =>
{
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"The value '{x}' is invalid.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => $"The field {x} must be a number.");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => $"The value '{x}' is not valid for {y}.");
    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => $"A value is required.");
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ParameterFilter<SortColumnFilter>();
    options.ParameterFilter<SortOrderFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"]);
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin",
        cfg =>
        {
            cfg.AllowAnyOrigin();
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
        });
});

string tableName = "logs";

IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
{
    {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
    {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
    {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
    { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
    { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
    { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
    { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "1") }
};

var logger = new LoggerConfiguration()
    .WriteTo.PostgreSQL(connectionString, tableName, columnOptions, needAutoCreateTable: true, schemaName: "LoggingSchema")
    .CreateLogger();

// Disables the automatic ModelState Validation feature. This setting suppresses the filter that automatically
// returns a BadRequestObjectResult when the ModelState is invalid. (This let's us check the ModelValidation manually).
// Don't use this method. Action methods were still executed even if parameters were invalid.
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//if (app.Configuration.GetValue<bool>("UseSwagger"))
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
{ app.UseDeveloperExceptionPage(); }
else
{
    app.UseExceptionHandler("/error");
    //app.UseExceptionHandler(action =>
    //{
    //    action.Run(async context =>
    //    {
    //        // Retrieves the Exception Handeler
    //        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();

    //        // Sets the Exception message.
    //        var details = new ProblemDetails();
    //        details.Detail = exceptionHandler?.Error.Message;
    //        details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
    //        details.Type = "https://tools.ieft.org/html/rfc7231#section-6.6.1";
    //        details.Status = StatusCodes.Status500InternalServerError;
    //        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(details));
    //    });
    //});
}

app.UseHttpsRedirection();

app.UseCors("AnyOrigin");

app.UseAuthorization();

// Applying AnyOrigin Policy to minimal API Routes. Could be applied to Map Controllers but then it would be applied globally to controllers.
// Try not to use endpoint routing but the [EnableCors] atrribute instead. Need to study details/ProblemDetails object.
app.MapGet("/error",
[EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)]
(HttpContext context) =>
{
    // Retrieves the Exception Handeler
    var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();

    // Sets the Exception message.
    var details = new ProblemDetails();
    details.Detail = exceptionHandler?.Error.Message;
    details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
    details.Type = "https://tools.ieft.org/html/rfc7231#section-6.6.1";
    details.Status = StatusCodes.Status500InternalServerError;

    app.Logger.LogError(CustomLogEvents.Error_Get, exceptionHandler?.Error, "An unhandled exception occured.");
    return Results.Problem(details);
});

app.MapGet("/error/test",
[EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)] () =>
{ throw new Exception("test"); });

// \r is carriage return. Cors is enabled using AnyOrigin but HTTP response is not cached by clients or proxy.
app.MapGet("/cod/test",
[EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)] () =>
Results.Text("<script>" + "window.alert('Your client supports JavaScript!" + "\\r\\n\\r\\n" +
$"Server time(UTC): {DateTime.UtcNow.ToString("o")}" + "\\r\\n" +
"Client time(UTC): ' + new Date().toISOString());" +
"</script>" +
"<noscript>Your client does not support JavaScript</noscript>", "text/html"));

app.MapControllers();

app.Run();


//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc(
//        "v1",
//        new OpenApiInfo { Title = "MyBGList_ApiVersion", Version = "v1.0" });
//    options.SwaggerDoc(
//        "v2",
//        new OpenApiInfo { Title = "MyBGList_ApiVersion", Version = "v2.0" });
//});

// Need to download the packages Asp.Versiong.Mvc, Asp.Versiong.ApiExplorer, and Microsoft.AspNetCore.OpenApi, to use versioning.
// Can replace UrlSegmentApi... for another versioning reader such as querystring or http headers.
//builder.Services.AddApiVersioning(options =>
//{
//    options.ApiVersionReader = new UrlSegmentApiVersionReader();
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.DefaultApiVersion = new ApiVersion(1, 0);
//})
//    .AddApiExplorer(options =>
//    {
//        options.GroupNameFormat = "'v'VVV";
//        options.SubstituteApiVersionInUrl = true;
//    });

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//// Need To make sure Swagger will load the swagger.json files.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint(
//            $"/swagger/v1/swagger.json",
//            $"MyBGList_ApiVersion v1");
//        options.SwaggerEndpoint(
//            $"/swagger/v2/swagger.json",
//            $"MyBGList_ApiVersion v2");
//    });
//}
//app.MapGet("/v{version:ApiVersion}/error",
//[ApiVersion("1.0")]
//[ApiVersion("2.0")]
//[EnableCors("AnyOrigin")]
//[ResponseCache(NoStore = true)]
//() => Results.Problem());