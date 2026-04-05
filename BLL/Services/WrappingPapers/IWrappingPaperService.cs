using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.WrappingPapers
{
    public interface IWrappingPaperService
    {
        Task<IEnumerable<WrappingPaper>> GetWrappingPapersAsync();
        Task<WrappingPaper> SelectBestMatchAsync(
            IEnumerable<ColorPreference> requestedColors, string pattern, string type);
    }
}
