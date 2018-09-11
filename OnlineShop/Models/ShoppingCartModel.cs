using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using OnlineShop.Models.IModels;

namespace OnlineShop.Models
{
	public class ShoppingCartModel : IEntity
	{
		[Key]
		public Guid ID { get; set; }

		[Required]
		public string UserDetailModelID { get; set; }

		[ForeignKey("UserDetailModelID")]
		public UserDetailModel UserDetailModel { get; set; }

		[Required]
		public Guid ProductModelID { get; set; }

		[ForeignKey("ProductModelID")]
		public ProductModel ProductModel { get; set; }

		[Required]
		public int BoughtAmount { get; set; }

		[Required]
		public decimal Price { get; set; }
	}
}
