using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DB.Infrastructure.Persistance.EntityConfigurations
{
    public class DesignTimeInstitutionContextFactory : IDesignTimeDbContextFactory<SqlServerContext>
    {
       public SqlServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=EntityDb;User Id=sa;Password=MATF12345678rs2;TrustServerCertificate=True;Encrypt=False");
            return new SqlServerContext(optionsBuilder.Options);
        }
    }
}
