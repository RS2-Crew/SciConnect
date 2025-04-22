using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Common;
using DB.Domain.Entities;
using DB.Infrastructure.Persistance.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DB.Infrastructure.Persistance
{
    public class SqlServerContext : DbContext
    {
        public DbSet<Institution> Institutions { get; set; } = null!;
        public DbSet<Instrument> Instruments { get; set; } = null!;
        public DbSet<Analysis> Analysis { get; set; } = null!;

        public SqlServerContext(DbContextOptions options)
            : base(options)
        {

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = "rs2";
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = "rs2";
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InstitutionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InstrumentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AnalysisEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
