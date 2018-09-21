using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Services.Interfaces;

namespace OnlineShop.Controllers
{
	//// For Admin ////
	[Authorize(Roles = "Admin")]
	public class OrderController : Controller
	{
		//// Constructor ////

		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			this._orderService = orderService;
		}

		//// Private Methods ////

		private IActionResult Result(InternalStatus result, string okStatusMessage)
		{
			switch (result)
			{
				case InternalStatus.EverythingOk:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-success";
					TempData["OrderMessage"] = okStatusMessage;
					return Redirect("/Order/Index");
				case InternalStatus.DatabaseError:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["OrderMessage"] = "Transaction Rejected! Database Error!";
					return Redirect("/Order/Index");
				case InternalStatus.ProductNotFound:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["OrderMessage"] = "Transaction Rejected! Product Not Found!";
					return Redirect("/Order/Index");
				case InternalStatus.OrderNotFound:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["OrderMessage"] = "Transaction Rejected! Order Not Found!";
					return Redirect("/Order/Index");
				case InternalStatus.UserDetailNotFound:
					return Redirect("/UserDetail/Create");
				case InternalStatus.QuantityToSmall:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["OrderMessage"] = "Transaction Rejected! Quantity To Small!";
					return Redirect("/Order/Index");
				case InternalStatus.BadFileExtension:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["OrderMessage"] = "Transaction Rejected! I Need A .png File!";
					return Redirect("/Order/Index");
				case InternalStatus.DefaultOption:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["OrderMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/Order/Index");
				default:
					TempData["OrderMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["OrderMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/Order/Index");
			}
		}

		//// Pages ////

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View(await _orderService.GetOrders());
		}

		[HttpPost]
		public async Task<IActionResult> Send(Guid id)
		{
			InternalStatus result = await _orderService.SendOrder(id);
			return Result(result, "Transaction Approved!");
		}
	}
}
