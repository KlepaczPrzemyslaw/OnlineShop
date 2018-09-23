using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Models;

namespace OnlineShop.Context
{
	public class DbInitializer : IDbInitializer
	{
		private readonly IServiceProvider _serviceProvider;

		public DbInitializer(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		//// Initialize ////

		public async void Initialize()
		{
			using (IServiceScope serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				ApplicationDbContext context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

				// DATABASE
				context.Database.EnsureCreated();

				RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

				// ROLES
				if (!(await roleManager.RoleExistsAsync("Admin")))
				{
					await roleManager.CreateAsync(new IdentityRole("Admin"));
				}
				if (!(await roleManager.RoleExistsAsync("User")))
				{
					await roleManager.CreateAsync(new IdentityRole("User"));
				}

				UserManager<IdentityUser> userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityUser>>();

				// ADMIN
				const string adminLogin = "admin@admin.com";
				const string adminPassword = "AdminPass1.";
				var rootSuccess = await userManager.CreateAsync(
					new IdentityUser()
					{
						UserName = adminLogin,
						Email = adminLogin,
						EmailConfirmed = true
					}, adminPassword);

				if (rootSuccess.Succeeded)
				{
					await userManager.AddToRoleAsync(await userManager.FindByNameAsync(adminLogin), "Admin");
				}

				// USER
				const string userLogin = "user@user.com";
				const string userPassword = "UserPass1.";
				var adminSuccess = await userManager.CreateAsync(
					new IdentityUser()
					{
						UserName = userLogin,
						Email = userLogin,
						EmailConfirmed = true
					}, userPassword);

				if (adminSuccess.Succeeded)
				{
					await userManager.AddToRoleAsync(await userManager.FindByNameAsync(userLogin), "User");
				}

				// PRODUCTS
				if (context.ProductModel.FirstOrDefault(x => x.Name == "Example") == null)
				{
					context.ProductModel.Add(new ProductModel()
					{
						Category = "First Example",
						Description = "Good product!",
						Name = "Example",
						PhotoID = Guid.Parse("12b8531d-50de-4577-b581-7fe7762c1bc3"),
						Price = 10.99M,
						Quantity = 10,
						Updated = DateTime.UtcNow
					});
					context.SaveChanges();
				}

				if (context.ProductModel.FirstOrDefault(x => x.Name == "Example 2") == null)
				{
					context.ProductModel.Add(new ProductModel()
					{
						Category = "Second Example",
						Description = "Good product!",
						Name = "Example 2",
						PhotoID = Guid.Parse("12b8531d-50de-4577-b581-7fe7762c1bc3"),
						Price = 39.99M,
						Quantity = 50,
						Updated = DateTime.UtcNow
					});
					context.SaveChanges();
				}

				if (context.ProductModel.FirstOrDefault(x => x.Name == "Example 3") == null)
				{
					context.ProductModel.Add(new ProductModel()
					{
						Category = "Second Example",
						Description = "Good product!",
						Name = "Example 3",
						PhotoID = Guid.Parse("12b8531d-50de-4577-b581-7fe7762c1bc3"),
						Price = 50.50M,
						Quantity = 3,
						Updated = DateTime.UtcNow
					});
					context.SaveChanges();
				}

				if (context.ProductModel.FirstOrDefault(x => x.Name == "Example 4") == null)
				{
					context.ProductModel.Add(new ProductModel()
					{
						Category = "Third Example",
						Description = "Good product!",
						Name = "Example 4",
						PhotoID = Guid.Parse("12b8531d-50de-4577-b581-7fe7762c1bc3"),
						Price = 150.00M,
						Quantity = 30,
						Updated = DateTime.UtcNow
					});
					context.SaveChanges();
				}
			}
		}
	}
}
