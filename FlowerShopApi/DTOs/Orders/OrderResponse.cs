using DAL.Models.Orders;

namespace FlowerShopApi.DTOs.Orders
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string DeliveryStreet { get; set; }
        public List<BouquetDetails> Bouquets { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Price { get; set; }
    }
}
