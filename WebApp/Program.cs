using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using WebApp;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

string returnUrl = "https://07r-webapp.azurewebsites.net/";
builder.WebHost.UseUrls("https://07r-webapp.azurewebsites.net/");
builder.Services.AddSingleton(returnUrl);

// Add services to the container.
builder.Services.AddControllersWithViews();

Console.WriteLine("Client ID: " + configuration["GOOGLE_PROVIDER_AUTHENTICATION_CLIENT_ID"]);
Console.WriteLine("Client Secret: " + configuration["GOOGLE_PROVIDER_AUTHENTICATION_SECRET"]);

// Set up Google Authentication scheme.
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = configuration["GOOGLE_PROVIDER_AUTHENTICATION_CLIENT_ID"]
        ?? throw new ArgumentException("Google client id environment variable cannot be null or empty.");
    options.ClientSecret = configuration["GOOGLE_PROVIDER_AUTHENTICATION_SECRET"]
        ?? throw new ArgumentException("Google client secret environment variable cannot be null or empty.");
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
