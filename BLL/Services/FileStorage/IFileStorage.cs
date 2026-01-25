using Microsoft.AspNetCore.Http;

namespace BLL.Services.FileStorage
{
    public interface IFileStorage
    {
        Task<string> AddFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileName);
    }
}
