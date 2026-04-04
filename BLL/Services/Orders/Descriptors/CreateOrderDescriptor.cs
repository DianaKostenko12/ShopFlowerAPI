namespace BLL.Services.Orders.Descriptors
{
    public class CreateOrderDescriptor
    {
       public string DeliveryStreet {  get; set; }
       public List<BouquetQuantityDescriptor> Bouquets { get; set; }
    }
}
