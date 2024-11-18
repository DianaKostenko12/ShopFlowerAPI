using AutoMapper;
using BLL.Services.Orders.Descriptors;
using DAL.Data.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;

        public OrderService(IUnitOfWork uow, IMapper mapper, UserManager<User> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task AddOrderAsync(CreateOrderDescriptor descriptor, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            //var newOrder = new Order()
            //{
            //   OrderDate = DateTime.Now,
            //   DeliveryStreet = descriptor.DeliveryStreet,
            //   Status = 
            //    User = user,
            //};
            //await _uow.BouquetRepository.AddAsync(newBouquet);

            //if (descriptor.Flowers != null && descriptor.Flowers.Any())
            //{
            //    List<BouquetFlower> newBouquetFlowers = new List<BouquetFlower>();

            //    foreach (var flower in descriptor.Flowers)
            //    {
            //        newBouquetFlowers.Add(new BouquetFlower()
            //        {
            //            Bouquet = newBouquet,
            //            FlowerId = flower.FlowerId,
            //            FlowerCount = flower.FlowerCount
            //        });
            //    }

            //    await _uow.BouquetFlowerRepository.AddRangeAsync(newBouquetFlowers);
            //}

            await _uow.BouquetRepository.Save();
        }
    }
}
