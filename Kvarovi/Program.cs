using System.Net.Http.Headers;
using System.Net.Mime;
using HtmlAgilityPack;
using Cyrillic.Convert;
using Kvarovi.AnnouncementGetters;
using Kvarovi.Contexts;
using Kvarovi.Middleware.ApiKeyAuthentication;
using Kvarovi.Repository;
using Kvarovi.Services;
using Kvarovi.Services.AnnouoncementUpdate;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
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
builder.Services.AddScoped<IUserRepository,UserRepository>() ;
builder.Services.AddScoped<UserNotifierFactory>() ;
builder.Services.AddSingleton<AnnouncementGetterFactory>();
builder.Services.AddHostedService<ExpoReceiptChecker>();
builder.Services.AddHostedService<AnnouncementUpdateService>();
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

 //Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    setup =>
    {
        setup.AddSecurityDefinition(ApiKeyAuthenticationOptions.DefaultScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = ApiKeyAuthenticationOptions.HeaderName,
            Type = SecuritySchemeType.ApiKey
        });

        setup.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ApiKeyAuthenticationOptions.DefaultScheme
                    }
                }, 
                Array.Empty<string>()
            }
        }); 
    } 
    );

builder.Services.AddSingleton<ApiKeyUserMemoryCache>();
builder.Services
    .AddScoped<ICacheService, CacheService>()
    .AddScoped<ApiKeyAuthenticationHandler>();

builder.Services.AddAuthentication()
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, null);
var app = builder.Build();



app.UseCors();
app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();



app.Run();

