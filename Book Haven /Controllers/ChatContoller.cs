using Fall2024_Assignment3_separal.Controllers;
using Fall2024_Assignment3_separal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

public class ChatController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ChatController> _logger;
    private readonly string _apiKey;
    private readonly string _endpointUrl;

    public ChatController(ApplicationDbContext context, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
       
        _apiKey = configuration["AIService:ApiKey"];
        _endpointUrl = configuration["AIService:EndpointUrl"];
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] dynamic body)
    {
        if (body == null || string.IsNullOrWhiteSpace(body.message?.ToString()))
        {
            return BadRequest("Message cannot be empty.");
        }

        string message = body.message;

        var client = _clientFactory.CreateClient("AIClient");

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are an AI assistant." },
                new { role = "user", content = message }
            },
            max_tokens = 150
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        try
        {
            // Use the full endpoint URL for the request
            var response = await client.PostAsync(_endpointUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("AI Response: {Response}", responseContent);

                var aiResponse = JsonConvert.DeserializeObject<AIResponse>(responseContent);

                if (aiResponse?.choices != null && aiResponse.choices.Length > 0 && !string.IsNullOrEmpty(aiResponse.choices[0].text))
                {
                    return Json(new { response = aiResponse.choices[0].text.Trim() });
                }
                else
                {
                    _logger.LogWarning("AI returned no choices or invalid text.");
                    return Json(new { response = "AI returned an incomplete response." });
                }
            }
            else
            {
                _logger.LogError("Error calling AI service: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                return StatusCode((int)response.StatusCode, new { error = "AI service failed" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the message.");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    public class AIResponse
    {
        public Choice[] choices { get; set; }
    }

    public class Choice
    {
        public string text { get; set; }
    }
}
