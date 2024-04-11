namespace ODataDemo.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public decimal Price { get; set; }

    public ICollection<Author> Authors { get; set; } = default!;
    public ICollection<PriceOffer> PriceOffers { get; set; } = default!;
}
