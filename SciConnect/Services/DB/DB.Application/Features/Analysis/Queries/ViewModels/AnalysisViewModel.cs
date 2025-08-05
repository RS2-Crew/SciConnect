using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Queries.ViewModels;

namespace DB.Application.Features.Analysis.Queries.ViewModels
{
    public class AnalysisViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<MicroorganismViewModel> Microorganisms { get; set; }
        public IEnumerable<InstitutionViewModel> Institutions { get; set; }


    }
}
