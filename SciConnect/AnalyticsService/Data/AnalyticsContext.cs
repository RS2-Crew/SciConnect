using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Data
{
    // Simple entities to represent the main database entities for analytics
    public class Institution
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Analysis
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class Instrument
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Keyword
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Microorganism
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AnalyticsContext : DbContext
    {
        public AnalyticsContext(DbContextOptions<AnalyticsContext> options) : base(options)
        {
        }

        public DbSet<Institution> Institutions { get; set; } = null!;
        public DbSet<Analysis> Analyses { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Instrument> Instruments { get; set; } = null!;
        public DbSet<Keyword> Keywords { get; set; } = null!;
        public DbSet<Microorganism> Microorganisms { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entities to map to existing tables (read-only)
            modelBuilder.Entity<Institution>(entity =>
            {
                entity.ToTable("Institution");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Analysis>(entity =>
            {
                entity.ToTable("Analysis");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
            });

            modelBuilder.Entity<Instrument>(entity =>
            {
                entity.ToTable("Instrument");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Keyword>(entity =>
            {
                entity.ToTable("Keyword");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Microorganism>(entity =>
            {
                entity.ToTable("Microorganism");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}