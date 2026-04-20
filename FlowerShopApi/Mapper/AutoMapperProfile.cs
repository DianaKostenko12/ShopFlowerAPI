using AutoMapper;
using BLL.Services.Auth.Descriptors;
using BLL.Services.Bouquets;
using BLL.Services.Bouquets.Descriptors;
using BLL.Services.Flowers.Descriptors;
using DAL.Models;
using DAL.Models.Flowers;
using DAL.Models.Orders;
using FlowerShopApi.DTOs;
using FlowerShopApi.DTOs.Bouquets;
using FlowerShopApi.DTOs.Categories;
using FlowerShopApi.DTOs.Colors;
using FlowerShopApi.DTOs.Flowers;
using FlowerShopApi.DTOs.Orders;
using FlowerShopApi.DTOs.Users;
using FlowerShopApi.DTOs.WrappingPapers;

namespace FlowerShopApi.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, RegisterDescriptor>();
            CreateMap<LoginRequest, LoginDescriptor>();

            CreateMap<FlowerResponse, Flower>();
            CreateMap<Flower, FlowerResponse>()
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.PhotoFileName))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorName : null))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null));

            CreateMap<CreateFlower, CreateFlowerDescriptor>();
            CreateMap<CreateFlowerDescriptor, CreateFlower>();

            CreateMap<FlowerResponse, UpdateFlowerDescriptor>();
            CreateMap<UpdateFlowerDescriptor, FlowerResponse>();

            CreateMap<Category, CategoryResponse>();
            CreateMap<CreateCategoryRequest, Category>();

            CreateMap<Color, ColorResponse>();
            CreateMap<CreateColorRequest, Color>();

            CreateMap<WrappingPaper, WrappingPaperResponse>()
                .ForMember(dest => dest.ColorId, opt => opt.MapFrom(src => src.ColorId))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorName : null));
            CreateMap<CreateWrappingPaperRequest, WrappingPaper>();

            CreateMap<FlowerQuantityRequest, FlowerQuantityDescriptor>();
            CreateMap<CreateBouquetRequest, CreateBouquetDescriptor>();
            CreateMap<CreateAIBouquetRequest, CreateBouquetDescriptor>();

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

            CreateMap<User, UserResponse>();
        }
    }
}
