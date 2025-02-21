using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Instruments.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Instruments.Queries.GetAllInstruments
{
    public class GetAllInstrumentsQuery : IRequest<IReadOnlyList<InstrumentViewModel>>
    {
    }
}
