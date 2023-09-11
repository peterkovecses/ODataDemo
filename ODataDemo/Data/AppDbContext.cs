using Microsoft.EntityFrameworkCore;
using ODataDemo.Models;

namespace ODataDemo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {        
    }

    public DbSet<Product> Products { get; set; }
}
