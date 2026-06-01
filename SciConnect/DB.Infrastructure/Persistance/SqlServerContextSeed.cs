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
                var institutions = GetPreconfiguredInstitutions();
                var instruments = GetPreconfiguredInstruments();
                var microorganisms = GetPreconfiguredMicroorganisms();
                var keywords = GetPreconfiguredKeywords();
                var employees = GetPreconfiguredEmployees(institutions, keywords);
                var analyses = GetPreconfiguredAnalyses();

                LinkInstitutionsAndInstruments(institutions, instruments);
                LinkInstitutionsAndMicroorganisms(institutions, microorganisms);
                LinkInstitutionsAndKeywords(institutions, keywords);
                LinkInstitutionsAndAnalyses(institutions, analyses);
                LinkAnalysesAndMicroorganisms(analyses, microorganisms);

                context.Institutions.AddRange(institutions);
                context.Instruments.AddRange(instruments);
                context.Microorganisms.AddRange(microorganisms);
                context.Keywords.AddRange(keywords);
                context.Employees.AddRange(employees);
                context.Analyses.AddRange(analyses);

                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Institutions} institutions, {Employees} employees, {Microorganisms} microorganisms, {Instruments} instruments, {Analyses} analyses, {Keywords} keywords",
                    institutions.Count, employees.Count, microorganisms.Count, instruments.Count, analyses.Count, keywords.Count);
            }
        }

        private static List<Institution> GetPreconfiguredInstitutions()
        {
            return new List<Institution>
            {
                new Institution("Institut za molekularnu genetiku i geneticko inzenjerstvo", "Vojvode Stepe", "444a", "Beograd", "Srbija",
                    "+381 11 397 6212", "office@imgge.bg.ac.rs", "https://www.imgge.bg.ac.rs"),
                new Institution("Institut Pasteur", "Rue du Docteur Roux", "25", "Pariz", "Francuska",
                    "+33 1 45 68 80 00", "contact@pasteur.fr", "https://www.pasteur.fr"),
                new Institution("Max Planck Institut za infektivnu biologiju", "Chariteplatz", "1", "Berlin", "Nemacka",
                    "+49 30 28460 10", "info@mpiib-berlin.mpg.de", "https://www.mpiib-berlin.mpg.de"),
                new Institution("Karolinska Institutet", "Solnavagen", "1", "Stokholm", "Svedska",
                    "+46 8 524 800 00", "info@ki.se", "https://www.ki.se"),
                new Institution("Institut za virusologiju, vakcine i serume Torlak", "Vojvode Stepe", "458", "Beograd", "Srbija",
                    "+381 11 247 0222", "info@torlak.rs", "https://www.torlak.rs"),
                new Institution("European Molecular Biology Laboratory", "Meyerhofstrasse", "1", "Hajdelberg", "Nemacka",
                    "+49 6221 387 0", "info@embl.org", "https://www.embl.org"),
                new Institution("Univerzitet u Beogradu - Bioloski fakultet", "Studentski trg", "16", "Beograd", "Srbija",
                    "+381 11 263 8500", "dekanat@bio.bg.ac.rs", "https://www.bio.bg.ac.rs"),
                new Institution("Swiss Federal Institute of Technology (ETH Zurich)", "Ramistrasse", "101", "Cirih", "Svajcarska",
                    "+41 44 632 11 11", "info@ethz.ch", "https://www.ethz.ch"),
                new Institution("Medicinski fakultet Univerziteta u Novom Sadu", "Hajduk Veljkova", "3", "Novi Sad", "Srbija",
                    "+381 21 420 677", "dekanat@mf.uns.ac.rs", "https://www.mf.uns.ac.rs"),
                new Institution("Institut za javno zdravlje Srbije Dr Milan Jovanovic Batut", "Dr Subotica", "5", "Beograd", "Srbija",
                    "+381 11 268 4566", "info@batut.org.rs", "https://www.batut.org.rs"),
                new Institution("Cambridge University Department of Pathology", "Tennis Court Road", "1", "Kembridz", "Velika Britanija",
                    "+44 1223 333 333", "path@cam.ac.uk", "https://www.path.cam.ac.uk"),
                new Institution("National Institute of Allergy and Infectious Diseases", "Center Drive", "9000", "Bethesda", "SAD",
                    "+1 301 496 5717", "ocpostoffice@niaid.nih.gov", "https://www.niaid.nih.gov"),
            };
        }

        private static List<Instrument> GetPreconfiguredInstruments()
        {
            return new List<Instrument>
            {
                new Instrument("Elektronski mikroskop (TEM)"),
                new Instrument("Konfokalni mikroskop"),
                new Instrument("PCR aparat (Thermocycler)"),
                new Instrument("Real-Time PCR aparat"),
                new Instrument("Spektrofotometar (UV-Vis)"),
                new Instrument("Protočni citometar (FACS)"),
                new Instrument("Maseni spektrometar"),
                new Instrument("Gel elektroforeza sistem"),
                new Instrument("Centrifuga (ultracentrifuga)"),
                new Instrument("ELISA čitač"),
                new Instrument("DNA sekvencer (NGS)"),
                new Instrument("Inkubator sa CO2"),
                new Instrument("Laminarni kabinet (Biosafety)"),
                new Instrument("Autoklav"),
                new Instrument("Western blot aparat"),
            };
        }

        private static List<Microorganism> GetPreconfiguredMicroorganisms()
        {
            return new List<Microorganism>
            {
                new Microorganism("SARS-CoV-2"),
                new Microorganism("Mycobacterium tuberculosis"),
                new Microorganism("Escherichia coli"),
                new Microorganism("Staphylococcus aureus (MRSA)"),
                new Microorganism("Streptococcus pneumoniae"),
                new Microorganism("Candida albicans"),
                new Microorganism("Aspergillus fumigatus"),
                new Microorganism("Plasmodium falciparum"),
                new Microorganism("Salmonella enterica"),
                new Microorganism("Helicobacter pylori"),
                new Microorganism("Klebsiella pneumoniae"),
                new Microorganism("Pseudomonas aeruginosa"),
                new Microorganism("Clostridium difficile"),
                new Microorganism("Neisseria meningitidis"),
                new Microorganism("Cytomegalovirus (CMV)"),
                new Microorganism("Chlamydia trachomatis"),
                new Microorganism("Hepatitis B virus"),
                new Microorganism("Influenza A virus (H1N1)"),
                new Microorganism("Borrelia burgdorferi"),
                new Microorganism("Legionella pneumophila"),
            };
        }

        private static List<Keyword> GetPreconfiguredKeywords()
        {
            return new List<Keyword>
            {
                new Keyword("Genomika"),
                new Keyword("Proteomika"),
                new Keyword("Sekvenciranje"),
                new Keyword("Antimikrobna rezistencija"),
                new Keyword("Epidemiologija"),
                new Keyword("Vakcinologija"),
                new Keyword("Molekularna dijagnostika"),
                new Keyword("Bioinformatika"),
                new Keyword("Virusologija"),
                new Keyword("Bakteriologija"),
                new Keyword("Mikologija"),
                new Keyword("Imunologija"),
                new Keyword("Ćelijske kulture"),
                new Keyword("Klinička mikrobiologija"),
                new Keyword("PCR dijagnostika"),
                new Keyword("Metagenomika"),
                new Keyword("Transkriptomika"),
                new Keyword("CRISPR"),
            };
        }

        private static List<Employee> GetPreconfiguredEmployees(List<Institution> institutions, List<Keyword> keywords)
        {
            // IMGGE Beograd
            var e1 = new Employee("Jelena", "Milašin", institutions[0]);
            e1.AddKeyword(keywords[0]); // Genomika
            e1.AddKeyword(keywords[2]); // Sekvenciranje
            e1.AddKeyword(keywords[7]); // Bioinformatika

            var e2 = new Employee("Maja", "Stojiljković", institutions[0]);
            e2.AddKeyword(keywords[0]); // Genomika
            e2.AddKeyword(keywords[6]); // Molekularna dijagnostika

            // Institut Pasteur
            var e3 = new Employee("Philippe", "Sansonetti", institutions[1]);
            e3.AddKeyword(keywords[9]); // Bakteriologija
            e3.AddKeyword(keywords[11]); // Imunologija

            var e4 = new Employee("Marie", "Durand", institutions[1]);
            e4.AddKeyword(keywords[5]); // Vakcinologija
            e4.AddKeyword(keywords[4]); // Epidemiologija

            var e5 = new Employee("Luc", "Bernard", institutions[1]);
            e5.AddKeyword(keywords[8]); // Virusologija

            // Max Planck Berlin
            var e6 = new Employee("Thomas", "Meyer", institutions[2]);
            e6.AddKeyword(keywords[9]); // Bakteriologija
            e6.AddKeyword(keywords[1]); // Proteomika

            var e7 = new Employee("Anna", "Schneider", institutions[2]);
            e7.AddKeyword(keywords[3]); // Antimikrobna rezistencija
            e7.AddKeyword(keywords[7]); // Bioinformatika

            // Karolinska
            var e8 = new Employee("Erik", "Lindström", institutions[3]);
            e8.AddKeyword(keywords[11]); // Imunologija
            e8.AddKeyword(keywords[5]); // Vakcinologija

            var e9 = new Employee("Astrid", "Bergman", institutions[3]);
            e9.AddKeyword(keywords[4]); // Epidemiologija
            e9.AddKeyword(keywords[13]); // Klinička mikrobiologija

            // Torlak
            var e10 = new Employee("Branko", "Nikolić", institutions[4]);
            e10.AddKeyword(keywords[5]); // Vakcinologija
            e10.AddKeyword(keywords[8]); // Virusologija

            var e11 = new Employee("Dragana", "Petrović", institutions[4]);
            e11.AddKeyword(keywords[14]); // PCR dijagnostika
            e11.AddKeyword(keywords[6]); // Molekularna dijagnostika

            // EMBL
            var e12 = new Employee("Jan", "Ellenberg", institutions[5]);
            e12.AddKeyword(keywords[0]); // Genomika
            e12.AddKeyword(keywords[15]); // Metagenomika
            e12.AddKeyword(keywords[7]); // Bioinformatika

            var e13 = new Employee("Sofia", "Müller", institutions[5]);
            e13.AddKeyword(keywords[1]); // Proteomika
            e13.AddKeyword(keywords[16]); // Transkriptomika

            // Biološki fakultet
            var e14 = new Employee("Ivana", "Đorđević", institutions[6]);
            e14.AddKeyword(keywords[10]); // Mikologija
            e14.AddKeyword(keywords[12]); // Ćelijske kulture

            var e15 = new Employee("Milan", "Obradović", institutions[6]);
            e15.AddKeyword(keywords[9]); // Bakteriologija
            e15.AddKeyword(keywords[3]); // Antimikrobna rezistencija

            // ETH Zurich
            var e16 = new Employee("Martin", "Ackermann", institutions[7]);
            e16.AddKeyword(keywords[4]); // Epidemiologija
            e16.AddKeyword(keywords[15]); // Metagenomika

            var e17 = new Employee("Laura", "Fischer", institutions[7]);
            e17.AddKeyword(keywords[17]); // CRISPR
            e17.AddKeyword(keywords[0]); // Genomika

            // Medicinski fakultet Novi Sad
            var e18 = new Employee("Siniša", "Sević", institutions[8]);
            e18.AddKeyword(keywords[13]); // Klinička mikrobiologija
            e18.AddKeyword(keywords[14]); // PCR dijagnostika

            var e19 = new Employee("Nataša", "Vuković", institutions[8]);
            e19.AddKeyword(keywords[4]); // Epidemiologija
            e19.AddKeyword(keywords[6]); // Molekularna dijagnostika

            // Batut
            var e20 = new Employee("Darija", "Kisić Tepavčević", institutions[9]);
            e20.AddKeyword(keywords[4]); // Epidemiologija

            var e21 = new Employee("Predrag", "Kon", institutions[9]);
            e21.AddKeyword(keywords[4]); // Epidemiologija
            e21.AddKeyword(keywords[8]); // Virusologija

            // Cambridge
            var e22 = new Employee("James", "Richardson", institutions[10]);
            e22.AddKeyword(keywords[8]); // Virusologija
            e22.AddKeyword(keywords[11]); // Imunologija

            var e23 = new Employee("Emily", "Turner", institutions[10]);
            e23.AddKeyword(keywords[3]); // Antimikrobna rezistencija
            e23.AddKeyword(keywords[2]); // Sekvenciranje

            // NIAID
            var e24 = new Employee("Anthony", "Martinez", institutions[11]);
            e24.AddKeyword(keywords[8]); // Virusologija
            e24.AddKeyword(keywords[5]); // Vakcinologija

            var e25 = new Employee("Sarah", "Chen", institutions[11]);
            e25.AddKeyword(keywords[11]); // Imunologija
            e25.AddKeyword(keywords[17]); // CRISPR

            return new List<Employee> { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16, e17, e18, e19, e20, e21, e22, e23, e24, e25 };
        }

        private static List<Analysis> GetPreconfiguredAnalyses()
        {
            return new List<Analysis>
            {
                new Analysis("PCR (Polymerase Chain Reaction)"),
                new Analysis("Real-Time PCR (qPCR)"),
                new Analysis("Serologija (ELISA)"),
                new Analysis("Western blot"),
                new Analysis("Gel elektroforeza"),
                new Analysis("Sekvenciranje genoma (NGS)"),
                new Analysis("Masena spektrometrija"),
                new Analysis("Protočna citometrija"),
                new Analysis("Antibiogram"),
                new Analysis("Mikroskopija (svetlosna)"),
                new Analysis("Elektronska mikroskopija"),
                new Analysis("Kultivacija na hranljivim podlogama"),
                new Analysis("CRISPR analiza"),
                new Analysis("Imunofluorescencija"),
                new Analysis("Metagenomska analiza"),
            };
        }

        private static void LinkInstitutionsAndInstruments(List<Institution> inst, List<Instrument> instr)
        {
            // IMGGE - molecular biology focused
            inst[0].AddInstrument(instr[2]);  // PCR
            inst[0].AddInstrument(instr[3]);  // Real-Time PCR
            inst[0].AddInstrument(instr[7]);  // Gel elektroforeza
            inst[0].AddInstrument(instr[10]); // NGS sekvencer
            inst[0].AddInstrument(instr[12]); // Laminarni kabinet

            // Pasteur - full range
            inst[1].AddInstrument(instr[0]);  // TEM
            inst[1].AddInstrument(instr[1]);  // Konfokalni
            inst[1].AddInstrument(instr[3]);  // Real-Time PCR
            inst[1].AddInstrument(instr[5]);  // FACS
            inst[1].AddInstrument(instr[6]);  // Maseni spektrometar
            inst[1].AddInstrument(instr[10]); // NGS
            inst[1].AddInstrument(instr[13]); // Autoklav

            // Max Planck
            inst[2].AddInstrument(instr[1]);  // Konfokalni
            inst[2].AddInstrument(instr[5]);  // FACS
            inst[2].AddInstrument(instr[6]);  // Maseni spektrometar
            inst[2].AddInstrument(instr[14]); // Western blot

            // Karolinska
            inst[3].AddInstrument(instr[3]);  // Real-Time PCR
            inst[3].AddInstrument(instr[5]);  // FACS
            inst[3].AddInstrument(instr[9]);  // ELISA čitač
            inst[3].AddInstrument(instr[11]); // CO2 inkubator
            inst[3].AddInstrument(instr[14]); // Western blot

            // Torlak
            inst[4].AddInstrument(instr[2]);  // PCR
            inst[4].AddInstrument(instr[3]);  // Real-Time PCR
            inst[4].AddInstrument(instr[9]);  // ELISA čitač
            inst[4].AddInstrument(instr[11]); // CO2 inkubator
            inst[4].AddInstrument(instr[12]); // Laminarni kabinet
            inst[4].AddInstrument(instr[13]); // Autoklav

            // EMBL
            inst[5].AddInstrument(instr[0]);  // TEM
            inst[5].AddInstrument(instr[1]);  // Konfokalni
            inst[5].AddInstrument(instr[6]);  // Maseni spektrometar
            inst[5].AddInstrument(instr[10]); // NGS
            inst[5].AddInstrument(instr[8]);  // Ultracentrifuga

            // Biološki fakultet
            inst[6].AddInstrument(instr[2]);  // PCR
            inst[6].AddInstrument(instr[4]);  // Spektrofotometar
            inst[6].AddInstrument(instr[7]);  // Gel elektroforeza
            inst[6].AddInstrument(instr[11]); // CO2 inkubator
            inst[6].AddInstrument(instr[13]); // Autoklav

            // ETH Zurich
            inst[7].AddInstrument(instr[0]);  // TEM
            inst[7].AddInstrument(instr[3]);  // Real-Time PCR
            inst[7].AddInstrument(instr[5]);  // FACS
            inst[7].AddInstrument(instr[6]);  // Maseni spektrometar
            inst[7].AddInstrument(instr[10]); // NGS

            // Med. fakultet Novi Sad
            inst[8].AddInstrument(instr[2]);  // PCR
            inst[8].AddInstrument(instr[3]);  // Real-Time PCR
            inst[8].AddInstrument(instr[4]);  // Spektrofotometar
            inst[8].AddInstrument(instr[9]);  // ELISA čitač

            // Batut
            inst[9].AddInstrument(instr[2]);  // PCR
            inst[9].AddInstrument(instr[3]);  // Real-Time PCR
            inst[9].AddInstrument(instr[9]);  // ELISA čitač
            inst[9].AddInstrument(instr[13]); // Autoklav

            // Cambridge
            inst[10].AddInstrument(instr[0]); // TEM
            inst[10].AddInstrument(instr[1]); // Konfokalni
            inst[10].AddInstrument(instr[5]); // FACS
            inst[10].AddInstrument(instr[10]);// NGS
            inst[10].AddInstrument(instr[14]);// Western blot

            // NIAID
            inst[11].AddInstrument(instr[0]); // TEM
            inst[11].AddInstrument(instr[3]); // Real-Time PCR
            inst[11].AddInstrument(instr[5]); // FACS
            inst[11].AddInstrument(instr[6]); // Maseni spektrometar
            inst[11].AddInstrument(instr[10]);// NGS
            inst[11].AddInstrument(instr[12]);// Laminarni kabinet
        }

        private static void LinkInstitutionsAndMicroorganisms(List<Institution> inst, List<Microorganism> mo)
        {
            // IMGGE
            inst[0].AddMicroorganism(mo[2]);  // E. coli
            inst[0].AddMicroorganism(mo[3]);  // MRSA
            inst[0].AddMicroorganism(mo[15]); // Chlamydia

            // Pasteur
            inst[1].AddMicroorganism(mo[0]);  // SARS-CoV-2
            inst[1].AddMicroorganism(mo[1]);  // M. tuberculosis
            inst[1].AddMicroorganism(mo[7]);  // Plasmodium
            inst[1].AddMicroorganism(mo[17]); // Influenza A

            // Max Planck
            inst[2].AddMicroorganism(mo[2]);  // E. coli
            inst[2].AddMicroorganism(mo[9]);  // H. pylori
            inst[2].AddMicroorganism(mo[11]); // P. aeruginosa

            // Karolinska
            inst[3].AddMicroorganism(mo[0]);  // SARS-CoV-2
            inst[3].AddMicroorganism(mo[4]);  // S. pneumoniae
            inst[3].AddMicroorganism(mo[13]); // N. meningitidis
            inst[3].AddMicroorganism(mo[17]); // Influenza A

            // Torlak
            inst[4].AddMicroorganism(mo[0]);  // SARS-CoV-2
            inst[4].AddMicroorganism(mo[14]); // CMV
            inst[4].AddMicroorganism(mo[16]); // Hepatitis B
            inst[4].AddMicroorganism(mo[17]); // Influenza A

            // EMBL
            inst[5].AddMicroorganism(mo[2]);  // E. coli
            inst[5].AddMicroorganism(mo[8]);  // Salmonella
            inst[5].AddMicroorganism(mo[10]); // Klebsiella

            // Biološki fakultet
            inst[6].AddMicroorganism(mo[5]);  // Candida
            inst[6].AddMicroorganism(mo[6]);  // Aspergillus
            inst[6].AddMicroorganism(mo[2]);  // E. coli

            // ETH Zurich
            inst[7].AddMicroorganism(mo[1]);  // M. tuberculosis
            inst[7].AddMicroorganism(mo[3]);  // MRSA
            inst[7].AddMicroorganism(mo[11]); // P. aeruginosa
            inst[7].AddMicroorganism(mo[19]); // Legionella

            // Med. fakultet Novi Sad
            inst[8].AddMicroorganism(mo[3]);  // MRSA
            inst[8].AddMicroorganism(mo[10]); // Klebsiella
            inst[8].AddMicroorganism(mo[12]); // C. difficile

            // Batut
            inst[9].AddMicroorganism(mo[0]);  // SARS-CoV-2
            inst[9].AddMicroorganism(mo[8]);  // Salmonella
            inst[9].AddMicroorganism(mo[17]); // Influenza A
            inst[9].AddMicroorganism(mo[18]); // Borrelia

            // Cambridge
            inst[10].AddMicroorganism(mo[1]); // M. tuberculosis
            inst[10].AddMicroorganism(mo[7]); // Plasmodium
            inst[10].AddMicroorganism(mo[14]);// CMV
            inst[10].AddMicroorganism(mo[16]);// Hepatitis B

            // NIAID
            inst[11].AddMicroorganism(mo[0]); // SARS-CoV-2
            inst[11].AddMicroorganism(mo[1]); // M. tuberculosis
            inst[11].AddMicroorganism(mo[7]); // Plasmodium
            inst[11].AddMicroorganism(mo[16]);// Hepatitis B
            inst[11].AddMicroorganism(mo[17]);// Influenza A
        }

        private static void LinkInstitutionsAndKeywords(List<Institution> inst, List<Keyword> kw)
        {
            inst[0].AddKeyword(kw[0]);  // Genomika
            inst[0].AddKeyword(kw[2]);  // Sekvenciranje
            inst[0].AddKeyword(kw[7]);  // Bioinformatika

            inst[1].AddKeyword(kw[5]);  // Vakcinologija
            inst[1].AddKeyword(kw[8]);  // Virusologija
            inst[1].AddKeyword(kw[11]); // Imunologija
            inst[1].AddKeyword(kw[4]);  // Epidemiologija

            inst[2].AddKeyword(kw[1]);  // Proteomika
            inst[2].AddKeyword(kw[3]);  // Antimikrobna rezistencija
            inst[2].AddKeyword(kw[9]);  // Bakteriologija

            inst[3].AddKeyword(kw[5]);  // Vakcinologija
            inst[3].AddKeyword(kw[11]); // Imunologija
            inst[3].AddKeyword(kw[13]); // Klinička mikrobiologija

            inst[4].AddKeyword(kw[5]);  // Vakcinologija
            inst[4].AddKeyword(kw[8]);  // Virusologija
            inst[4].AddKeyword(kw[14]); // PCR dijagnostika
            inst[4].AddKeyword(kw[6]);  // Molekularna dijagnostika

            inst[5].AddKeyword(kw[0]);  // Genomika
            inst[5].AddKeyword(kw[1]);  // Proteomika
            inst[5].AddKeyword(kw[7]);  // Bioinformatika
            inst[5].AddKeyword(kw[15]); // Metagenomika

            inst[6].AddKeyword(kw[9]);  // Bakteriologija
            inst[6].AddKeyword(kw[10]); // Mikologija
            inst[6].AddKeyword(kw[12]); // Ćelijske kulture

            inst[7].AddKeyword(kw[0]);  // Genomika
            inst[7].AddKeyword(kw[15]); // Metagenomika
            inst[7].AddKeyword(kw[17]); // CRISPR
            inst[7].AddKeyword(kw[3]);  // Antimikrobna rezistencija

            inst[8].AddKeyword(kw[6]);  // Molekularna dijagnostika
            inst[8].AddKeyword(kw[13]); // Klinička mikrobiologija
            inst[8].AddKeyword(kw[14]); // PCR dijagnostika

            inst[9].AddKeyword(kw[4]);  // Epidemiologija
            inst[9].AddKeyword(kw[8]);  // Virusologija
            inst[9].AddKeyword(kw[14]); // PCR dijagnostika

            inst[10].AddKeyword(kw[8]); // Virusologija
            inst[10].AddKeyword(kw[2]); // Sekvenciranje
            inst[10].AddKeyword(kw[3]); // Antimikrobna rezistencija
            inst[10].AddKeyword(kw[11]);// Imunologija

            inst[11].AddKeyword(kw[5]); // Vakcinologija
            inst[11].AddKeyword(kw[8]); // Virusologija
            inst[11].AddKeyword(kw[11]);// Imunologija
            inst[11].AddKeyword(kw[17]);// CRISPR
        }

        private static void LinkInstitutionsAndAnalyses(List<Institution> inst, List<Analysis> an)
        {
            inst[0].AddAnalysis(an[0]);  // PCR
            inst[0].AddAnalysis(an[1]);  // qPCR
            inst[0].AddAnalysis(an[4]);  // Gel elektroforeza
            inst[0].AddAnalysis(an[5]);  // NGS

            inst[1].AddAnalysis(an[0]);  // PCR
            inst[1].AddAnalysis(an[1]);  // qPCR
            inst[1].AddAnalysis(an[2]);  // ELISA
            inst[1].AddAnalysis(an[7]);  // Protočna citometrija
            inst[1].AddAnalysis(an[10]); // Elektronska mikroskopija
            inst[1].AddAnalysis(an[14]); // Metagenomska analiza

            inst[2].AddAnalysis(an[3]);  // Western blot
            inst[2].AddAnalysis(an[6]);  // Masena spektrometrija
            inst[2].AddAnalysis(an[7]);  // Protočna citometrija

            inst[3].AddAnalysis(an[1]);  // qPCR
            inst[3].AddAnalysis(an[2]);  // ELISA
            inst[3].AddAnalysis(an[3]);  // Western blot
            inst[3].AddAnalysis(an[7]);  // Protočna citometrija
            inst[3].AddAnalysis(an[13]); // Imunofluorescencija

            inst[4].AddAnalysis(an[0]);  // PCR
            inst[4].AddAnalysis(an[1]);  // qPCR
            inst[4].AddAnalysis(an[2]);  // ELISA
            inst[4].AddAnalysis(an[8]);  // Antibiogram
            inst[4].AddAnalysis(an[11]); // Kultivacija

            inst[5].AddAnalysis(an[5]);  // NGS
            inst[5].AddAnalysis(an[6]);  // Masena spektrometrija
            inst[5].AddAnalysis(an[10]); // Elektronska mikroskopija
            inst[5].AddAnalysis(an[14]); // Metagenomska analiza

            inst[6].AddAnalysis(an[0]);  // PCR
            inst[6].AddAnalysis(an[4]);  // Gel elektroforeza
            inst[6].AddAnalysis(an[9]);  // Mikroskopija
            inst[6].AddAnalysis(an[11]); // Kultivacija

            inst[7].AddAnalysis(an[1]);  // qPCR
            inst[7].AddAnalysis(an[5]);  // NGS
            inst[7].AddAnalysis(an[6]);  // Masena spektrometrija
            inst[7].AddAnalysis(an[12]); // CRISPR
            inst[7].AddAnalysis(an[14]); // Metagenomska analiza

            inst[8].AddAnalysis(an[0]);  // PCR
            inst[8].AddAnalysis(an[1]);  // qPCR
            inst[8].AddAnalysis(an[8]);  // Antibiogram
            inst[8].AddAnalysis(an[11]); // Kultivacija

            inst[9].AddAnalysis(an[0]);  // PCR
            inst[9].AddAnalysis(an[1]);  // qPCR
            inst[9].AddAnalysis(an[2]);  // ELISA
            inst[9].AddAnalysis(an[8]);  // Antibiogram

            inst[10].AddAnalysis(an[1]); // qPCR
            inst[10].AddAnalysis(an[3]); // Western blot
            inst[10].AddAnalysis(an[5]); // NGS
            inst[10].AddAnalysis(an[7]); // Protočna citometrija
            inst[10].AddAnalysis(an[13]);// Imunofluorescencija

            inst[11].AddAnalysis(an[1]); // qPCR
            inst[11].AddAnalysis(an[2]); // ELISA
            inst[11].AddAnalysis(an[5]); // NGS
            inst[11].AddAnalysis(an[7]); // Protočna citometrija
            inst[11].AddAnalysis(an[10]);// Elektronska mikroskopija
            inst[11].AddAnalysis(an[12]);// CRISPR
        }

        private static void LinkAnalysesAndMicroorganisms(List<Analysis> an, List<Microorganism> mo)
        {
            // PCR
            an[0].AddMicroorganism(mo[0]);  // SARS-CoV-2
            an[0].AddMicroorganism(mo[1]);  // M. tuberculosis
            an[0].AddMicroorganism(mo[15]); // Chlamydia

            // qPCR
            an[1].AddMicroorganism(mo[0]);  // SARS-CoV-2
            an[1].AddMicroorganism(mo[14]); // CMV
            an[1].AddMicroorganism(mo[17]); // Influenza A
            an[1].AddMicroorganism(mo[16]); // Hepatitis B

            // ELISA
            an[2].AddMicroorganism(mo[0]);  // SARS-CoV-2
            an[2].AddMicroorganism(mo[16]); // Hepatitis B
            an[2].AddMicroorganism(mo[18]); // Borrelia

            // Western blot
            an[3].AddMicroorganism(mo[16]); // Hepatitis B
            an[3].AddMicroorganism(mo[18]); // Borrelia

            // Gel elektroforeza
            an[4].AddMicroorganism(mo[2]);  // E. coli
            an[4].AddMicroorganism(mo[3]);  // MRSA

            // NGS
            an[5].AddMicroorganism(mo[0]);  // SARS-CoV-2
            an[5].AddMicroorganism(mo[1]);  // M. tuberculosis
            an[5].AddMicroorganism(mo[2]);  // E. coli
            an[5].AddMicroorganism(mo[11]); // P. aeruginosa

            // Masena spektrometrija
            an[6].AddMicroorganism(mo[2]);  // E. coli
            an[6].AddMicroorganism(mo[3]);  // MRSA
            an[6].AddMicroorganism(mo[10]); // Klebsiella

            // Protočna citometrija
            an[7].AddMicroorganism(mo[7]);  // Plasmodium
            an[7].AddMicroorganism(mo[5]);  // Candida

            // Antibiogram
            an[8].AddMicroorganism(mo[3]);  // MRSA
            an[8].AddMicroorganism(mo[4]);  // S. pneumoniae
            an[8].AddMicroorganism(mo[10]); // Klebsiella
            an[8].AddMicroorganism(mo[11]); // P. aeruginosa
            an[8].AddMicroorganism(mo[12]); // C. difficile

            // Mikroskopija
            an[9].AddMicroorganism(mo[5]);  // Candida
            an[9].AddMicroorganism(mo[6]);  // Aspergillus
            an[9].AddMicroorganism(mo[7]);  // Plasmodium

            // Elektronska mikroskopija
            an[10].AddMicroorganism(mo[0]); // SARS-CoV-2
            an[10].AddMicroorganism(mo[17]);// Influenza A

            // Kultivacija
            an[11].AddMicroorganism(mo[2]); // E. coli
            an[11].AddMicroorganism(mo[3]); // MRSA
            an[11].AddMicroorganism(mo[5]); // Candida
            an[11].AddMicroorganism(mo[8]); // Salmonella
            an[11].AddMicroorganism(mo[9]); // H. pylori

            // CRISPR
            an[12].AddMicroorganism(mo[2]); // E. coli
            an[12].AddMicroorganism(mo[1]); // M. tuberculosis

            // Imunofluorescencija
            an[13].AddMicroorganism(mo[14]);// CMV
            an[13].AddMicroorganism(mo[15]);// Chlamydia
            an[13].AddMicroorganism(mo[13]);// N. meningitidis

            // Metagenomska analiza
            an[14].AddMicroorganism(mo[8]); // Salmonella
            an[14].AddMicroorganism(mo[10]);// Klebsiella
            an[14].AddMicroorganism(mo[19]);// Legionella
        }
    }
}
