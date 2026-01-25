using BLL.Services.OpenAi.Dto;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BLL.Services.OpenAi.OpenAiClient
{
    internal sealed class OpenAiClient : IOpenAiClient
    {
        private readonly HttpClient _httpClient;

        public OpenAiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> ChatAsync(string prompt, string responseFormat, CancellationToken ct = default)
        {
            var payload = new StringContent(CreatePayload(prompt, responseFormat), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("/v1/chat/completions", payload);
            var response = ParseResponse(await result.Content.ReadAsStringAsync());

            return response;
        }

        private string ParseResponse(string response)
        {
            var responseObject = JsonNode.Parse(response);
            return responseObject?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? "Sorry, I don't have a response for you at this time";
        }

        private void ConfigureClient()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + OpenAiConstants.ApiKey);
            _httpClient.BaseAddress = new Uri("https://api.openai.com");
        }

        private string CreatePayload(string prompt, string responseFormat)
        {
            var request = new Payload
            {
                Model = OpenAiConstants.Model,
                Temperature = OpenAiConstants.Temperature,
                MaxTokens = OpenAiConstants.MaxTokens,
                TopP = OpenAiConstants.TopP,
                FrequencyPenalty = OpenAiConstants.FrequencyPenalty,
                PresencePenalty = OpenAiConstants.PresencePenalty,

                ResponseFormat = new ResponseFormat
                {
                    Type = responseFormat
                },

                Messages =
                [
                    new Message
                    {
                        Content =
                        [
                            new MessageContent
                            {
                                Text = prompt
                            }
                        ]
                    }
                ]
            };

            return JsonSerializer.Serialize(request);
        }
    }
}
