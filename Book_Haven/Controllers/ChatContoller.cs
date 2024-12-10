using Fall2024_Assignment3_separal.Controllers;
using Fall2024_Assignment3_separal.Models;
using Fall2024_Assignment3_separal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

public class ChatController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _apiKey;
    private readonly string _endpointUrl;

    public ChatController(ApplicationDbContext context, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
       
        _apiKey = configuration["AIService:ApiKey"];
        _endpointUrl = configuration["AIService:EndpointUrl"];
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }


    [HttpPost]
public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
{
    if (request == null || string.IsNullOrWhiteSpace(request.Message))
    {
        return BadRequest("Message cannot be empty.");
    }

    var message = request.Message;
    Console.WriteLine(message);

    var client = _clientFactory.CreateClient("AIClient");

    var requestBody = new
    {
        messages = new[] 
        {
            new { role = "system", content = "You are an AI librarian assistant." },
            new { role = "user", content = message }
        },
        max_tokens = 150,
        temperature = 0.7,
        top_p = 0.95
    };

    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
    Console.WriteLine(content);

    try
    {
        client.DefaultRequestHeaders.Add("api-key", _apiKey);
        var response = await client.PostAsync(_endpointUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var aiResponse = JsonConvert.DeserializeObject<AIResponse>(responseContent);

            if (aiResponse?.choices != null && aiResponse.choices.Length > 0 && aiResponse.choices[0].message != null)
            {
                return Json(new { response = aiResponse.choices[0].message.content.Trim() });
            }
            else
            {
                return Json(new { response = "AI returned an incomplete response." });
            }
        }
        else
        {
            return StatusCode((int)response.StatusCode, new { error = "AI service failed" });
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "An unexpected error occurred" });
    }
}


public class AIResponse
{
    public Choice[] choices { get; set; }
}

public class Choice
{
    public Message message { get; set; }
}

public class Message
{
    public string content { get; set; }
}

}
