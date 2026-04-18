using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.WrappingPapers
{
    public interface IWrappingPaperService
    {
        Task<WrappingPaper> AddWrappingPaperAsync(WrappingPaper wrappingPaper);
        Task<IEnumerable<WrappingPaper>> GetWrappingPapersAsync();
        Task DeleteWrappingPaperAsync(int wrappingPaperId);
        Task<WrappingPaper> SelectBestMatchAsync(
            IEnumerable<ColorPreference> requestedColors, string pattern, string type);
    }
}
