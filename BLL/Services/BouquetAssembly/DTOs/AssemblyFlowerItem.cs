namespace BLL.Services.BouquetAssembly.DTOs
{
    public record AssemblyFlowerItem
    (
        int FlowerId,
        string FlowerName,
        int Quantity,
        int HeadSizeCm,
        double StemThicknessMm,
        bool Flexibility,
        string Role
    );
}
