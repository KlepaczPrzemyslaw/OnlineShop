using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Context;
using OnlineShop.Models;
using OnlineShop.Services.IServices;
using OnlineShop.ViewModels;

namespace OnlineShop.Services.Services
{
	public class OrderService : IOrderService
	{
		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Constructor 
		/////////////////////////////////////////////////////////////////////////////////////////////

		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		
		public OrderService(ApplicationDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Methods
		/////////////////////////////////////////////////////////////////////////////////////////////

		public async Task<IEnumerable<OrderViewModelForAdmin>> GetOrders()
		{
			List<OrderViewModelForAdmin> listForAdmin = new List<OrderViewModelForAdmin>();

			List<OrderModel> orderedProductsList = await _context.OrderModel.Where(x => x.Status == OrderStatus.New).ToListAsync();
			foreach (OrderModel anOrder in orderedProductsList)
			{
				ProductModel product = await _context.ProductModel.FirstOrDefaultAsync(x => x.ID == anOrder.ProductModelID);
				UserDetailModel userDetail = await _context.UserDetailModel.FirstOrDefaultAsync(x => x.ID == anOrder.UserDetailModelID);

				OrderViewModelForAdmin modelForAdmin = new OrderViewModelForAdmin();
				_mapper.Map(product, modelForAdmin);
				_mapper.Map(userDetail, modelForAdmin);
				_mapper.Map(anOrder, modelForAdmin);

				listForAdmin.Add(modelForAdmin);
			}

			return listForAdmin;
		}

		public async Task<InternalStatus> SendOrder(Guid id)
		{
			OrderModel order = await _context.OrderModel.FirstOrDefaultAsync(x => x.ID == id);

			if (order == null)
				return InternalStatus.OrderNotFound;

			try
			{
				order.Status = OrderStatus.Sent;
				_context.OrderModel.Update(order);
				_context.SaveChanges();
				return InternalStatus.EverythingOk;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return InternalStatus.DatabaseError;
			}
		}
	}
}
