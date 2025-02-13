using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Entities;
using DB.Infrastructure.Persistance.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DB.Infrastructure.Persistance
{
    public class SqlServerContext : DbContext
    {
        public DbSet<Institution> Institutions { get; set; } = null!;

        public SqlServerContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InstituitionEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
