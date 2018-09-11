﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Services.IServices
{
	public interface IOrderService
	{
		Task<IEnumerable<OrderViewModelForAdmin>> GetOrders();
		Task<InternalStatus> SendOrder(Guid id);
	}
}
