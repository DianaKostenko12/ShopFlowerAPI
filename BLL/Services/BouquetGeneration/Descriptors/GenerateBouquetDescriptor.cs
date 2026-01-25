namespace BLL.Services.BouquetGeneration.Descriptors
{
    public record GenerateBouquetDescriptor(
         string Color,
         decimal? Budget,
         string Style,
         string Shape,
         string AdditionalComment
    );
}
