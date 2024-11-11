using AutoMapper;
using Azure.Core;
using BLL.Services.Flowers.Descriptors;
using DAL.Models;
using DAL.Repositories.Flowers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Flowers
{
    public class FlowerService : IFlowerService
    {
        private readonly IFlowerRepository _flowerRepository;
        private readonly IMapper _mapper;

        public FlowerService(IFlowerRepository flowerRepository, IMapper mapper)
        {
            _flowerRepository = flowerRepository;
            _mapper = mapper;
        }
        public async Task AddFlowerAsync(CreateFlowerDescriptor descriptor)
        {
            if(descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var flowerToCreate = _mapper.Map<Flower>(descriptor);

            await _flowerRepository.AddAsync(flowerToCreate);

            await _flowerRepository.Save();
        }

        public async Task DeleteFlowerAsync(int flowerId)
        {
            var flowerToDelete = await _flowerRepository.FindAsync(flowerId);
            if (flowerToDelete == null)
            {
                throw new KeyNotFoundException($"Flower with ID {flowerId} was not found.");
            }

            await _flowerRepository.RemoveAsync(flowerToDelete);
            await _flowerRepository.Save();
        }

        public async Task<Flower> GetFlowerByIdAsync(int floweId)
        {
            return await _flowerRepository.FindAsync(floweId);
        }

        public async Task<IEnumerable<Flower>> GetFlowersAsync()
        {
            return await _flowerRepository.FindAllAsync();
        }

        public async Task UpdateFlowerAsync(UpdateFlowerDescriptor descriptor)
        {
            Flower flower = await _flowerRepository.FindAsync(descriptor.FlowerId);
            if (flower == null)
            {
                throw new Exception($"Flower with ID {descriptor.FlowerId} was not found.");
            }

            flower.FlowerName = descriptor.FlowerName;
            flower.FlowerCost = descriptor.FlowerCost;
            flower.FlowerCount = descriptor.FlowerCount;

            await _flowerRepository.Save();
        }
    }
}
