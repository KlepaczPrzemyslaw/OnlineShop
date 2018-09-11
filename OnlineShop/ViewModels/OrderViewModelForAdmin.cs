using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
	public class OrderViewModelForAdmin // Prepare To Send
	{
		[Key]
		public Guid ID { get; set; }

		[Required, DisplayName("Product Name")]
		public string ProductName { get; set; }

		[Required, DisplayName("Order Status")]
		public OrderStatus Status { get; set; }

		[Required, DisplayName("Bought Amount")]
		public int BoughtAmount { get; set; }

		[Required, MaxLength(64), DisplayName("First Name")]
		public string FirstName { get; set; }

		[Required, MaxLength(64), DisplayName("Last Name")]
		public string LastName { get; set; }

		[Required, MaxLength(64)]
		public string City { get; set; }

		[Required, MaxLength(12), DisplayName("Post Code")]
		public string PostCode { get; set; }

		[Required, MaxLength(64), DisplayName("Street + House Number")]
		public string Address { get; set; }
	}
}
