using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DAL.Exceptions;

namespace BLL.Services.FileStorage
{
    public class FileStorage : IFileStorage
    {
        private readonly string _uploadPath;

        public FileStorage(string uploadPath)
        {
            if (string.IsNullOrWhiteSpace(uploadPath))
            {
                throw new ArgumentException("Шлях для збереження файлів не вказаний.", nameof(uploadPath));
            }

            _uploadPath = uploadPath;
        }
        public async Task<string> AddFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BusinessException(HttpStatusCode.BadRequest, "Файл не завантажено.");
            }

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadPath, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return newFileName;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new BusinessException(HttpStatusCode.BadRequest, "Назва файлу не вказана.");
            }

            var filePath = Path.Combine(_uploadPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new BusinessException(HttpStatusCode.NotFound, "Файл не знайдено.");
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new BusinessException(HttpStatusCode.BadRequest, $"Помилка при видаленні файлу: {ex.Message}");
            }
        }
    }
}
