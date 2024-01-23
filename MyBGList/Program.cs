using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyBGList.Data;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
}

app.UseHttpsRedirection();

app.UseCors("AnyOrigin");

app.UseAuthorization();

// Applying AnyOrigin Policy to minimal API Routes. Could be applied to Map Controllers but then it would be applied globally to controllers.
// Try not to use endpoint routing but the [EnableCors] atrribute instead.
app.MapGet("/error",
[EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)]
() => Results.Problem());

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