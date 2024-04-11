using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataDemo.Data;
using ODataDemo.Models;

namespace ODataDemo.Controllers;

public class AuthorsController : ODataController
{
    private readonly AppDbContext _context;

    public AuthorsController(AppDbContext context)
    {
        _context = context;
    }

    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var authors = await _context
            .Authors
            .Include(author => author.Books)
            .ToListAsync();

        return Ok(authors);
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        var author = await _context
            .Authors
            .Include(author => author.Books)
            .ThenInclude(book => book.PriceOffers)
            .SingleOrDefaultAsync(author => author.Id == key);

        if (author is null)
        {
            return NotFound($"Author with ID {key} not found.");
        }

        return Ok(author);
    }

    [EnableQuery]
    public async Task<IActionResult> GetBooks(int key)
    {
        var books = 
            (await _context.
                Authors
                .Include(author => author.Books)
                .ThenInclude(book => book.PriceOffers)
                .SingleOrDefaultAsync(author => author.Id == key))?
            .Books;

        if (books is null)
        {
            return NotFound();
        }

        return Ok(books);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] Author author)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { key = author.Id }, author.Id);
    }

    [EnableQuery]
    public async Task<IActionResult> Put(int key, [FromBody] Author author)
    {
        if (key != author.Id)
        {
            ModelState.AddModelError("id", "The entered key does not match the author ID.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authorInDb = await _context.Authors.SingleOrDefaultAsync(a => a.Id == key);
        if (authorInDb is null)
        {
            return NotFound($"Author with ID {key} not found.");
        }

        authorInDb.Name = author.Name;
        authorInDb.Books = author.Books;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [EnableQuery]
    public async Task<IActionResult> Delete(int key)
    {
        var authorToDelete = await _context.Authors.SingleOrDefaultAsync(author => author.Id == key);
        if (authorToDelete is null)
        {
            return NotFound($"Author with ID {key} not found.");
        }
        _context.Authors.Remove(authorToDelete);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
