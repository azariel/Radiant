using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radiant.Common.Utils;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Installers;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    //// http:localhost:5000
    //options.Listen(IPAddress.Loopback, 5000);
    //// https:localhost:5001
    //options.Listen(IPAddress.Loopback, 5001);
    //// http:*:80
    //options.Listen(IPAddress.Any, 80);
    // https:*:443
    options.Listen(IPAddress.Any, 58225, listenOptions => { listenOptions.UseHttps(HttpsCertificateHelper.GetSelfSignedCertificate(nameof(Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi))); });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add all other dependencies
Installer.ConfigureDependencies(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware(typeof(ProductsHistoryWebApiExceptionMiddleware));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();