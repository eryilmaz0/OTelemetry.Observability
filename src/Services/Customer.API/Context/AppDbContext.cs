using Microsoft.EntityFrameworkCore;

namespace Customer.API.Context;

public class AppDbContext : DbContext
{
    public DbSet<Model.Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Server=customer_db;Port=5432;Database=customer_db;User Id=admin;Password=admin1234;");
    }
}