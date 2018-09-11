using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineShop.Models.IModels;

namespace OnlineShop.Models
{
	public class OrderModel : IEntity
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
		public OrderStatus Status { get; set; }

		[Required]
		public int BoughtAmount { get; set; }
	}
}
