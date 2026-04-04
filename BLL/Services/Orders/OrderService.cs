using AutoMapper;
using BLL.Services.BouquetAssembly;
using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.Orders.Descriptors;
using DAL.Data.UnitOfWork;
using DAL.Exceptions;
using DAL.Models;
using DAL.Models.Orders;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace BLL.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;
        private readonly IBouquetAssembly _bouquetAssembly;

        public OrderService(IUnitOfWork uow, IMapper mapper, UserManager<User> userManager, IBouquetAssembly bouquetAssembly)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
            _bouquetAssembly = bouquetAssembly;
        }

        public async Task<int> AddOrderAsync(CreateOrderDescriptor descriptor, int userId)
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
            return newOrder.OrderId;
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

        public async Task AssembleOrderBouquetsAsync(int orderId, List<BouquetQuantityDescriptor> bouquets)
        {
            bool allAssembled = true;

            foreach (var bouquetItem in bouquets)
            {
                var bouquet = await _uow.BouquetRepository.GetBouquetWithFlowersAsync(bouquetItem.BouquetId);
                if (bouquet == null)
                {
                    allAssembled = false;
                    break;
                }

                var assemblyFlowers = bouquet.BouquetsFlowers.Select(bf => new AssemblyFlowerItem(
                    bf.FlowerId,
                    bf.Flower.FlowerName,
                    bf.FlowerCount,
                    bf.Flower.HeadSizeCm,
                    bf.Flower.StemThicknessMm,
                    bf.Flower.StemKind,
                    bf.Role
                )).ToList();

                var plan = new AssemblyPlanDescriptor(
                    bouquet.BouquetId,
                    bouquet.Shape,
                    bouquet.WrappingPaperId,
                    assemblyFlowers
                );

                try
                {
                    var result = _bouquetAssembly.ExecuteAssembly(plan);

                    if (!ValidateAssemblyResult(result))
                    {
                        allAssembled = false;
                        break;
                    }
                }
                catch
                {
                    allAssembled = false;
                    break;
                }
            }

            var newStatus = allAssembled ? OrderStatus.Assembled : OrderStatus.Canceled;
            await ChangeOrderStatus(orderId, newStatus);
        }

        private bool ValidateAssemblyResult(AssemblyResult result)
        {
            if (!result.IsAssembled)
                return false;

            if (result.Coordinates == null || result.Coordinates.Count == 0)
                return false;

            if (result.FinalWidthCm <= 0)
                return false;

            if (result.Wrapping == null)
                return false;

            foreach (var coord in result.Coordinates)
            {
                if (double.IsNaN(coord.X) || double.IsInfinity(coord.X) ||
                    double.IsNaN(coord.Y) || double.IsInfinity(coord.Y))
                    return false;
            }

            return true;
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
