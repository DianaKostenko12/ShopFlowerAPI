using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OpenAi
{
    internal interface IOpenAIService
    {
        Task<string> GenerateBouquetDescriptionAsync(
        string prompt,
        CancellationToken cancellationToken = default);

        Task<byte[]> GenerateBouquetImageAsync(
            string imagePrompt,
            CancellationToken cancellationToken = default);
    }
}
