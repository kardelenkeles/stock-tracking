using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using Project.Data;
using Project.Core.Models;
using Project.Services;

class SeedData
{
    static void Main()
    {
        Database.Initialize("app.db");
        var conn = Database.ConnectionString;

        var userRepo = new UserRepository(conn);
        var catRepo = new CategoryRepository(conn);
        var supRepo = new SupplierRepository(conn);
        var prodRepo = new ProductRepository(conn);
        var movRepo = new StockMovementRepository(conn);
        var logRepo = new ActivityLogRepository(conn);

        var logService = new ActivityLogService(logRepo);

        var userService = new UserService(userRepo, logService, movRepo);
        var catService = new CategoryService(catRepo, prodRepo, logService);
        var supService = new SupplierService(supRepo, prodRepo, logService);
        var prodService = new ProductService(prodRepo, movRepo, logService);
        var stockService = new StockService(movRepo, prodRepo, logService);

        try
        {
            Console.WriteLine("Seeding data...");

            var admin = userRepo.GetAll().FirstOrDefault(u => u.Username == "admin");
            if (admin == null)
            {
                admin = new User { Username = "admin", PasswordHash = AuthService.HashPassword("admin123"), Role = "Admin" };
                userService.Create(admin);
            }

            var user = userRepo.GetAll().FirstOrDefault(u => u.Username == "user");
            if (user == null)
            {
                user = new User { Username = "user", PasswordHash = AuthService.HashPassword("user123"), Role = "User" };
                userService.Create(user);
            }

            for (int i = 1; i <= 20; i++)
            {
                var category = new Category { Name = $"Category {i}", Description = $"Description for Category {i}" };
                catService.Create(category, admin);

                var supplier = new Supplier { Name = $"Supplier {i}", ContactName = $"Contact {i}", Phone = $"123-456-78{i:00}", Email = $"supplier{i}@example.com", Address = $"Address {i}" };
                supService.Create(supplier, admin);

                for (int j = 1; j <= 10; j++)
                {
                    var product = new Product
                    {
                        Name = $"Product {i}-{j}",
                        CategoryId = i,
                        SupplierId = i,
                        Quantity = j * 5,
                        MinQuantity = j,
                        PurchasePrice = j * 10,
                        SellPrice = j * 15
                    };
                    prodService.Create(product, admin);
                }
            }

            Console.WriteLine("Data seeded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
