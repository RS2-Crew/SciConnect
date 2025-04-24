using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Analysis.Queries.ViewModels;
using DB.Application.Features.Employees.Queries.ViewModels;
using DB.Application.Features.Instruments.Queries.ViewModels;
using DB.Application.Features.Keywords.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Queries.ViewModels;

namespace DB.Application.Features.Institutions.Queries.ViewModels
{
    public class InstitutionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }

        public IEnumerable<InstrumentViewModel> Instruments { get; set; }

        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        public IEnumerable<AnalysisViewModel> Analyses { get; set; }
        public IEnumerable<KeywordViewModel> Keywords { get; set; }
        public IEnumerable<MicroorganismViewModel> Microorganisms { get; set; }




    }
}
