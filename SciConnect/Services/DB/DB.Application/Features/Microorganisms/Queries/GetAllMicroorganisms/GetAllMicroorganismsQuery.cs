using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Microorganisms.Queries.GetAllMicroorganisms
{
    public class GetAllMicroorganismsQuery : IRequest<IReadOnlyList<MicroorganismViewModel>>
    { 
    }
}
