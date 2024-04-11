using Microsoft.EntityFrameworkCore;
using ODataDemo.Models;

namespace ODataDemo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {        
    }

    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<Author> Authors { get; set; } = default!;
    public DbSet<PriceOffer> PriceOffers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceOffer>()
            .HasOne(offer => offer.Book).WithMany(book => book.PriceOffers)
            .HasForeignKey(offer => offer.BookId).HasPrincipalKey(book => book.Id);

        modelBuilder.Entity<Author>()
            .HasMany(author => author.Books).WithMany(book => book.Authors);
    }
}
