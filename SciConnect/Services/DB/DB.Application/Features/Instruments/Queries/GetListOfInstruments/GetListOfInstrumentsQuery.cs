using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Instruments.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Instruments.Queries.GetListOfInstruments
{
    public class GetListOfInstrumentsQuery : IRequest<IReadOnlyCollection<InstrumentViewModel>>
    {
        public string Name { get; set; }

        public GetListOfInstrumentsQuery(string name)
        {
            Name = name;
        }
    }
}
