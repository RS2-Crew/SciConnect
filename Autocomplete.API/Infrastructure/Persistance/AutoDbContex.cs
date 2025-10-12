using Autocomplete.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Autocomplete.API.Infrastructure.Persistence;

public class AutoDbContext : DbContext
{
    public AutoDbContext(DbContextOptions<AutoDbContext> options) : base(options) { }
    public DbSet<AutocompleteEntry> Entries => Set<AutocompleteEntry>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AutocompleteEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasMaxLength(32).IsRequired();
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
            e.Property(x => x.NameNorm).HasMaxLength(256).IsRequired();
            e.HasIndex(x => new { x.Type, x.NameNorm });
        });
    }
}
