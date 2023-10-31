using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using WebApp;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(Config.returnUrl);
builder.WebHost.UseUrls(Config.returnUrl);


// Set up Google Authentication scheme.
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
