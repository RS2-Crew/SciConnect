using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Microorganisms.Queries
{
    public class GetListOfMicroorganismsQuery : IRequest<IReadOnlyCollection<MicroorganismViewModel>>
    {
        public string Name { get; set; }

        public GetListOfMicroorganismsQuery(string name)
        {
            Name = name;
        }
    }
}
