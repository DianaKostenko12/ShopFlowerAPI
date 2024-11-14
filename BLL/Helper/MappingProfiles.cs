using AutoMapper;
using BLL.Services.Flowers.Descriptors;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
