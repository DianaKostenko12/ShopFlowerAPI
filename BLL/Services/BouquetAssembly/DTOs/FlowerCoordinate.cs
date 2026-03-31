using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
namespace BLL.Services.BouquetAssembly.DTOs
{
    public record FlowerCoordinate(
        ProcessedFlower Flower,
        double X,                  
        double Y,                  
        double Z = 0.0,            
        double TiltAngle = 0.0
    );
}
