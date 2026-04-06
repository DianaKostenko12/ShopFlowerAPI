using BLL.Services.Colors;
using BLL.Services.OpenAi.Dto;
using DAL.Data.UnitOfWork;
using DAL.Models;

namespace BLL.Services.WrappingPapers
{
    public class WrappingPaperService : IWrappingPaperService
    {
        private readonly IUnitOfWork _uow;
        private readonly IColorMatchingService _colorMatchingService;

        public WrappingPaperService(IUnitOfWork uow, IColorMatchingService colorMatchingService)
        {
            _uow = uow;
            _colorMatchingService = colorMatchingService;
        }

        public async Task AddWrappingPaperAsync(WrappingPaper wrappingPaper)
        {
            ArgumentNullException.ThrowIfNull(wrappingPaper);

            var color = await _uow.ColorRepository.FindAsync(wrappingPaper.ColorId);
            if (color == null)
            {
                throw new KeyNotFoundException($"Color with ID {wrappingPaper.ColorId} was not found.");
            }

            await _uow.WrappingPaperRepository.AddAsync(wrappingPaper);
            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<WrappingPaper>> GetWrappingPapersAsync()
        {
            return await _uow.WrappingPaperRepository.FindAllAsync();
        }

        public async Task<WrappingPaper> SelectBestMatchAsync(
            IEnumerable<ColorPreference> requestedColors, string pattern, string type)
        {
            var wrappingPapers = await _uow.WrappingPaperRepository.FindAllAsync();

            if (!Enum.TryParse<WrappingPaperPattern>(pattern, out var requestedPattern))
                requestedPattern = WrappingPaperPattern.Plain;

            if (!Enum.TryParse<WrappingPaperType>(type, out var requestedType))
                requestedType = WrappingPaperType.Paper;

            return wrappingPapers.FirstOrDefault(wp =>
                    _colorMatchingService.MatchesWrappingColor(wp.Color, requestedColors)
                    && wp.Pattern == requestedPattern
                    && wp.Type == requestedType
                )
                ?? wrappingPapers.FirstOrDefault();
        }
    }
}
