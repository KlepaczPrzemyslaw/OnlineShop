using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
	public class UserDetailViewModel // DTO
	{
		[Key]
		public string ID { get; set; }

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
