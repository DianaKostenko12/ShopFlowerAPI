namespace BLL.Services.BouquetGeneration.Descriptors
{
    public record GenerateBouquetDescriptor(
         List<string> Color,
         decimal Budget,
         string Style,
         string Shape,
         string AdditionalComment
    );
}
