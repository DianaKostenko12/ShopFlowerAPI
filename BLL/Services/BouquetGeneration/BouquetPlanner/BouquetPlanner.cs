using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.OpenAi;
using BLL.Services.WrappingPapers;

namespace BLL.Services.BouquetGeneration.BouquetPlanner
{
    public class BouquetPlanner : IBouquetPlanner
    {
        private readonly IOpenAIService _openAIService;
        private readonly IFlowerSelector _flowerSelector;
        private readonly IFlowerCompositionBuilder _flowerCompositionBuilder;
        private readonly IWrappingPaperService _wrappingPaperService;

        public BouquetPlanner(
            IOpenAIService openAiService,
            IFlowerSelector flowerSelector,
            IFlowerCompositionBuilder flowerCompositionBuilder,
            IWrappingPaperService wrappingPaperService)
        {
            _openAIService = openAiService;
            _flowerSelector = flowerSelector;
            _flowerCompositionBuilder = flowerCompositionBuilder;
            _wrappingPaperService = wrappingPaperService;
        }

        public async Task<BouquetDetails> BuildPlanAsync(GenerateBouquetDescriptor descriptor, CancellationToken ct)
        {
            var aiStyleAdvice = await _openAIService.GenerateBouquetDescriptionAsync(descriptor);

            var primaryColors = aiStyleAdvice.Palette.Primary ?? [];
            var accentColors = aiStyleAdvice.Palette.Accent ?? [];

            var flowersWithRole = await _flowerSelector.SelectFlowersWithRolesAsync(aiStyleAdvice, primaryColors, accentColors);

            var selectedWrappingPaper = await _wrappingPaperService.SelectBestMatchAsync(
                aiStyleAdvice.WrappingPaper.Colors ?? [],
                aiStyleAdvice.WrappingPaper.Pattern,
                aiStyleAdvice.WrappingPaper.Type);

            var completedComposition = _flowerCompositionBuilder.BuildFlowersComposition(flowersWithRole, aiStyleAdvice, descriptor.Budget);

            return new BouquetDetails(aiStyleAdvice.BouquetName, completedComposition, selectedWrappingPaper, descriptor.Shape);
        }
    }
}
