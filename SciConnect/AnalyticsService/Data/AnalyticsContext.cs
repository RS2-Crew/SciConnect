using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Data
{
    public class Institution
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Analysis> Analyses { get; set; } = new List<Analysis>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();
    }

    public class Analysis
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Institution> Institutions { get; set; } = new List<Institution>();
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Institution Institution { get; set; } = null!;
    }

    public class Instrument
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Institution> Institutions { get; set; } = new List<Institution>();
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

                entity.HasMany(a => a.Institutions)
                    .WithMany(i => i.Analyses)
                    .UsingEntity<Dictionary<string, object>>(
                        "InstitutionAnalyses",
                        j => j.HasOne<Institution>().WithMany().HasForeignKey("institution_id"),
                        j => j.HasOne<Analysis>().WithMany().HasForeignKey("analysis_id")
                    );
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();

                entity.HasOne(e => e.Institution)
                    .WithMany(i => i.Employees)
                    .HasForeignKey("institution_id");
            });

            modelBuilder.Entity<Instrument>(entity =>
            {
                entity.ToTable("Instrument");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();

                entity.HasMany(i => i.Institutions)
                    .WithMany(inst => inst.Instruments)
                    .UsingEntity<Dictionary<string, object>>(
                        "InstitutionInstrument",
                        j => j.HasOne<Institution>().WithMany().HasForeignKey("institution_id"),
                        j => j.HasOne<Instrument>().WithMany().HasForeignKey("instrument_id")
                    );
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