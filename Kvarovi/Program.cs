using System.Net.Http.Headers;
using System.Net.Mime;
using HtmlAgilityPack;
using Cyrillic.Convert;
using Kvarovi.AnnouncementGetters;
using Kvarovi.Contexts;
using Kvarovi.Repository;
using Kvarovi.Services.AnnouoncementUpdate;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;




Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Async(a => a.Console())
    .WriteTo.Async(a => a.File("logs/log.txt", rollingInterval: RollingInterval.Day))
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command",LogEventLevel.Warning)
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5050");
builder.Configuration.AddEnvironmentVariables();
builder.Host.UseSerilog();


builder.Services.AddDbContext<MySqlContext>(optionsBuilder =>
{
    var connString = builder.Configuration.GetConnectionString("Default");
    optionsBuilder.UseMySql(connString,
        ServerVersion.AutoDetect(connString));
    // optionsBuilder.EnableSensitiveDataLogging();
});


builder.Services.AddScoped<IAnnouncementRepository,AnnouncementRepository>() ;
builder.Services.AddHostedService<ExpoReceiptChecker>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => 
        {
            
            //policy.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback).AllowAnyHeader().AllowAnyMethod();
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});


builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseForwardedHeaders();
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();


app.Run();

