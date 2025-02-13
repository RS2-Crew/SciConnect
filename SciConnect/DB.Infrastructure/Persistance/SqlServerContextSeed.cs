using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DB.Infrastructure.Persistance
{
    public class SqlServerContextSeed
    {
        public static async Task SeedAsync(SqlServerContext orderContext, ILogger<SqlServerContextSeed> logger)
        {
            if (!orderContext.Institutions.Any())
            {
                orderContext.Institutions.AddRange(GetPreconfiguredInstitutions());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seeding database associated with context {DbContextName}", nameof(SqlServerContext));
            }
        }

        private static IEnumerable<Institution> GetPreconfiguredInstitutions()
        {
            var i1 = new Institution("Tech University", "Innovation Ave", "42", "New York", "USA", "+1 555 123 456", "contact@techuniversity.edu", "https://www.techuniversity.edu");

            ;
            var i2 = new Institution("Global Research Center", "Science Blvd", "101", "London", "UK", "+44 20 7946 0958", "info@grc.uk", "https://www.grc.uk");
            var i3 = new Institution("AI Institute", "Neural St", "77", "Berlin", "Germany", "+49 30 1234 5678", "ai@ai-institute.de", "https://www.ai-institute.de");


            return new List<Institution> { i1, i2, i3 };
        }
    }
}
