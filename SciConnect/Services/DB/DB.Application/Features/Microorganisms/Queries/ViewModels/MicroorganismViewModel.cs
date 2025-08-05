using DB.Application.Features.Institutions.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Application.Features.Analysis.Queries.ViewModels;

namespace DB.Application.Features.Microorganisms.Queries.ViewModels
{
    public class MicroorganismViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public MicroorganismViewModel() { }

        public MicroorganismViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public IEnumerable<AnalysisViewModel> Analyses { get; set; }
        public IEnumerable<InstitutionViewModel> Institutions { get; set; }

    }
}
