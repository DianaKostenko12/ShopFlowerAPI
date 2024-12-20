﻿using AutoMapper;
using Azure.Core;
using BLL.Services.Bouquets.Descriptors;
using BLL.Services.FileStorage;
using DAL.Data.UnitOfWork;
using DAL.Exceptions;
using DAL.Filters;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace BLL.Services.Bouquets
{
    public class BouquetService : IBouquetService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;
        private readonly IFileStorage _fileStorage;

        public BouquetService(IUnitOfWork uow, IMapper mapper, UserManager<User> userManager, IFileStorage fileStorage)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
            _fileStorage = fileStorage;
        }

        public async Task AddBouquetAsync(CreateBouquetDescriptor descriptor, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            try
            {
                if (descriptor.Photo == null)
                {
                    throw new ArgumentNullException(nameof(descriptor.Photo), "Photo cannot be null.");
                }

                if (_fileStorage == null)
                {
                    throw new InvalidOperationException("FileStorage service is not initialized.");
                }

                string fileName = await _fileStorage.AddFileAsync(descriptor.Photo);
                var newBouquet = new Bouquet()
                {
                    BouquetName = descriptor.BouquetName,
                    BouquetDescription = descriptor.BouquetDescription,
                    PhotoFileName = fileName,
                    User = user,
                };
                await _uow.BouquetRepository.AddAsync(newBouquet);

                if (descriptor.Flowers != null && descriptor.Flowers.Any())
                {
                    List<BouquetFlower> newBouquetFlowers = new List<BouquetFlower>();

                    foreach (var flower in descriptor.Flowers)
                    {
                        var flowerExists = await _uow.FlowerRepository.FindAsync(flower.FlowerId);
                        if (flowerExists == null)
                        {
                            throw new ArgumentException($"Flower with ID {flower.FlowerId} does not exist.");
                        }

                        newBouquetFlowers.Add(new BouquetFlower()
                        {
                            Bouquet = newBouquet,
                            FlowerId = flower.FlowerId,
                            FlowerCount = flower.FlowerCount
                        });
                    }

                    await _uow.BouquetFlowerRepository.AddRangeAsync(newBouquetFlowers);
                }

                await _uow.CompleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteBouquetAsync(int bouquetId, int userId)
        {
            var bouquetToDelete = await _uow.BouquetRepository.FindAsync(bouquetId);
            if (bouquetToDelete == null)
            {
                throw new KeyNotFoundException($"Bouquet with ID {bouquetId} was not found.");
            }
            User recordAuthor = await _userManager.FindByIdAsync(bouquetToDelete.User.Id.ToString());
            IList<string> recordAuthorRoles = await _userManager.GetRolesAsync(recordAuthor);

            User editor = await _userManager.FindByIdAsync(userId.ToString());
            IList<string> editorRoles = await _userManager.GetRolesAsync(editor);

            if (recordAuthorRoles.Contains(Roles.Admin) && !editorRoles.Contains(Roles.Admin))
            {
                throw new BusinessException(HttpStatusCode.Forbidden, "Ви не маєте права");
            }

            if (!recordAuthorRoles.Contains(Roles.Admin) && !editorRoles.Contains(Roles.Admin) && recordAuthor.Id != editor.Id)
            {
                throw new BusinessException(HttpStatusCode.Forbidden, "Ви не маєте права");
            }

            await _uow.BouquetRepository.RemoveAsync(bouquetToDelete);
            await _uow.CompleteAsync();
        }

        public async Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId)
        {
            var bouquets = await _uow.BouquetRepository.GetBouquetsByUserIdAsync(userId);
            if (!bouquets.Any())
            {
                throw new KeyNotFoundException($"No bouquets found for user with ID {userId}.");
            }
            return bouquets;
        }

        public async Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view, int? userId)
        {
            List<Bouquet> bouquets =  await _uow.BouquetRepository.GetBouquetsByFilterAsync(view);
            List<Bouquet> filterBouquets = new List<Bouquet>();
            foreach (var bouquet in bouquets)
            {
                User creator = await _userManager.FindByIdAsync(bouquet.User.Id.ToString());
                IList<string> creatorRoles = await _userManager.GetRolesAsync(creator);
                if (creatorRoles.Contains(Roles.Admin))
                {
                    filterBouquets.Add(bouquet);
                }
            }

            if(userId.HasValue)
            {
                foreach (var bouquet in bouquets)
                {
                    if (await IsUserBouquetOwnerAsync(bouquet.BouquetId, userId.Value))
                    {
                        filterBouquets.Add(bouquet);
                    }
                }
            }

            return filterBouquets.DistinctBy(x => x.BouquetId).ToList();
        }

        public async Task<bool> IsUserBouquetOwnerAsync(int bouquetId, int userId)
        {
            var bouquet = await _uow.BouquetRepository.GetBouquetByIdAsync(bouquetId);

            return bouquet != null && bouquet.User.Id == userId;
        }

        //public async Task UpdateBouquetAsync(UpdateBouquetDescriptor descriptor)
        //{
        //    Bouquet bouquet = await _uow.BouquetRepository.FindAsync(descriptor.BouquetId);
        //    if (bouquet == null)
        //    {
        //        throw new Exception($"Bouquet with ID {descriptor.BouquetId} was not found.");
        //    }

        //    bouquet.BouquetName = descriptor.BouquetName;
        //    bouquet.BouquetDescription = descriptor.BouquetDescription;

        //    if (descriptor.Flowers != null && descriptor.Flowers.Any())
        //    {
        //        var existingBouquetFlowers = await _uow.BouquetFlowerRepository.GetByBouquetIdAsync(bouquet.BouquetId);
        //        var existingFlowerIds = existingBouquetFlowers.Select(bf => bf.FlowerId).ToList();
        //        var updatedFlowerIds = descriptor.Flowers.Select(f => f.FlowerId).ToList();

        //        var flowerIdsToRemove = existingFlowerIds.Except(updatedFlowerIds).ToList();
        //        if (flowerIdsToRemove.Any())
        //        {
        //            var flowersToRemove = existingBouquetFlowers.Where(bf => flowerIdsToRemove.Contains(bf.FlowerId)).ToList();
        //            await _uow.BouquetFlowerRepository.RemoveRangeAsync(flowersToRemove);
        //        }

        //        foreach (var flowerDescriptor in descriptor.Flowers)
        //        {
        //            var existingBouquetFlower = existingBouquetFlowers.FirstOrDefault(bf => bf.FlowerId == flowerDescriptor.FlowerId);
        //            if (existingBouquetFlower != null)
        //            {
        //                existingBouquetFlower.FlowerCount = flowerDescriptor.FlowerCount;
        //            }
        //            else
        //            {
        //                var newBouquetFlower = new BouquetFlower()
        //                {
        //                    Bouquet = bouquet,
        //                    FlowerId = flowerDescriptor.FlowerId,
        //                    FlowerCount = flowerDescriptor.FlowerCount
        //                };
        //                await _uow.BouquetFlowerRepository.AddAsync(newBouquetFlower);
        //            }
        //        }
        //    }

        //    await _uow.FlowerRepository.Save();
        //}
    }
}
