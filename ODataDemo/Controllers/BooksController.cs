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
    public async Task<IActionResult> Get(ODataQueryOptions<Book> options)
    {
        var books =
            await (options.ApplyTo(_context.Books.Include(book => book.PriceOffers).Include(book => book.Authors)) as IQueryable<Book>)!
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
    public async Task<IActionResult> Post([FromBody] CreateBook createBook)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authorsInDb = new List<Author>();
        foreach (var id in createBook.AuthorIds)
        {
            var authorInDb = await _context.Authors.Where(author => author.Id == id).SingleOrDefaultAsync();
            if (authorInDb is null)
            {
                return BadRequest($"Author with id {id} does not exist.");
            }

            authorsInDb.Add(authorInDb);
        }

        var newBook = new Book { Title = createBook.Title, Price = createBook.Price, Authors = authorsInDb };
        await _context.Books.AddAsync(newBook);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { key = newBook.Id }, newBook.Id);
    }

    [EnableQuery]
    public async Task<IActionResult> Put(int key, [FromBody] UpdateBook updateBook)
    {
        if (key != updateBook.Id)
        {
            ModelState.AddModelError("id", "The entered key does not match the createBook ID.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var bookInDb = await _context.Books.SingleOrDefaultAsync(book => book.Id == key);
        if (bookInDb is null)
        {
            return NotFound($"Product with ID {key} not found.");
        }

        bookInDb.Title = updateBook.Title;
        bookInDb.Price = updateBook.Price;
        if (updateBook.NewPriceOffer is not null)
        {
            _context.PriceOffers.Add(new PriceOffer { Price = updateBook.NewPriceOffer.Value, BookId = bookInDb.Id });
        }
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
