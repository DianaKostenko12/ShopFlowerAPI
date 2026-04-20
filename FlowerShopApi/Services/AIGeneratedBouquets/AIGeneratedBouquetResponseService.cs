using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.Responses;
using DAL.Models;
using FlowerShopApi.DTOs.AIGeneratedBouquets;

namespace FlowerShopApi.Services.AIGeneratedBouquets
{
    public class AIGeneratedBouquetResponseService : IAIGeneratedBouquetResponseService
    {
        public GenerateAIBouquetResponse BuildResponse(GenerateBouquetResponse generatedBouquet)
        {
            var bouquetDetails = generatedBouquet.BouquetDetails;

            var bouquetInfo = new BouquetInfo(
                bouquetDetails.BouquetName,
                BuildBouquetDescription(bouquetDetails),
                bouquetDetails.FlowerComposition
                    .Select(flower => new BouquetCompositionItem(
                        flower.flower,
                        flower.Role,
                        flower.Quantity))
                    .ToList(),
                bouquetDetails.WrappingPaper.WrappingPaperId,
                bouquetDetails.Shape);

            return new GenerateAIBouquetResponse(generatedBouquet.BouquetImage, bouquetInfo);
        }

        private static string BuildBouquetDescription(BouquetDetails bouquetDetails)
        {
            var flowersDescription = string.Join(", ", bouquetDetails.FlowerComposition
                .Select(flower =>
                {
                    var flowerName = flower.flower?.FlowerName ?? "квітка";
                    return $"{flowerName} - {flower.Quantity} {GetUkrainianPieceWord(flower.Quantity)}";
                }));

            var wrappingPaper = bouquetDetails.WrappingPaper;
            var wrappingDescription = $"{MapWrappingPaperType(wrappingPaper.Type)}, колір - {wrappingPaper.Color?.ColorName ?? "не вказано"}, візерунок - {MapWrappingPaperPattern(wrappingPaper.Pattern)}";

            return $"Склад букету: {flowersDescription}. Папір обгортання: {wrappingDescription}.";
        }

        private static string GetUkrainianPieceWord(int quantity)
        {
            var lastTwoDigits = quantity % 100;
            var lastDigit = quantity % 10;

            if (lastTwoDigits is >= 11 and <= 14)
            {
                return "штук";
            }

            return lastDigit switch
            {
                1 => "штука",
                >= 2 and <= 4 => "штуки",
                _ => "штук"
            };
        }

        private static string MapWrappingPaperType(WrappingPaperType type)
        {
            return type switch
            {
                WrappingPaperType.Paper => "папір",
                WrappingPaperType.Kraft => "крафтовий папір",
                WrappingPaperType.Film => "плівка",
                WrappingPaperType.Mesh => "сітка",
                WrappingPaperType.Fabric => "тканина",
                _ => type.ToString()
            };
        }

        private static string MapWrappingPaperPattern(WrappingPaperPattern pattern)
        {
            return pattern switch
            {
                WrappingPaperPattern.Plain => "однотонний",
                WrappingPaperPattern.Lines => "смужки",
                WrappingPaperPattern.Dots => "горошок",
                WrappingPaperPattern.NoPattern => "без візерунку",
                _ => pattern.ToString()
            };
        }
    }
}
