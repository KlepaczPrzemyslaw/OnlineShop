using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
	public class OrderViewModel // DTO
	{
		[Key]
		public Guid ID { get; set; }

		[Required, DisplayName("Product Name")]
		public string ProductName { get; set; }

		[Required, DisplayName("Order Status")]
		public OrderStatus Status { get; set; }

		[Required, DisplayName("Bought Amount")]
		public int BoughtAmount { get; set; }
	}
}
