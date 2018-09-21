using System;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models.Interfaces;

namespace OnlineShop.Models
{
	public class ProductModel : IEntity
	{
		[Key]
		public Guid ID { get; set; }

		[Required, MaxLength(64)]
		public string Category { get; set; }

		[Required, MaxLength(64)]
		public string Name { get; set; }

		[Required, MaxLength(256)]
		public string Description { get; set; }

		[Required]
		public DateTime Updated { get; set; }

		[Required]
		public decimal Price { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		public Guid PhotoID { get; set; }
	}
}
