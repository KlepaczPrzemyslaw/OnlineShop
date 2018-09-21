using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Services.Interfaces
{
	public interface IShoppingCartService
	{
		int GetCount(IdentityUser user);
		decimal GetPriceForSummary(IdentityUser user);

		Task<InternalStatus> AddProduct(Guid productID, IdentityUser user, int boughtAmount);
		Task<InternalStatus> RemoveProduct(Guid shoppingCartInternalID, IdentityUser user);

		Task<InternalStatus> ClearCart(IdentityUser user);
		Task<IEnumerable<ShoppingCartViewModel>> GetCart(IdentityUser user);
	}
}
