using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataDemo.Data;
using ODataDemo.Models;

namespace ODataDemo.Controllers;

public class BooksController : ODataController
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var books = 
            await _context
                .Books
                .Include(book => book.PriceOffers)
                .Include(book => book.Authors)
                .ToListAsync();

        return Ok(books);
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        var book = await _context
            .Books
            .Include(book => book.PriceOffers)
            .Include(book => book.Authors)
            .SingleOrDefaultAsync(book => book.Id == key);

        if (book is null)
        {
            return NotFound($"Book with ID {key} not found.");
        }

        return Ok(book);
    }

    [EnableQuery]
    public async Task<IActionResult> GetAuthors(int key)
    {
        var authors = 
            (await _context.Books.Include(book => book.Authors).SingleOrDefaultAsync(book => book.Id == key))?
            .Authors;

        if (authors is null)
        {
            return NotFound();
        }

        return Ok(authors);
    }

    [EnableQuery]
    public async Task<IActionResult> GetPriceOffers(int key)
    {
        var priceOffers = 
            (await _context.Books.Include(book => book.PriceOffers).SingleOrDefaultAsync(book => book.Id == key))?
            .PriceOffers;

        if (priceOffers is null)
        {
            return NotFound();
        }

        return Ok(priceOffers);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] Book book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { key = book.Id }, book.Id);
    }

    [EnableQuery]
    public async Task<IActionResult> Put(int key, [FromBody] Book book)
    {
        if (key != book.Id)
        {
            ModelState.AddModelError("id", "The entered key does not match the book ID.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var bookInDb = await _context.Books.SingleOrDefaultAsync(b => b.Id == key);
        if (bookInDb is null)
        {
            return NotFound($"Product with ID {key} not found.");
        }

        bookInDb.Title = book.Title;
        bookInDb.Price = book.Price;
        bookInDb.Authors = book.Authors;
        bookInDb.PriceOffers = book.PriceOffers;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [EnableQuery]
    public async Task<IActionResult> Delete(int key)
    {
        var bookToDelete = await _context.Books.SingleOrDefaultAsync(book => book.Id == key);
        if (bookToDelete is null)
        {
            return NotFound($"Book with ID {key} not found.");
        }
        _context.Books.Remove(bookToDelete);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
