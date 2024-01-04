using System.Net.Http.Headers;
using HtmlAgilityPack;
using Cyrillic.Convert;
using Kvarovi.Contexts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

/*
 www.bvk.rs/planirani-radovi & www.bvks.rs/kvarovi-na-mrezi
-----------------------------------------------------------
p.toggler -> title data
div.toggle_content ->  text data


https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_0_Iskljucenja.htm
(ide od dan 0-3)
----------------------------------------------------------------------------------
table -> tr sa 3 td (opstina,vreme,ulice)
znaci pretrazujes po tr preskocis 1.

*/

// string vodaplan = "https://www.bvk.rs/planirani-radovi";
// string vodakvar = "https://www.bvks.rs/kvarovi-na-mrezi";
// string elektro = "https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_0_Iskljucenja.htm";
// HttpClient client = new();
// client.DefaultRequestHeaders.Accept.Clear();
// client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("text/html"));
//
// async Task<string> getPage(string uri)
// {
//  var resp = await client.GetAsync(uri);
//  return await resp.Content.ReadAsStringAsync();
// }
//
// var html = new HtmlDocument();
// html.LoadHtml(await getPage(vodaplan));
//
// var titles = html.DocumentNode.SelectNodes("//p[contains(@class, 'toggler')]");
// var texts = html.DocumentNode.SelectNodes("//div[contains(@class, 'toggle_content')]");
// foreach (var htmlNode in titles)
// {
//  Console.WriteLine(htmlNode.InnerText.ToSerbianLatin());
// }
//
// foreach (var htmlNode in texts)
// {
//  Console.WriteLine(htmlNode.InnerText.ToSerbianLatin());
// }



Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Async(a => a.Console())
    .WriteTo.Async(a => a.File("logs/log.txt", rollingInterval: RollingInterval.Day))
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command",LogEventLevel.Warning)
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Host.UseSerilog();


builder.Services.AddDbContext<MySqlContext>(optionsBuilder =>
{
    var connString = builder.Configuration.GetConnectionString("Default");
    optionsBuilder.UseMySql(connString,
        ServerVersion.AutoDetect(connString));
    // optionsBuilder.EnableSensitiveDataLogging();
});


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

