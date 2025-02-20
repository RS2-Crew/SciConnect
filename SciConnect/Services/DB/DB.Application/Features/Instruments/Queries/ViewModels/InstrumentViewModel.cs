using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Application.Features.Instruments.Queries.ViewModels
{
    public class InstrumentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

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
