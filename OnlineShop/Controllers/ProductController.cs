using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Services.Interfaces;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
	public class ProductController : Controller
	{
		//// Constructor ////

		private readonly IProductService _productService;
		private readonly UserManager<IdentityUser> _userManager;

		public ProductController(IProductService productService, UserManager<IdentityUser> userManager)
		{
			this._productService = productService;
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
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-success";
					TempData["ProductMessage"] = okStatusMessage;
					return Redirect("/Product/Index");
				case InternalStatus.DatabaseError:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Transaction Rejected! Database Error!";
					return Redirect("/Product/Index");
				case InternalStatus.ProductNotFound:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Transaction Rejected! Product Not Found!";
					return Redirect("/Product/Index");
				case InternalStatus.OrderNotFound:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Transaction Rejected! Order Not Found!";
					return Redirect("/Product/Index");
				case InternalStatus.UserDetailNotFound:
					return Redirect("/UserDetail/Create");
				case InternalStatus.QuantityToSmall:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Transaction Rejected! Quantity To Small!";
					return Redirect("/Product/Index");
				case InternalStatus.BadFileExtension:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Transaction Rejected! I Need A .png File!";
					return Redirect("/Product/Index");
				case InternalStatus.DefaultOption:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/Product/Index");
				default:
					TempData["ProductMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["ProductMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/Product/Index");
			}
		}

		//// For View ////

		[HttpGet]
		public async Task<IActionResult> GetProductPhoto(Guid id)
		{
			Guid imageGuid = await _productService.GetImageGuidForProductID(id);

			if (imageGuid.Equals(Guid.Empty))
				return NotFound();

			var image = System.IO.File.OpenRead($@"wwwroot/Product/Images/{imageGuid}.png");
			return File(image, "image/jpeg");
		}

		//// Pages ////

		[HttpGet] 
		public async Task<IActionResult> Index(string id = "")
		{
			IEnumerable<ProductViewModel> products = await _productService.BrowseAsync(id);

			if (products == null)
				return NoContent();

			ViewBag.Categories = await _productService.GetCategoriesAsync();

			return View(products);
		}

		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			ProductDetailsViewModel product = await _productService.GetAsync(id);

			if (product == null)
				return BadRequest();

			return View(product);
		}

		[HttpPost]
		[Authorize] // For Admin and User
		public async Task<IActionResult> Payment()
		{
			return View(await GetUser());
		}

		[HttpPost]
		[Authorize] // For Admin and User
		public async Task<IActionResult> Buy()
		{
			InternalStatus result = await _productService.BuyAsync(await GetUser());
			return Result(result, "Transaction Approved! You bought it!");
		}

		[HttpPost]
		[Authorize] // For Admin and User
		public async Task<IActionResult> Cancel(Guid id)
		{
			InternalStatus result = await _productService.CancelAsync(id);

			// New Way For Status OK :(
			if (result == InternalStatus.EverythingOk)
			{
				TempData["CancelOrderMessageColor"] = "alert alert-dismissible alert-success";
				TempData["CancelOrderMessage"] = "Transaction Approved! You Canceled This Order!";
				return Redirect("/UserDetail/ShowBoughtProducts");
			}

			return Result(result, "This Should Never Happen!");
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create([Bind("ID,Category,Name,Description,Price,Quantity")] ProductDetailsViewModel productViewModel, IFormFile fileFromSite)
		{
			if (ModelState.IsValid)
			{
				InternalStatus result = await _productService.CreateAsync(productViewModel, fileFromSite);
				return Result(result, "Created!");
			}

			return View(productViewModel);
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(Guid id)
		{
			ProductDetailsViewModel product = await _productService.GetAsync(id);

			if (product == null)
				return BadRequest();

			return View(product);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Edit([Bind("ID,Category,Name,Description,Price,Quantity")] ProductDetailsViewModel productViewModel)
		{
			if (ModelState.IsValid)
			{
				InternalStatus result = _productService.Edit(productViewModel);
				return Result(result, "Edited!");
			}
			return View(productViewModel);
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(Guid id)
		{
			ProductDetailsViewModel product = await _productService.GetAsync(id);

			if (product == null)
				return BadRequest();

			return View(product);
		}

		[HttpPost, ActionName("Delete")]
		[Authorize(Roles = "Admin")]
		public IActionResult DeleteConfirmed(Guid id)
		{
			InternalStatus result = _productService.Delete(id);
			return Result(result, "Deleted!");
		}
	}
}