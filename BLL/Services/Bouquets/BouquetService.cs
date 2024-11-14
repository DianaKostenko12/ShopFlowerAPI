using AutoMapper;
using BLL.Services.Bouquets.Descriptors;
using DAL.Data.UnitOfWork;
using DAL.Filters;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Bouquets
{
    public class BouquetService : IBouquetService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;

        public BouquetService(IUnitOfWork uow, IMapper mapper, UserManager<User> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task AddBouquetAsync(CreateBouquetDescriptor descriptor, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var newBouquet = new Bouquet()
            {
                BouquetName = descriptor.BouquetName,
                BouquetDescription = descriptor.BouquetDescription,
                User = user,
            };
            await _uow.BouquetRepository.AddAsync(newBouquet);

            if (descriptor.Flowers != null && descriptor.Flowers.Any())
            {
                List<BouquetFlower> newBouquetFlowers = new List<BouquetFlower>();

                foreach (var flower in descriptor.Flowers)
                {
                    newBouquetFlowers.Add(new BouquetFlower()
                    {
                        Bouquet = newBouquet,
                        FlowerId = flower.FlowerId,
                        FlowerCount = flower.FlowerCount
                    });
                }

                await _uow.BouquetFlowerRepository.AddRangeAsync(newBouquetFlowers);
            }

            await _uow.BouquetRepository.Save();
        }

        public async Task DeleteBouquetAsync(int bouquetId)
        {
            var bouquetToDelete = await _uow.BouquetRepository.FindAsync(bouquetId);
            if (bouquetToDelete == null)
            {
                throw new KeyNotFoundException($"Bouquet with ID {bouquetId} was not found.");
            }

            await _uow.BouquetRepository.RemoveAsync(bouquetToDelete);
            await _uow.BouquetRepository.Save();
        }

        public async Task<List<Bouquet>> GetBouquetByUserIdAsync(int userId)
        {
            var bouquets = await _uow.BouquetRepository.GetBouquetsByUserIdAsync(userId);
            if (!bouquets.Any())
            {
                throw new KeyNotFoundException($"No bouquets found for user with ID {userId}.");
            }
            return bouquets;
        }

        public Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view)
        {
            return _uow.BouquetRepository.GetBouquetsByFilterAsync(view);
        }

        public async Task UpdateBouquetAsync(UpdateBouquetDescriptor descriptor)
        {
            Bouquet bouquet = await _uow.BouquetRepository.FindAsync(descriptor.BouquetId);
            if (bouquet == null)
            {
                throw new Exception($"Bouquet with ID {descriptor.BouquetId} was not found.");
            }

            bouquet.BouquetName = descriptor.BouquetName;
            bouquet.BouquetDescription = descriptor.BouquetDescription;

            if (descriptor.FlowerIds != null)
            {
                var existingBouquetFlowers = await _uow.BouquetFlowerRepository.GetByBouquetIdAsync(bouquet.BouquetId);
                var existingFlowerIds = existingBouquetFlowers.Select(bf => bf.FlowerId).ToList();

                var flowerIdsToRemove = existingFlowerIds.Except(descriptor.FlowerIds).ToList();
                if (flowerIdsToRemove.Any())
                {
                    var flowersToRemove = existingBouquetFlowers.Where(bf => flowerIdsToRemove.Contains(bf.FlowerId)).ToList();
                    await _uow.BouquetFlowerRepository.RemoveRangeAsync(flowersToRemove);
                }

                var flowerIdsToAdd = descriptor.FlowerIds.Except(existingFlowerIds).ToList();
                if (flowerIdsToAdd.Any())
                {
                    var newBouquetFlowers = flowerIdsToAdd.Select(flowerId => new BouquetFlower()
                    {
                        Bouquet = bouquet,
                        FlowerId = flowerId,
                        FlowerCount = descriptor.FlowerCount,
                    }).ToList();
                    await _uow.BouquetFlowerRepository.AddRangeAsync(newBouquetFlowers);
                }
            }

            await _uow.FlowerRepository.Save();
        }
    }
}
