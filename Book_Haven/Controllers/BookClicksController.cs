using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_separal.Data;
using Fall2024_Assignment3_separal.Models;

using System.Linq;
using System.Threading.Tasks;

public class BookClicksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookClicksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookClicksData = await _context.BookClicks
            .Include(bc => bc.Book) // Include the Book navigation property
            .GroupBy(bc => bc.Book)
            .Select(group => new
            {
                Book = group.Key,
                ClickCount = group.Count()
            })
            .ToListAsync();

        var viewModel = bookClicksData.Select(data => new BookClicksIndexViewModel
        {
            BookTitle = data.Book.Title,
            ClickCount = data.ClickCount
        }).ToList();

        return View(viewModel);
    }
}


