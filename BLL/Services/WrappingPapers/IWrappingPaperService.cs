using DAL.Models;
namespace BLL.Services.WrappingPapers
{
    public interface IWrappingPaperService
    {
        Task<IEnumerable<WrappingPaper>> GetWrappingPapersAsync();
    }
}
