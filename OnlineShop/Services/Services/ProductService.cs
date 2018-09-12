using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Context;
using OnlineShop.Models;
using OnlineShop.Services.IServices;
using OnlineShop.ViewModels;

namespace OnlineShop.Services.Services
{
	public class ProductService : IProductService
	{
		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Constructor 
		/////////////////////////////////////////////////////////////////////////////////////////////

		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public ProductService(ApplicationDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Methods
		/////////////////////////////////////////////////////////////////////////////////////////////

		public async Task<ProductDetailsViewModel> GetAsync(Guid id)
		{
			return _mapper.Map<ProductDetailsViewModel>(await _context.ProductModel.FirstOrDefaultAsync(x => x.ID == id));
		}

		public async Task<IEnumerable<ProductViewModel>> BrowseAsync(string name = "")
		{
			return _mapper.Map<IEnumerable<ProductViewModel>>(await _context.ProductModel.Where(x => x.Category.ToLower().Contains(name.ToLower())).ToListAsync());
		}

		public async Task<IEnumerable<string>> GetCategoriesAsync()
		{
			return await _context.ProductModel.GroupBy(x => x.Category).Select(gr => gr.Key).ToListAsync();
		}

		public async Task<InternalStatus> CreateAsync(ProductDetailsViewModel productFromView, IFormFile fileFromSite)
		{
			try
			{
				ProductModel productModel = _mapper.Map<ProductModel>(productFromView);
				productModel.PhotoID = Guid.NewGuid(); // Mapper always gave the same Guid !?
				await _context.ProductModel.AddAsync(productModel);
				
				string fileToDelete = $@"{Directory.GetCurrentDirectory()}\wwwroot\Product\Images\{productModel.PhotoID}.png";
				if (File.Exists(fileToDelete))
				{
					File.Delete(fileToDelete);
				}

				if (fileFromSite.FileName.ToLower().EndsWith(".png") == false)
				{
					return InternalStatus.BadFileExtension;
				}

				string fileName = $"{productModel.PhotoID}.png";
				var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Product\Images\", fileName);

				using (var stream = new FileStream(path, FileMode.Create))
				{
					await fileFromSite.CopyToAsync(stream);
				}

				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public InternalStatus Edit(ProductDetailsViewModel productFromView)
		{
			try
			{
				ProductModel myModel = _mapper.Map<ProductModel>(productFromView);

				_context.Entry(myModel).State = EntityState.Modified;
				_context.Entry(myModel).Property(x => x.PhotoID).IsModified = false;
				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public InternalStatus Delete(Guid id)
		{
			try
			{
				ProductModel model = _context.ProductModel.FirstOrDefault(x => x.ID == id);

				if (model == null)
					return InternalStatus.ProductNotFound;

				_context.ProductModel.Remove(model);
				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public async Task<InternalStatus> BuyAsync(IdentityUser user)
		{
			if (await _context.UserDetailModel.FirstOrDefaultAsync(x => x.ID == user.Id) == null)
				return InternalStatus.UserDetailNotFound;

			List<ShoppingCartModel> productsList = await
				_context.ShoppingCartModel.Where(x => x.UserDetailModelID == user.Id).ToListAsync();

			try
			{
				foreach (ShoppingCartModel model in productsList)
				{
					ProductModel productModel = await _context.ProductModel.FirstOrDefaultAsync(x => x.ID == model.ProductModelID);

					if (productModel == null)
						return InternalStatus.ProductNotFound;

					if (productModel.Quantity < model.BoughtAmount)
						return InternalStatus.QuantityToSmall;

					// Decreasing the amount
					productModel.Quantity -= model.BoughtAmount;
					productModel.Updated = DateTime.UtcNow;
					_context.ProductModel.Update(productModel);

					// Order
					await _context.OrderModel.AddAsync(_mapper.Map<OrderModel>(model));

					// Cart
					_context.ShoppingCartModel.Remove(model);
				}

				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public async Task<InternalStatus> CancelAsync(Guid id)
		{
			OrderModel order = await _context.OrderModel.FirstOrDefaultAsync(x => x.ID == id);

			if (order == null)
				return InternalStatus.OrderNotFound;

			ProductModel productModel = await _context.ProductModel.FirstOrDefaultAsync(x => x.ID == order.ProductModelID);

			if (productModel == null)
				return InternalStatus.ProductNotFound;

			try
			{
				// Increasing amount
				productModel.Quantity += order.BoughtAmount;
				productModel.Updated = DateTime.UtcNow;
				_context.ProductModel.Update(productModel);

				// Order
				_context.OrderModel.Remove(order);

				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public async Task<Guid> GetImageGuidForProductID(Guid productID)
		{
			ProductModel model = await _context.ProductModel.FirstOrDefaultAsync(x => x.ID == productID);
			return model?.PhotoID ?? Guid.Empty;
		}
	}
}