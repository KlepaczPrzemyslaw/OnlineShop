using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineShop.Context
{
	public class DbInitializer : IDbInitializer
	{
		private readonly IServiceProvider _serviceProvider;

		public DbInitializer(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		///////////////////////
		/// Initialize
		///////////////////////

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
					await userManager.AddToRoleAsync(await userManager.FindByNameAsync(userLogin), "Admin");
				}
			}
		}
	}
}
