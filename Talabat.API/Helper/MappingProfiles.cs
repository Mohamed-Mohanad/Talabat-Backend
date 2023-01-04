using AutoMapper;
using Talabat.API.Dtos;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Identity;
using Talabat.DAL.Entities.Order;

namespace Talabat.API.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType!.Name))
                .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => src.ProductBrand!.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<ProductUrlResolver>());

            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();

            CreateMap<BasketItem, BasketItemDto>().ReverseMap();

            CreateMap<UserAddress, AddressDto>().ReverseMap();

            CreateMap<ShippingAddress, AddressDto>().ReverseMap();


            CreateMap<Order, UserOrderDto>()
                .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod.ShortName))
                .ForMember(dest => dest.DeliveryCost, opt => opt.MapFrom(src => src.DeliveryMethod.Cost))
                .ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ItemOrdered.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ItemOrdered.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<OrderItemUrlResolver>())
                .ReverseMap();


        }
    }
}
