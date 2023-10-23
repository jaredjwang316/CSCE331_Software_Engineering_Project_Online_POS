using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add PostgreSQL database context to the container.
// var url_base = Environment.GetEnvironmentVariable("JDBC_URL_BASE") ?? "";
// var connection_string = "csce315331_07r_db";
// var username = "csce331_970_griffinbeaudreau";
// var password = "password";
var connString = "Host=csce-315-db.engr.tamu.edu;Port=5432;Username=csce331_970_griffinbeaudreau;Password=password;Database=csce315331_07r_db;";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
var dataSource = dataSourceBuilder.Build();

var conn = await dataSource.OpenConnectionAsync();

// Retrieve all rows from the products table
await using (var cmd = new NpgsqlCommand("SELECT * FROM products", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
    while (await reader.ReadAsync())
        Console.WriteLine(reader.GetString(0));


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
