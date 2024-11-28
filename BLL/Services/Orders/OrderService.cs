using AutoMapper;
using BLL.Services.Orders.Descriptors;
using DAL.Data.UnitOfWork;
using DAL.Exceptions;
using DAL.Models;
using DAL.Models.Orders;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var newOrder = new Order()
            {
                OrderDate = DateTime.Now,
                DeliveryStreet = descriptor.DeliveryStreet,
                Status = OrderStatus.Pending,
                User = user,
            };
            await _uow.OrderRepository.AddAsync(newOrder);

            if (descriptor.Bouquets != null && descriptor.Bouquets.Any())
            {
                List<OrderBouquet> newOrderBouquets = new List<OrderBouquet>();

                foreach (var bouquet in descriptor.Bouquets)
                {
                    var bouquetExists = await _uow.BouquetRepository.GetBouquetByIdAsync(bouquet.BouquetId);
                    if(bouquetExists == null)
                    {
                        throw new ArgumentException($"Bouquet with ID {bouquet.BouquetId} does not exist.");
                    }

                    newOrderBouquets.Add(new OrderBouquet()
                    {
                        Order = newOrder,
                        BouquetId = bouquet.BouquetId,
                        BouquetCount = bouquet.BouquetCount
                    });
                }

                await _uow.OrderBouquetRepository.AddRangeAsync(newOrderBouquets);
            }

            await _uow.CompleteAsync();
        }

        public async Task ChangeOrderStatus(int orderId, OrderStatus status)
        {
            var existOrder = await _uow.OrderRepository.FindAsync(orderId);

            if (existOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

            existOrder.Status = status;

            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _uow.OrderRepository.FindAllAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            var orders = await _uow.OrderRepository.GetOrdersByUserIdAsync(userId);
            if (!orders.Any())
            {
                throw new BusinessException(HttpStatusCode.NotFound, $"No bookings found for user with ID {userId}.");
            }
            return orders;
        }
    }
}
