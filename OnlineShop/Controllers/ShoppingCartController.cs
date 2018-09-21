using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Services.Interfaces;

namespace OnlineShop.Controllers
{
	//// For Admin and User ////
	[Authorize] 
	public class ShoppingCartController : Controller
	{
		//// Constructor ////

		private readonly IShoppingCartService _shoppingCartService;
		private readonly UserManager<IdentityUser> _userManager;

		public ShoppingCartController(IShoppingCartService shoppingCartService, UserManager<IdentityUser> userManager)
		{
			this._shoppingCartService = shoppingCartService;
			this._userManager = userManager;
		}

		//// Private Methods ////

		private async Task<IdentityUser> GetUser()
		{
			IdentityUser user = await _userManager.GetUserAsync(User);
			return user;
		}

		private IActionResult Result(InternalStatus result, string okStatusMessage)
		{
			switch (result)
			{
				case InternalStatus.EverythingOk:
					TempData["ShoppingCartMessageColor"] = "alert alert-dismissible alert-success";
					TempData["ShoppingCartMessage"] = okStatusMessage;
					return Redirect("/ShoppingCart/Index");
				case InternalStatus.DatabaseError:
					TempData["ShoppingCartMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ShoppingCartMessage"] = "Transaction Rejected! Database Error!";
					return Redirect("/ShoppingCart/Index");
				case InternalStatus.ProductNotFound:
					TempData["ShoppingCartMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ShoppingCartMessage"] = "Transaction Rejected! Product Not Found!";
					return Redirect("/ShoppingCart/Index");
				case InternalStatus.OrderNotFound:
					TempData["ShoppingCartMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ShoppingCartMessage"] = "Transaction Rejected! Order Not Found!";
					return Redirect("/ShoppingCart/Index");
				case InternalStatus.UserDetailNotFound:
					return Redirect("/UserDetail/Create");
				case InternalStatus.QuantityToSmall:
					TempData["ShoppingCartMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ShoppingCartMessage"] = "Transaction Rejected! Quantity To Small!";
					return Redirect("/ShoppingCart/Index");
				case InternalStatus.BadFileExtension:
					TempData["ShoppingCartMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ShoppingCartMessage"] = "Transaction Rejected! I Need A .png File!";
					return Redirect("/ShoppingCart/Index");
				case InternalStatus.DefaultOption:
					TempData["ShoppingCartMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ShoppingCartMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/ShoppingCart/Index");
				default:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/ShoppingCart/Index");
			}
		}
		
		//// Pages ////

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View(await _shoppingCartService.GetCart(await GetUser()));
		}

		[HttpPost]
		public async Task<IActionResult> Add(Guid id, int productQuantity)
		{
			InternalStatus result = await _shoppingCartService.AddProduct(id, await GetUser(), productQuantity);

			// New Way For Status OK :(
			if (result == InternalStatus.EverythingOk)
			{
				TempData["ProductMessageColor"] = "alert alert-dismissible alert-success";
				TempData["ProductMessage"] = "Product Added To Cart!";
				return Redirect("/Product/Index");
			}

			return Result(result, "This Should Never Happen!");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(Guid shoppingCartID)
		{
			InternalStatus result = await _shoppingCartService.RemoveProduct(shoppingCartID, await GetUser());
			return Result(result, "The Action Was Successful!");
		}

		[HttpPost]
		public async Task<IActionResult> ClearCart()
		{
			InternalStatus result = await _shoppingCartService.ClearCart(await GetUser());
			return Result(result, "The Action Was Successful!");
		}
	}
}
