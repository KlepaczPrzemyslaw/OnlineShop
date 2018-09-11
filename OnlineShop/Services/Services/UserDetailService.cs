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
using OnlineShop.Services.IServices;
using OnlineShop.ViewModels;

namespace OnlineShop.Services.Services
{
	public class UserDetailService : IUserDetailService
	{
		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Constructor 
		/////////////////////////////////////////////////////////////////////////////////////////////

		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public UserDetailService(ApplicationDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Methods
		/////////////////////////////////////////////////////////////////////////////////////////////

		public async Task<IEnumerable<OrderViewModel>> ShowMyOrders(IdentityUser user)
		{
			List<OrderViewModel> orderedViewModelsList = new List<OrderViewModel>();

			List<OrderModel> orderedProductList = _context.OrderModel.Where(x => x.UserDetailModelID == user.Id).ToList();
			foreach (OrderModel anOrder in orderedProductList)
			{
				ProductModel product = await _context.ProductModel.FirstOrDefaultAsync(x => x.ID == anOrder.ProductModelID);
				string productName = product == null ? "Produkt został usunięty!" : product.Name;

				orderedViewModelsList.Add(new OrderViewModel()
				{
					BoughtAmount = anOrder.BoughtAmount,
					ID = anOrder.ID,
					ProductName = productName,
					Status = anOrder.Status
				});
			}

			return orderedViewModelsList;
		}

		public async Task<UserDetailViewModel> GetUserDetail(string id)
		{
			return _mapper.Map<UserDetailViewModel>(await _context.UserDetailModel.FirstOrDefaultAsync(x => x.ID == id));
		}

		public async Task<InternalStatus> CreateAsync(UserDetailViewModel userDetailFromView, IdentityUser user)
		{
			try
			{
				UserDetailModel userModel = _mapper.Map<UserDetailModel>(userDetailFromView);
				userModel.ID = user.Id;

				await _context.UserDetailModel.AddAsync(userModel);
				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}

		public InternalStatus Edit(UserDetailViewModel userDetailFromView, IdentityUser user)
		{
			try
			{
				UserDetailModel userModel = _mapper.Map<UserDetailModel>(userDetailFromView);
				userModel.ID = user.Id;

				_context.UserDetailModel.Update(userModel);
				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Typ: {e.GetType()} - Wiadomość: {e.Message}");
				return InternalStatus.DatabaseError;
			}
		}
	}
}