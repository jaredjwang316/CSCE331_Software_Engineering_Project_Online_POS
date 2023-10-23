using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var username = Environment.GetEnvironmentVariable("PSQL_USERNAME");
var password = Environment.GetEnvironmentVariable("PSQL_PASSWORD");
var connString = $"Host=csce-315-db.engr.tamu.edu;Port=5432;Username={username};Password={password};Database=csce315331_07r_db;";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
var dataSource = dataSourceBuilder.Build();

var conn = await dataSource.OpenConnectionAsync();

// Retrieve all rows from the products table
await using (var cmd = new NpgsqlCommand("SELECT * FROM products", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
    while (await reader.ReadAsync()) {
        // int product_id = reader.GetInt32(0);
        string product_name = reader.GetString(1);
        // double product_price = reader.GetDouble(2);
        // string product_series = reader.GetString(3);

        Console.WriteLine(product_name);
    }


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
