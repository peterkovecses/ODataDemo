using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataDemo.Data;
using ODataDemo.Models;

namespace ODataDemo.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
            SeedIfEmpty();
        }

        public async Task<IActionResult> Get(ODataQueryOptions<Product> options)
        {
            var products = await (options.ApplyTo(_context.Products) as IQueryable<Product>).ToListAsync();

            return Ok(products);
        }

        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            var product = await _context.Products.SingleAsync(product => product.Id == key);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { key = product.Id }, product.Id);
        }

        [EnableQuery]
        public async Task<IActionResult> Put(int key, [FromBody] Product product)
        {
            if (key != product.Id)
            {
                ModelState.AddModelError("id", "The entered key does not match the product ID.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productInDb = await _context.Products.SingleAsync(product => product.Id == key);
            productInDb.Name = product.Name;
            productInDb.Category = product.Category;
            productInDb.Price = product.Price;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [EnableQuery]
        public async Task<IActionResult> Delete(int key)
        {
            var productToDelete = await _context.Products.SingleAsync(product => product.Id == key);
            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private void SeedIfEmpty()
        {
            if (!_context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Name = "Samsung Galaxy S21", Category = "Smartphones", Price = 84999 },
                    new Product { Name = "Apple iPhone 12", Category = "Smartphones", Price = 79999 },
                    new Product { Name = "Sony WH-1000XM4", Category = "Headphones", Price = 34999 },
                    new Product { Name = "Asus Zenbook 14", Category = "Laptops", Price = 249999 },
                    new Product { Name = "Dell XPS 13", Category = "Laptops", Price = 299999 },
                    new Product { Name = "Logitech MX Master 3", Category = "Mice", Price = 17999 },
                    new Product { Name = "Bose SoundLink Revolve", Category = "Speakers", Price = 8999 },
                    new Product { Name = "Nikon D3500", Category = "Cameras", Price = 199999 },
                    new Product { Name = "Canon EOS Rebel T7", Category = "Cameras", Price = 189999 },
                    new Product { Name = "Samsung T7 Portable SSD", Category = "Storage", Price = 29999 },
                };

                _context.Products.AddRange(products);
                _context.SaveChanges();
            }
        }
    }
}
