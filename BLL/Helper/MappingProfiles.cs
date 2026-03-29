using AutoMapper;
using BLL.Services.Flowers.Descriptors;
using DAL.Models.Flowers;

namespace BLL.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateFlowerDescriptor, Flower>();
            CreateMap<Flower, CreateFlowerDescriptor>();

            CreateMap<Flower, UpdateFlowerDescriptor>();
            CreateMap<UpdateFlowerDescriptor, Flower>();
        }
    }
}
