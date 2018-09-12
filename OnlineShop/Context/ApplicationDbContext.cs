using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Context
{
	public class ApplicationDbContext : IdentityDbContext
	{
		// OnlineShop-B92F0AA6-9855-4F13-BA2D-13DE3EC44201
		
		public DbSet<ProductModel> ProductModel { get; set; }
		public DbSet<OrderModel> OrderModel { get; set; }
		public DbSet<UserDetailModel> UserDetailModel { get; set; }
		public DbSet<ShoppingCartModel> ShoppingCartModel { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}
