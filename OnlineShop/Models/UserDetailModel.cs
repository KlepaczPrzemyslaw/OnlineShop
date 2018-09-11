using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Models.IModels;

namespace OnlineShop.Models
{
	public class UserDetailModel : IMicrosoftUserEntity
	{
		[Key]
		public string ID { get; set; }

		[ForeignKey("ID")]
		public IdentityUser IdentityUser { get; set; }

		[Required, MaxLength(64)]
		public string FirstName { get; set; }

		[Required, MaxLength(64)]
		public string LastName { get; set; }
		
		[Required, MaxLength(64)]
		public string City { get; set; }

		[Required, MaxLength(12)]
		public string PostCode { get; set; }

		[Required, MaxLength(64)]
		public string Address { get; set; }
	}
}
