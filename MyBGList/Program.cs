using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
//app.MapGet("/error", () => Results.Problem())
//  .RequireCors("AnyOrigin");

//app.MapGet("/error/test", [EnableCors("AnyOrigin")]
//[ResponseCache(NoStore = true)] () => { throw new exception("test"); });

// \r is carriage return. Cors is enabled using AnyOrigin but HTTP response is not cached by clients or proxy.
app.MapGet("/cod/test", [EnableCors("AnyOrigin")][ResponseCache(NoStore = true)] () =>
Results.Text("<script>" + "window.alert('Your client supports JavaScript!" + "\\r\\n\\r\\n" +
$"Server time(UTC): {DateTime.UtcNow.ToString("o")}" + "\\r\\n" +
"Client time(UTC): ' + new Date().toISOString());" +
"</script>" +
"<noscript>Your client does not support JavaScript</noscript>", "text/html"));

app.MapControllers();

app.Run();
