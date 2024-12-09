using System.Net.Http;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_separal.Data;
using Fall2024_Assignment3_separal.Models;

public class BooksController : Controller
{
    private readonly string _apiKey;
    private readonly string _endpointUrl;
    private readonly ILogger<BooksController> _logger;

    private readonly ApplicationDbContext _context;

    private readonly HttpClient _httpClient;

    public BooksController(ApplicationDbContext context, HttpClient httpClient, ILogger<BooksController> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["AIService:ApiKey"];
        _endpointUrl = configuration["AIService:EndpointUrl"];
        _context = context;
    }


    // Index - Get all books
    public async Task<IActionResult> Index()
    {
        var books = await _context.Books.ToListAsync();
        return View(books);
    }

   

    // Create - GET
    public IActionResult Create()
    {
        return View();
    }

    // Create - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Author,Year,Genre,Rating,ImageLink")] Book book)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            }
        }
        return View(book);
    }

    // Edit - GET
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // Edit - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Author,Year,Genre,Rating,ImageLink")] Book book)
    {
        if (id != book.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Error updating the book.");
            }
        }
        return View(book);
    }

    // Delete - GET
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FirstOrDefaultAsync(m => m.ID == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // Delete - POST
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        // Fetch AI-generated description
        var description = await GetDescriptionForBook(book.Title);

        // Fetch AI-generated reviews
        var reviews = await GetReviewsForBook(book.Title);

        // Create a view model to pass data to the view
        var viewModel = new BookDetailsViewModel
        {
            Book = book,
            Description = description,
            Reviews = reviews
        };

        return View(viewModel);
    }

    private async Task<string> GetDescriptionForBook(string bookTitle)
    {
        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are an AI assistant that generates book descriptions." },
                new { role = "user", content = $"Provide a brief description for the book titled '{bookTitle}'." }
            },
            max_tokens = 300,
            temperature = 0.7,
            top_p = 0.95
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

        var response = await _httpClient.PostAsync(_endpointUrl, content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonDocument.Parse(responseContent);
        if (result.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
        {
            return choices[0].GetProperty("message").GetProperty("content").GetString()?.Trim() ?? "No description available.";
        }
        return "No description available.";
    }

    private async Task<List<string>> GetReviewsForBook(string bookTitle)
    {
        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are an AI assistant that writes reviews about books." },
                new { role = "user", content = $"Write ten short reviews for the book titled '{bookTitle}'." }
            },
            max_tokens = 800,
            temperature = 0.7,
            top_p = 0.95
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

        var response = await _httpClient.PostAsync(_endpointUrl, content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonDocument.Parse(responseContent);
        if (result.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
        {
            var reviews = choices[0].GetProperty("message").GetProperty("content").GetString();
            return reviews.Split('\n').Where(r => !string.IsNullOrEmpty(r)).Take(10).ToList();
        }
        return new List<string> { "No reviews available." };
    }
}
