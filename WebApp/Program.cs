/*
    File: Program.cs
    Author: Griffin Beaudreau
    Date: November 24, 2023
    Purpose: This file is the entry point for the web app,
        and sets up the web app's configuration.
*/

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using WebApp;
using WebApp.Data;
using WebApp.Models.Cart;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(
    options => {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
);
builder.Services.AddMemoryCache();

builder.Services.AddSingleton(Config.returnUrl);
builder.WebHost.UseUrls(Config.returnUrl);

builder.Services.AddScoped<UnitOfWork>(_ => new(Config.AWS_DB_NAME));
builder.Services.AddScoped<CartService>();

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.ReturnUrlParameter = "returnUrl";
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = configuration["GOOGLE_PROVIDER_AUTHENTICATION_CLIENT_ID"]
        ?? throw new ArgumentException("GOOGLE_PROVIDER_AUTHENTICATION_CLIENT_ID environment variable cannot be null or empty.");
    options.ClientSecret = configuration["GOOGLE_PROVIDER_AUTHENTICATION_SECRET"]
        ?? throw new ArgumentException("GOOGLE_PROVIDER_AUTHENTICATION_SECRET environment variable cannot be null or empty.");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
