using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infraestructure.Data;

public static class StoreSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<Client> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            string[] roleNames = { "Administrator", "Client" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // create admin if they don't exist
            var testEmail = "cliente@test.com";
            if (!await userManager.Users.AnyAsync(u => u.Email == testEmail))
            {
                var client = new Client
                {
                    UserName = testEmail,
                    Email = testEmail,
                    FirstName = "Juan",
                    LastName = "Perez",
                    DocumentNumber = "1234567890",
                    Address = "Calle Falsa 123",
                    PhoneNumber = "3001234567",
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(client, "ClientPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(client, "Client");
                }
            }

            // create products if they don't exist
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Name = "Cemento Gris Argos", Description = "Saco de 50kg multiuso", UnitPrice = 28000, Stock = 100 },
                    new Product { Name = "Ladrillo Rojo", Description = "Ladrillo estructural 10x20x40", UnitPrice = 1500, Stock = 5000 },
                    new Product { Name = "Arena de Río", Description = "Metro cúbico de arena lavada", UnitPrice = 65000, Stock = 30 },
                    new Product { Name = "Varilla Corrugada 1/2", Description = "Varilla de acero 6m", UnitPrice = 22000, Stock = 200 },
                    new Product { Name = "Pintura Blanca", Description = "Cuñete tipo 1 lavable", UnitPrice = 120000, Stock = 15 }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }