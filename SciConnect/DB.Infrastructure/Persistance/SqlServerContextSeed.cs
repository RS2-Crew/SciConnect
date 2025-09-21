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
                !context.Analyses.Any())
            {
                List<Institution> institutions = GetPreconfiguredInstitutions();
                List<Instrument> instruments = GetPreconfiguredInstruments();
                List<Microorganism> microorganisms = GetPreconfiguredMicroorganisms();
                List<Keyword> keywords = GetPreconfiguredKeywords();
                List<Employee> employees = GetPreconfiguredEmployees(institutions, keywords);
                List<Analysis> analyses = GetPreconfiguredAnalysis();

                institutions[0].AddInstrument(instruments[0]);
                institutions[0].AddInstrument(instruments[1]);
                institutions[0].AddInstrument(instruments[3]);
                institutions[0].AddInstrument(instruments[4]);
                institutions[1].AddInstrument(instruments[2]);
                institutions[1].AddInstrument(instruments[5]);
                institutions[1].AddInstrument(instruments[6]);
                institutions[2].AddInstrument(instruments[1]);
                institutions[2].AddInstrument(instruments[7]);
                institutions[2].AddInstrument(instruments[8]);

                institutions[0].AddMicroorgnaism(microorganisms[1]);
                institutions[0].AddMicroorgnaism(microorganisms[2]);
                institutions[0].AddMicroorgnaism(microorganisms[3]);
                institutions[0].AddMicroorgnaism(microorganisms[4]);
                institutions[1].AddMicroorgnaism(microorganisms[2]);
                institutions[1].AddMicroorgnaism(microorganisms[5]);
                institutions[1].AddMicroorgnaism(microorganisms[6]);
                institutions[2].AddMicroorgnaism(microorganisms[0]);
                institutions[2].AddMicroorgnaism(microorganisms[7]);
                institutions[2].AddMicroorgnaism(microorganisms[8]);

                institutions[0].AddKeyword(keywords[1]);
                institutions[0].AddKeyword(keywords[2]);
                institutions[1].AddKeyword(keywords[2]);
                institutions[2].AddKeyword(keywords[0]);
                
                // Ensure bidirectional relationships for institution-keyword
                keywords[1].AddInstitution(institutions[0]);
                keywords[2].AddInstitution(institutions[0]);
                keywords[2].AddInstitution(institutions[1]);
                keywords[0].AddInstitution(institutions[2]);

                institutions[0].AddEmployee(employees[0]);
                institutions[1].AddEmployee(employees[1]);
                institutions[2].AddEmployee(employees[2]);

                institutions[0].AddAnalysis(analyses[1]);
                institutions[0].AddAnalysis(analyses[2]);
                institutions[1].AddAnalysis(analyses[0]);
                institutions[2].AddAnalysis(analyses[0]);

                analyses[0].AddMicroorgnaism(microorganisms[1]);
                analyses[0].AddMicroorgnaism(microorganisms[2]);
                analyses[0].AddMicroorgnaism(microorganisms[3]);
                analyses[1].AddMicroorgnaism(microorganisms[0]);
                analyses[1].AddMicroorgnaism(microorganisms[4]);
                analyses[2].AddMicroorgnaism(microorganisms[0]);
                analyses[2].AddMicroorgnaism(microorganisms[5]);

                context.Institutions.AddRange(institutions);
                context.Instruments.AddRange(instruments);
                context.Microorganisms.AddRange(microorganisms);
                context.Keywords.AddRange(keywords);
                context.Employees.AddRange(employees);
                context.Analyses.AddRange(analyses);

                await context.SaveChangesAsync();
                logger.LogInformation("Seeding database associated with context {DbContextName}", nameof(SqlServerContext));
                logger.LogInformation("Analyses seeded: {AnalysesCount}", analyses.Count);
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
                new Microorganism("Chlamydia sp."),
                new Microorganism("Escherichia coli"),
                new Microorganism("Staphylococcus aureus"),
                new Microorganism("Streptococcus pneumoniae"),
                new Microorganism("Mycobacterium tuberculosis"),
                new Microorganism("Candida albicans"),
                new Microorganism("Aspergillus fumigatus"),
                new Microorganism("Pseudomonas aeruginosa"),
                new Microorganism("Klebsiella pneumoniae"),
                new Microorganism("Enterococcus faecalis"),
                new Microorganism("Clostridium difficile"),
                new Microorganism("Neisseria gonorrhoeae"),
                new Microorganism("Treponema pallidum")
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
                new Instrument("PCR Machine"),
                new Instrument("Flow Cytometer"),
                new Instrument("Mass Spectrometer"),
                new Instrument("Electron Microscope"),
                new Instrument("Centrifuge"),
                new Instrument("Incubator"),
                new Instrument("Autoclave"),
                new Instrument("Gel Electrophoresis System"),
                new Instrument("Real-Time PCR System"),
                new Instrument("Sequencing Machine"),
                new Instrument("Microplate Reader"),
                new Instrument("Liquid Chromatography System"),
                new Instrument("Gas Chromatography System"),
                new Instrument("Atomic Absorption Spectrometer"),
                new Instrument("Fluorescence Microscope"),
                new Instrument("Confocal Microscope"),
                new Instrument("Cell Counter"),
                new Instrument("Freeze Dryer")
            };
        }

        private static List<Employee> GetPreconfiguredEmployees(List<Institution> institutions, List<Keyword> keywords)
        {
            var e1 = new Employee("John", "Doe", institutions[0]);
            e1.AddKeyword(keywords[0]);
            e1.AddKeyword(keywords[2]);
            // Ensure bidirectional relationship
            keywords[0].AddEmployee(e1);
            keywords[2].AddEmployee(e1);

            var e2 = new Employee("Jane", "Smith", institutions[1]);
            e2.AddKeyword(keywords[1]);
            // Ensure bidirectional relationship
            keywords[1].AddEmployee(e2);

            var e3 = new Employee("Alice", "Johnson", institutions[2]);
            e3.AddKeyword(keywords[2]);
            // Ensure bidirectional relationship
            keywords[2].AddEmployee(e3);

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
