using System.Text;
using Newtonsoft.Json.Linq;

namespace VoiceCommandAPI.Services
{
    public class GroqService
    {
        private readonly IConfiguration _configuration;

        public GroqService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetIntentAsync(string userSpeech)
        {
            var apiKey = _configuration["Groq:ApiKey"] ?? "";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var systemPrompt = "You are a voice command assistant. The user will say something and you must identify the intent. Supported commands: hello, exit, clear, open notepad, open chrome, open calculator, open paint, show date, show time, lock screen, minimize window, maximize window. If the user speech matches any supported command even loosely return that exact command text only. If it does not match any command return unknown. Return ONLY the command text nothing else.";

            var requestBody = new JObject
            {
                ["model"] = "llama-3.3-70b-versatile",
                ["messages"] = new JArray
                {
                    new JObject { ["role"] = "system", ["content"] = systemPrompt },
                    new JObject { ["role"] = "user", ["content"] = userSpeech }
                },
                ["max_tokens"] = 20,
                ["temperature"] = 0.1
            };

            var content = new StringContent(
                requestBody.ToString(),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions", content);

            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Groq API error: {responseJson}");

            var parsed = JObject.Parse(responseJson);
            var intent = parsed["choices"]?[0]?["message"]?["content"]?.ToString().Trim().ToLower();

            return intent ?? "unknown";
        }
    }
}