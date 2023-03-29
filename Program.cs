using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SmartHome.Data;
using SmartHome.Arduino.Models;
using SmartHome.Models;
using System.Reflection;
using SmartHome.Arduino.Models.Components;
using Microsoft.JSInterop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<CustomTimeModule>();
builder.Services.AddScoped<SnackBar>();
builder.Services.AddSingleton<GenericComponent>();
builder.Services.AddSingleton(new Server());
builder.Services.AddSingleton(new WindowDimension());
builder.Services.AddSingleton<ClipboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
