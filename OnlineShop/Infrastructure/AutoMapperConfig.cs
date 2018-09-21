using System;
using AutoMapper;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Infrastructure
{
	public static class AutoMapperConfig
	{
		public static IMapper Initialize()
			=> new MapperConfiguration(cfg =>
			{
				// x -> y
				cfg.CreateMap<ProductModel, ProductViewModel>();
				cfg.CreateMap<ProductModel, ProductDetailsViewModel>();
				cfg.CreateMap<ProductDetailsViewModel, ProductModel>()
					.ForMember(x => x.Updated, o => o.UseValue(DateTime.UtcNow));

				// x -> y
				cfg.CreateMap<UserDetailViewModel, UserDetailModel>();
				cfg.CreateMap<UserDetailModel, UserDetailViewModel>();

				// x -> y 
				cfg.CreateMap<UserDetailModel, OrderViewModelForAdmin>()
					.ForMember(d => d.ID, o => o.Ignore());
				cfg.CreateMap<ProductModel, OrderViewModelForAdmin>()
					.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Name)).ForAllOtherMembers(o => o.Ignore());
				cfg.CreateMap<OrderModel, OrderViewModelForAdmin>();

				// x -> y
				cfg.CreateMap<ShoppingCartModel, ShoppingCartViewModel>()
					.ForMember(d => d.ID, o => o.MapFrom(s => s.ID))
					.ForMember(d => d.BoughtAmount, o => o.MapFrom(s => s.BoughtAmount))
					.ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
					.ForAllOtherMembers(o => o.Ignore());
				cfg.CreateMap<ProductModel, ShoppingCartViewModel>()
					.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Name))
					.ForAllOtherMembers(o => o.Ignore());

				// x -> y
				cfg.CreateMap<ShoppingCartModel, OrderModel>()
					.ForMember(d => d.ID, o => o.Ignore())
					.ForMember(d => d.Status, o => o.UseValue(OrderStatus.New));

			}).CreateMapper();
	}
}