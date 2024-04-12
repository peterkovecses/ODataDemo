namespace ODataDemo.Models;

public class UpdateBook
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public decimal Price { get; set; }

    public decimal? NewPriceOffer { get; set; }
}
