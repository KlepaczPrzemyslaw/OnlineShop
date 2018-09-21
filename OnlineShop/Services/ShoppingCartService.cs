using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Context;
using OnlineShop.Models;
using OnlineShop.Services.Interfaces;
using OnlineShop.ViewModels;

namespace OnlineShop.Services
{
	public class ShoppingCartService : IShoppingCartService
	{
		//// Constructor ////

		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public ShoppingCartService(ApplicationDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		//// Other Methods For Views Too ////

		public int GetCount(IdentityUser user)
		{
			return _context.ShoppingCartModel.Count(x => x.UserDetailModelID == user.Id);
		}

		public decimal GetPriceForSummary(IdentityUser user)
		{
			return _context.ShoppingCartModel.Where(x => x.UserDetailModelID == user.Id).Sum(x => (x.Price * x.BoughtAmount));
		}

		//// Methods ////

		public async Task<InternalStatus> AddProduct(Guid productID, IdentityUser user, int boughtAmount)
		{
			if (await _context.UserDetailModel.FirstOrDefaultAsync(x => x.ID == user.Id) == null)
				return InternalStatus.UserDetailNotFound;

			if (boughtAmount <= 0)
				return InternalStatus.QuantityToSmall;

			ProductModel productModel = await _context.ProductModel.FirstOrDefaultAsync(x => x.ID == productID);
			if (productModel == null)
				return InternalStatus.ProductNotFound;

			if (productModel.Quantity < boughtAmount)
				return InternalStatus.QuantityToSmall;

			try
			{
				ShoppingCartModel cartModel = new ShoppingCartModel
				{
					ProductModelID = productID,
					UserDetailModelID = user.Id,
					BoughtAmount = boughtAmount,
					Price = productModel.Price
				};

				await _context.ShoppingCartModel.AddAsync(cartModel);
				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public async Task<InternalStatus> RemoveProduct(Guid shoppingCartInternalID, IdentityUser user)
		{
			ShoppingCartModel model = await _context.ShoppingCartModel.FirstOrDefaultAsync(x => x.ID == shoppingCartInternalID);
			if (model == null)
				return InternalStatus.OrderNotFound;

			try
			{
				_context.ShoppingCartModel.Remove(model);
				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public async Task<InternalStatus> ClearCart(IdentityUser user)
		{
			try
			{
				List<ShoppingCartModel> list = await _context.ShoppingCartModel.Where(x => x.UserDetailModelID == user.Id).ToListAsync();

				foreach (ShoppingCartModel productFromCart in list)
				{
					_context.ShoppingCartModel.Remove(productFromCart);
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

		public async Task<IEnumerable<ShoppingCartViewModel>> GetCart(IdentityUser user)
		{
			List<ShoppingCartViewModel> cartToReturn = new List<ShoppingCartViewModel>();

			List<ShoppingCartModel> cartFromDb =
				await _context.ShoppingCartModel.Where(x => x.UserDetailModelID == user.Id).ToListAsync();
			foreach (ShoppingCartModel model in cartFromDb)
			{
				ProductModel product = _context.ProductModel.FirstOrDefault(x => x.ID == model.ProductModelID);
				ShoppingCartViewModel actualModel = new ShoppingCartViewModel();

				_mapper.Map(product, actualModel);
				_mapper.Map(model, actualModel);

				cartToReturn.Add(actualModel);
			}

			return cartToReturn;
		}
	}
}
