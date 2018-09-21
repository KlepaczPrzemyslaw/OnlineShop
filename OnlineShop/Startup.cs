using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Infrastructure;
using OnlineShop.Services.Interfaces;
using OnlineShop.Services;

namespace OnlineShop
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			// Identity
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));
			services.AddDefaultIdentity<IdentityUser>( config => { config.SignIn.RequireConfirmedEmail = true; } )
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			// Singleton
			services.AddSingleton(AutoMapperConfig.Initialize());

			// Scoped
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IUserDetailService, UserDetailService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, UserClaimsPrincipalFactory<IdentityUser, IdentityRole>>();
			services.AddScoped<IShoppingCartService, ShoppingCartService>();
			services.AddScoped<IDbInitializer, DbInitializer>();

			// Transient
			services.AddTransient<IEmailSender, EmailSender>(i =>
				new EmailSender(
					Configuration["EmailSender:Host"],
					Configuration.GetValue<int>("EmailSender:Port"),
					Configuration.GetValue<bool>("EmailSender:EnableSSL"),
					Configuration["EmailSender:UserName"],
					Configuration["EmailSender:Password"]
				)
			);

			// MVC
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
			IServiceProvider serviceProvider, IDbInitializer dbInitializer)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Product/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Product}/{action=Index}/{id?}");
			});

			dbInitializer.Initialize();
		}
	}
}
