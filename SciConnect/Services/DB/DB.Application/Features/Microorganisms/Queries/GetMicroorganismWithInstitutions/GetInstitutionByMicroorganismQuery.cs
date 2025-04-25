using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Analysis.Queries.GetMicroorganismWithInstitutions
{
    public class GetInstitutionByMicroorganismQuery : IRequest<MicroorganismViewModel>
    {
        public string MicroorganismName { get; set; }

        public GetInstitutionByMicroorganismQuery(string microorganismName)
        {
            MicroorganismName = microorganismName;
        }
    }
}

