using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DB.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DB.Infrastructure.Persistance
{
    public class SqlServerContextSeed
    {
        public static async Task SeedAsync(SqlServerContext context, ILogger<SqlServerContextSeed> logger)
        {
            if (!context.Institutions.Any() &&
                !context.Instruments.Any() &&
                !context.Microorganisms.Any() &&
                !context.Employees.Any() &&
                !context.Keywords.Any() &&
                !context.Analyses.Any()
                )
            {
                List<Institution> institutions = GetPreconfiguredInstitutions();
                List<Instrument> instruments = GetPreconfiguredInstruments();
                List<Microorganism> microorganisms = GetPreconfiguredMicroorganisms();
                List<Keyword> keywords = GetPreconfiguredKeywords();
                List<Employee> employees = GetPreconfiguredEmployees(institutions, keywords);
                List<Analysis> analyses = GetPreconfiguredAnalysis();
                // Povezivanje entiteta
                institutions[0].AddInstrument(instruments[0]); // Tech University ↔ Microscope
                institutions[0].AddInstrument(instruments[1]); // Tech University ↔ Spectrometer
                institutions[1].AddInstrument(instruments[2]); // Global Research Center ↔ PCR Machine
                institutions[2].AddInstrument(instruments[1]); // AI Institute ↔ Spectrometer

                institutions[0].AddMicroorgnaism(microorganisms[1]); // Tech University ↔ Cytomegalovirus
                institutions[0].AddMicroorgnaism(microorganisms[2]); // Tech University ↔ Chlamydia sp.
                institutions[1].AddMicroorgnaism(microorganisms[2]); // Global Research Center ↔ Chlamydia sp.
                institutions[2].AddMicroorgnaism(microorganisms[0]); // AI Institute ↔ SARS-CoV-2

                institutions[0].AddKeyword(keywords[1]); // Tech University ↔ Chlamydia sp.
                institutions[0].AddKeyword(keywords[2]); // Tech University ↔ Sequencing
                institutions[1].AddKeyword(keywords[2]); // Global Research Center ↔ Sequencing
                institutions[2].AddKeyword(keywords[0]); // AI Institute ↔ Cytomegalovirus

                institutions[0].AddEmployee(employees[1]); // Tech University ↔ Jane Smith
                institutions[0].AddEmployee(employees[2]); // Tech University ↔ Alice Johnson
                institutions[2].AddEmployee(employees[0]); // AI Institute ↔ John Doe

                institutions[0].AddAnalysis(analyses[1]); // Tech University ↔ Jane Smith
                institutions[0].AddAnalysis(analyses[2]); // Tech University ↔ Alice Johnson
                institutions[1].AddAnalysis(analyses[0]); // AI Institute ↔ John Doe
                institutions[2].AddAnalysis(analyses[0]); // AI Institute ↔ John Doe

                analyses[0].AddMicroorgnaism(microorganisms[1]);
                analyses[0].AddMicroorgnaism(microorganisms[2]);
                analyses[1].AddMicroorgnaism(microorganisms[0]);
                analyses[2].AddMicroorgnaism(microorganisms[0]);


                // Dodavanje u kontekst
                context.Institutions.AddRange(institutions);
                context.Instruments.AddRange(instruments);
                context.Microorganisms.AddRange(microorganisms);
                context.Keywords.AddRange(keywords);
                context.Employees.AddRange(employees);
                context.Analyses.AddRange(analyses);
               
              


                await context.SaveChangesAsync();
                logger.LogInformation("Seeding database associated with context {DbContextName}", nameof(SqlServerContext));
                logger.LogInformation("ANaliza {Analyses}", analyses);
            }
        }

       
        private static List<Keyword> GetPreconfiguredKeywords()
        {
            return new List<Keyword>
            {
                new Keyword("Cytomegalovirus"),
                new Keyword("Chlamydia sp."),
                new Keyword("Sequencing")
            };
        }

        private static List<Microorganism> GetPreconfiguredMicroorganisms()
        {
            return new List<Microorganism>
            {
                new Microorganism("SARS-CoV-2"),
                new Microorganism("Cytomegalovirus"),
                new Microorganism("Chlamydia sp.")
            };
        }

        private static List<Institution> GetPreconfiguredInstitutions()
        {
            var i1 = new Institution("Tech University", "Innovation Ave", "42", "New York", "USA", "+1 555 123 456", "contact@techuniversity.edu", "https://www.techuniversity.edu");
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

        private static List<Employee> GetPreconfiguredEmployees(List<Institution> institutions, List<Keyword> keywords)
        {
            var e1 = new Employee("John", "Doe", institutions[0]);
            e1.AddKeyword(keywords[0]); // Cytomegalovirus
            e1.AddKeyword(keywords[2]); // Sequencing

            var e2 = new Employee("Jane", "Smith", institutions[1]);
            e2.AddKeyword(keywords[1]); // Chlamydia sp.

            var e3 = new Employee("Alice", "Johnson", institutions[2]);
            e3.AddKeyword(keywords[2]); // Sequencing

            return new List<Employee> { e1, e2, e3 };
        }

        private static List<Analysis> GetPreconfiguredAnalysis()
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
