using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;

namespace DB.Application.Features.Instruments.Queries.ViewModels
{
    public class InstrumentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<InstitutionBasicViewModel> Institutions { get; set; }

        public InstrumentViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public InstrumentViewModel()
        {
        }
    }
}
