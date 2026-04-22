using BLL.Services.OpenAi.Dto;

namespace BLL.Services.BouquetGeneration.Descriptors
{
    public record GenerateBouquetDescriptor(
         List<ColorPreference> Color,
         decimal Budget,
         string Style,
         string Shape,
         string AdditionalComment
    );
}
