using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataDemo.Data;
using ODataDemo.Data.Seed;
using ODataDemo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ODataDemoDatabase")));

builder.Services.AddScoped<DatabaseInitializer>();

builder.Services.AddControllers()
    .AddOData(opt => opt.AddRouteComponents("odata", GetEdmModel()).Filter().Select().OrderBy().Expand());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services.CreateScope()
    .ServiceProvider.GetRequiredService<DatabaseInitializer>()
    .SeedDataAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();
    builder.EntityType<PriceOffer>();
    builder.EntitySet<Book>("Books");
    builder.EntitySet<Author>("Authors");

    return builder.GetEdmModel();
}