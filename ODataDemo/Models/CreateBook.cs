namespace ODataDemo.Models;

public class CreateBook
{
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public IEnumerable<int> AuthorIds { get; set; } = default!;
}
