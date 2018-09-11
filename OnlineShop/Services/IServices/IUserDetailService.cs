using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Services.IServices
{
	public interface IUserDetailService
	{
		Task<IEnumerable<OrderViewModel>> ShowMyOrders(IdentityUser user);

		Task<UserDetailViewModel> GetUserDetail(string id);

		Task<InternalStatus> CreateAsync(UserDetailViewModel userDetailFromView, IdentityUser user);
		InternalStatus Edit(UserDetailViewModel userDetailFromView, IdentityUser user);
	}
}
