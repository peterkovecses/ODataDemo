namespace ODataDemo.Models;

public class PriceOffer
{
    public int Id { get; set; }
    public decimal Price { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = default!;
}
