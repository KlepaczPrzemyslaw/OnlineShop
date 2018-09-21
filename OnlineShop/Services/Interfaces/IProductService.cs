using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Services.Interfaces
{
	public interface IProductService
	{
		Task<ProductDetailsViewModel> GetAsync(Guid id);

		Task<IEnumerable<string>> GetCategoriesAsync();
		Task<IEnumerable<ProductViewModel>> BrowseAsync(string name = "");

		Task<InternalStatus> CreateAsync(ProductDetailsViewModel productFromView, IFormFile fileFromSite);
		InternalStatus Edit(ProductDetailsViewModel productFromView);
		InternalStatus Delete(Guid id);
		
		Task<InternalStatus> BuyAsync(IdentityUser user);
		Task<InternalStatus> CancelAsync(Guid id);

		Task<Guid> GetImageGuidForProductID(Guid productID);
	}
}
