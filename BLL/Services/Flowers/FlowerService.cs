﻿using AutoMapper;
using BLL.Services.Flowers.Descriptors;
using DAL.Data.UnitOfWork;
using DAL.Models;

namespace BLL.Services.Flowers
{
    public class FlowerService : IFlowerService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public FlowerService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public async Task AddFlowerAsync(CreateFlowerDescriptor descriptor)
        {
            if(descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var flowerToCreate = _mapper.Map<Flower>(descriptor);

            await _uow.FlowerRepository.AddAsync(flowerToCreate);

            await _uow.CompleteAsync();
        }

        public async Task DeleteFlowerAsync(int flowerId)
        {
            var flowerToDelete = await _uow.FlowerRepository.FindAsync(flowerId);
            if (flowerToDelete == null)
            {
                throw new KeyNotFoundException($"Flower with ID {flowerId} was not found.");
            }

            await _uow.FlowerRepository.RemoveAsync(flowerToDelete);
            await _uow.CompleteAsync();
        }

        public async Task<Flower> GetFlowerByIdAsync(int floweId)
        {
            return await _uow.FlowerRepository.FindAsync(floweId);
        }

        public async Task<IEnumerable<Flower>> GetFlowersAsync()
        {
            return await _uow.FlowerRepository.FindAllAsync();
        }

        public async Task UpdateFlowerAsync(UpdateFlowerDescriptor descriptor)
        {
            Flower flower = await _uow.FlowerRepository.FindAsync(descriptor.FlowerId);
            if (flower == null)
            {
                throw new Exception($"Flower with ID {descriptor.FlowerId} was not found.");
            }

            flower.FlowerName = descriptor.FlowerName;
            flower.FlowerCost = descriptor.FlowerCost;
            flower.FlowerCount = descriptor.FlowerCount;

            await _uow.CompleteAsync();
        }
    }
}
