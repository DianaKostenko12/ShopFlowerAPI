using DAL.Data.UnitOfWork;
using DAL.Models;

namespace BLL.Services.WrappingPapers
{
    public class WrappingPaperService : IWrappingPaperService
    {
        private readonly IUnitOfWork _uow;
        public WrappingPaperService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IEnumerable<WrappingPaper>> GetWrappingPapersAsync()
        {
            return await _uow.WrappingPaperRepository.FindAllAsync();
        }
    }
}
