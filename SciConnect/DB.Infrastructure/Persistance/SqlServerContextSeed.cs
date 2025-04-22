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
        public static async Task SeedAsync(SqlServerContext context, ILogger<SqlServerContextSeed> logger)
        {
            if (!context.Institutions.Any() && !context.Instruments.Any())
            {
                List<Institution> institutions = GetPreconfiguredInstitutions();
                List<Instrument> instruments = GetPreconfiguredInstruments();

                institutions[0].AddInstrument(instruments[0]); // Tech University ↔ Microscope
                institutions[0].AddInstrument(instruments[1]); // Tech University ↔ Spectrometer
                institutions[1].AddInstrument(instruments[2]); // Global Research Center ↔ PCR Machine
                institutions[2].AddInstrument(instruments[1]); // AI Institute ↔ Spectrometer

                context.Institutions.AddRange(institutions);
                context.Instruments.AddRange(instruments);

                await context.SaveChangesAsync();
                logger.LogInformation("Seeding database associated with context {DbContextName}", nameof(SqlServerContext));
            }
        }

        private static List<Institution> GetPreconfiguredInstitutions()
        {
            var i1 = new Institution("Tech University", "Innovation Ave", "42", "New York", "USA", "+1 555 123 456", "contact@techuniversity.edu", "https://www.techuniversity.edu");

            ;
            var i2 = new Institution("Global Research Center", "Science Blvd", "101", "London", "UK", "+44 20 7946 0958", "info@grc.uk", "https://www.grc.uk");
            var i3 = new Institution("AI Institute", "Neural St", "77", "Berlin", "Germany", "+49 30 1234 5678", "ai@ai-institute.de", "https://www.ai-institute.de");


            return new List<Institution> { i1, i2, i3 };
        }

        private static List<Instrument> GetPreconfiguredInstruments()
        {
            return new List<Instrument>
        {
            new Instrument("Microscope"),
            new Instrument("Spectrometer"),
            new Instrument("PCR Machine")
        };
        }

        private static List<Analysis> GetPreconfiguredAnalyses()
        {
            return new List<Analysis>
        {
            new Analysis("PCR"),
            new Analysis("Real-Time PCR"),
            new Analysis("Serologija")
        };
        }

    }
}
