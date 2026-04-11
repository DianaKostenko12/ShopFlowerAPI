using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.OpenAi;
using BLL.Services.OpenAi.Dto;
using BLL.Services.WrappingPapers;
using DAL.Exceptions;
using System.Net;
using FlowerCompositionDto = BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition.Dto.FlowerComposition;

namespace BLL.Services.BouquetGeneration.BouquetPlanner
{
    public class BouquetPlanner : IBouquetPlanner
    {
        private const int MaxGenerationAttempts = 3;

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
            GptStyleRecommendation aiStyleAdvice = null;
            List<FlowerCompositionDto> completedComposition = null;

            for (int attempt = 1; attempt <= MaxGenerationAttempts; attempt++)
            {
                aiStyleAdvice = await _openAIService.GenerateBouquetDescriptionAsync(descriptor);

                var primaryColors = aiStyleAdvice.Palette.Primary ?? [];
                var accentColors = aiStyleAdvice.Palette.Accent ?? [];

                var flowersWithRole = await _flowerSelector.SelectFlowersWithRolesAsync(aiStyleAdvice, primaryColors, accentColors);

                completedComposition = _flowerCompositionBuilder.BuildFlowersComposition(flowersWithRole, aiStyleAdvice, descriptor.Budget);

                if (completedComposition.Count > 0)
                {
                    break;
                }
            }

            if (completedComposition == null || completedComposition.Count == 0)
            {
                throw new BusinessException(HttpStatusCode.UnprocessableEntity,
                    "Не вдалося згенерувати букет за заданими параметрами. Спробуйте змінити кольори або стиль.");
            }

            var selectedWrappingPaper = await _wrappingPaperService.SelectBestMatchAsync(
                aiStyleAdvice.WrappingPaper.Colors ?? [],
                aiStyleAdvice.WrappingPaper.Pattern,
                aiStyleAdvice.WrappingPaper.Type);

            return new BouquetDetails(aiStyleAdvice.BouquetName, completedComposition, selectedWrappingPaper, descriptor.Shape);
        }
    }
}
