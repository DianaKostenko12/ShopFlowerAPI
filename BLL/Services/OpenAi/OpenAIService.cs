using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OpenAi
{
    internal class OpenAIService : IOpenAIService
    {
        public Task<string> GenerateBouquetDescriptionAsync(string prompt, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GenerateBouquetImageAsync(string imagePrompt, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
