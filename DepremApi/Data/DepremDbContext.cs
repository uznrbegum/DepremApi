using DepremApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DepremApi.Data;

public class DepremDbContext : DbContext
{
    public DepremDbContext(DbContextOptions<DepremDbContext> options)
        : base(options)
    {
    }

    public DbSet<Deprem> Depremler { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deprem>()
            .HasIndex(d => d.EventId) // EventId değerleri tekrar edemez(duplicate koruması)
            .IsUnique();
    }
}