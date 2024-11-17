using AutoMapper;
using BLL.Services.Auth.Descriptors;
using BLL.Services.Bouquets;
using BLL.Services.Flowers.Descriptors;
using DAL.Models;
using FlowerShopApi.DTOs;
using FlowerShopApi.DTOs.Bouquets;
using FlowerShopApi.DTOs.Flowers;

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
        }
    }
}
