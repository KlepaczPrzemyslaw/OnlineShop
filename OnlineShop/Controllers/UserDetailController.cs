using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Context;
using OnlineShop.Models;
using OnlineShop.Services.IServices;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
	///////////////////////////
	/// For Admin and User 
	///////////////////////////
	[Authorize]
	public class UserDetailController : Controller
	{
		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Constructor 
		/////////////////////////////////////////////////////////////////////////////////////////////

		private readonly IUserDetailService _userDetailService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _context;

		public UserDetailController(IUserDetailService userDetailService, 
			UserManager<IdentityUser> userManager, ApplicationDbContext context)
		{
			this._userDetailService = userDetailService;
			this._userManager = userManager;
			this._context = context;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Private Methods 
		/////////////////////////////////////////////////////////////////////////////////////////////

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
					TempData["UserMessageColor"] = "alert alert-dismissible alert-success";
					TempData["UserMessage"] = okStatusMessage;
					return Redirect("/Identity/Account/Manage");
				case InternalStatus.DatabaseError:
					TempData["UserMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["UserMessage"] = "Transaction Rejected! Database Error!";
					return Redirect("/Identity/Account/Manage");
				case InternalStatus.ProductNotFound:
					TempData["UserMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["UserMessage"] = "Transaction Rejected! Product Not Found!";
					return Redirect("/Identity/Account/Manage");
				case InternalStatus.OrderNotFound:
					TempData["UserMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["UserMessage"] = "Transaction Rejected! Order Not Found!";
					return Redirect("/Identity/Account/Manage");
				case InternalStatus.UserDetailNotFound:
					return Redirect("/UserDetail/Create");
				case InternalStatus.QuantityToSmall:
					TempData["UserMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["UserMessage"] = "Transaction Rejected! Quantity To Small!";
					return Redirect("/Identity/Account/Manage");
				case InternalStatus.BadFileExtension:
					TempData["UserMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["UserMessage"] = "Transaction Rejected! I Need A .png File!";
					return Redirect("/Identity/Account/Manage");
				case InternalStatus.DefaultOption:
					TempData["UserMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["UserMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/Identity/Account/Manage");
				default:
					TempData["UserMessageColor"] = "alert alert-dismissible alert-danger";
					TempData["UserMessage"] = "Not Implemented! Contact With Developer!";
					return Redirect("/Identity/Account/Manage");
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/// Pages 
		/////////////////////////////////////////////////////////////////////////////////////////////

		[HttpGet]
		[AllowAnonymous]
		public IActionResult EmailConfirmationInfo()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> ShowBoughtProducts()
		{
			var myUser = await GetUser();
			return View(await _userDetailService.ShowMyOrders(myUser));
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,City,PostCode,Address")] UserDetailViewModel userDetailViewModel)
		{
			if (ModelState.IsValid)
			{
				var myUser = await GetUser();

				if (_context.UserDetailModel.FirstOrDefault(x => x.ID == myUser.Id) != null)
					return Redirect("/UserDetail/Edit");

				InternalStatus result = await _userDetailService.CreateAsync(userDetailViewModel, myUser);
				return Result(result, "Saved!");
			}

			return View(userDetailViewModel);
		}

		[HttpGet]
		public async Task<IActionResult> Edit()
		{
			var myUser = await GetUser();
			UserDetailViewModel userDetail = await _userDetailService.GetUserDetail(myUser.Id);

			if (userDetail == null)
				return Redirect("/UserDetail/Create");

			return View(userDetail);
		}

		[HttpPost]
		public async Task<IActionResult> Edit([Bind("ID,FirstName,LastName,City,PostCode,Address")] UserDetailViewModel userDetailViewModel)
		{
			var myUser = await GetUser();
			InternalStatus result = _userDetailService.Edit(userDetailViewModel, myUser);
			return Result(result, "Edited!");
		}
	}
}
