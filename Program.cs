using ECommerceMvc.Data;
using ECommerceMvc.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product
            {
                Name = "Wireless Headphones",
                Description = "Comfortable headphones with long battery life.",
                Price = 89.99m,
                Quantity = 25,
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=1200"
            },
            new Product
            {
                Name = "Smart Watch",
                Description = "Track your daily activity and receive notifications.",
                Price = 129.00m,
                Quantity = 18,
                ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=1200"
            },
            new Product
            {
                Name = "Portable Speaker",
                Description = "Small but powerful Bluetooth speaker.",
                Price = 59.50m,
                Quantity = 34,
                ImageUrl = "https://images.unsplash.com/photo-1589003077984-894e133dabab?w=1200"
            }
        );

        db.SaveChanges();
    }
}


app.Run();
