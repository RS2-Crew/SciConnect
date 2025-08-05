using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetListOfInstitutions
{
    public class GetListOfInstitutionsQuery : IRequest<List<InstitutionViewModel>>
    {
        public string Name { get; set; }

        public GetListOfInstitutionsQuery(string name)
        {
            Name = name;
        }
    }
}
