using AutoMapper;
using BLL.Services.Auth.Descriptors;
using BLL.Services.Bouquets;
using BLL.Services.Flowers.Descriptors;
using DAL.Models;
using DAL.Models.Orders;
using FlowerShopApi.DTOs;
using FlowerShopApi.DTOs.Bouquets;
using FlowerShopApi.DTOs.Flowers;
using FlowerShopApi.DTOs.Orders;

namespace FlowerShopApi.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, RegisterDescriptor>();
            CreateMap<LoginRequest, LoginDescriptor>();

            CreateMap<FlowerRequest, Flower>();
            CreateMap<Flower, FlowerRequest>();

            CreateMap<CreateFlower, CreateFlowerDescriptor>();
            CreateMap<CreateFlowerDescriptor, CreateFlower>();

            CreateMap<FlowerRequest, UpdateFlowerDescriptor>();
            CreateMap<UpdateFlowerDescriptor, FlowerRequest>();

            CreateMap<Bouquet, GetBouquetResponse>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Bouquets, opt => opt.MapFrom(src => src.OrderBouquets));

            CreateMap<OrderBouquet, BouquetDetails>()
                .ForMember(dest => dest.BouquetName, opt => opt.MapFrom(src => src.Bouquet.BouquetName));
        }
    }
}
