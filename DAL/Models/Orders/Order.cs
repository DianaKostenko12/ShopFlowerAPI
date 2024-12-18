﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Orders
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string DeliveryStreet { get; set; }
        public ICollection<OrderBouquet> OrderBouquets { get; set; }
        public User User { get; set; }

        public decimal Price => OrderBouquets?.Sum(ob => ob.Bouquet.Price * ob.BouquetCount) ?? 0;
    }
}
