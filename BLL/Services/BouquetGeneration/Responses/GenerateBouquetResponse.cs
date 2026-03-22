using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;

namespace BLL.Services.BouquetGeneration.Responses
{
    public record GenerateBouquetResponse
    (
        byte[] BouquetImage, 
        BouquetDetails BouquetDetails
    );
}
