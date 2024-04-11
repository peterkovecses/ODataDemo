using ODataDemo.Models;

namespace ODataDemo.Data.Seed;

public class DatabaseInitializer
{
    private readonly AppDbContext _context;

    public DatabaseInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (!_context.Books.Any())
        {
            Author author0 = new() { Name = "Martin Fowler" };
            Author author1 = new() { Name = "Robert C. Martin" };
            Author author2 = new() { Name = "Steve McConnell" };
            var books = new List<Book>
            {
                new() { Title = "Refactoring: Improving the Design of Existing Code", Price = 40, Authors = new List<Author>{ author0 }},
                new() { Title = "Clean Code: A Handbook of Agile Software Craftsmanship", Price = 50, Authors = new List<Author> { author1 }},
                new() { Title = "Code Complete: A Practical Handbook of Software Construction", Price = 60, Authors = new List<Author> { author2 } },
                new() { Title = "The Art of Agile Development", Price = 45, Authors = new List<Author> { author0, author1 }}
            };
            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            _context.PriceOffers.AddRange(
                new PriceOffer { Price = 30, Book = books[0] },
                new PriceOffer { Price = 35, Book = books[1] },
                new PriceOffer { Price = 45, Book = books[2] },
                new PriceOffer { Price = 40, Book = books[3] },
                new PriceOffer { Price = 40, Book = books[3] }
            );

            await _context.SaveChangesAsync();
        }
    }
}
