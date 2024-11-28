using AutoMapper;
using DAL.Data.UnitOfWork;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OrderBouquets
{
    public class OrderBouquetService : IOrderBouquetService
    {
        private readonly IUnitOfWork _uow;

        public OrderBouquetService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
        }

        public async Task<List<OrderBouquet>> GetByOrderIdAsync(int orderId)
        {
            return await _uow.OrderBouquetRepository.GetByOrderIdAsync(orderId);
        }
    }
}
