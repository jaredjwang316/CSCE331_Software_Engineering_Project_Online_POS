using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
builder.WebHost.UseUrls("https://localhost:5001");
Console.WriteLine("DEBUG MODE");
#else
builder.WebHost.UseUrls("https://07r-webapp.azurewebsites.net/");
Console.WriteLine("RELEASE MODE");
#endif

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddCookie()
.AddGoogle("Google", options =>
{
    options.ClientId = "342025315231-mvkk9dtjsld6j5ghvekcnralf8smk71p.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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
