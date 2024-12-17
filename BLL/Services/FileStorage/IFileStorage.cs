using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.FileStorage
{
    public interface IFileStorage
    {
        Task<string> AddFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileName);
    }
}
