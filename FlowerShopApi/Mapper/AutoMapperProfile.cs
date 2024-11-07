using AutoMapper;
using BLL.Services.Auth.Descriptors;
using FlowerShopApi.DTOs;

namespace FlowerShopApi.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, RegisterDescriptor>();
            CreateMap<LoginRequest, LoginDescriptor>();
        }
    }
}
