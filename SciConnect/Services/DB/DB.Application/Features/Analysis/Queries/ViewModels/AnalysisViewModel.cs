using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Application.Features.Analysis.Queries.ViewModels
{
    public class AnalysisViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AnalysisViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
