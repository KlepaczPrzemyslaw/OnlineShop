using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
	public class ShoppingCartViewModel
	{
		[Key]
		public Guid ID { get; set; }

		[Required, DisplayName("Product Name")]
		public string ProductName { get; set; }

		[Required]
		public decimal Price { get; set; }
		
		[Required, DisplayName("Bought Amount")]
		public int BoughtAmount { get; set; }
	}
}
