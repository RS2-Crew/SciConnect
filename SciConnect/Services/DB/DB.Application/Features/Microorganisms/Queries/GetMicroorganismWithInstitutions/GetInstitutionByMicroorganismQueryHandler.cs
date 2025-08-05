using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DB.Application.Features.Analysis.Queries.GetMicroorganismWithInstitutions;
using DB.Domain.Entities;

namespace DB.Application.Features.Institutions.Queries.GetMicroorganismWithInstitutions
{
    public class GetInstitutionByMicroorganismQueryHandler : IRequestHandler<GetInstitutionByMicroorganismQuery, MicroorganismViewModel>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IInstitutionViewModelFactory _institutionVMFactory;

        public GetInstitutionByMicroorganismQueryHandler(
            IMicroorganismRepository microorganismRepository,
            IInstitutionViewModelFactory institutionVMFactory)
        {
            _microorganismRepository = microorganismRepository;
            _institutionVMFactory = institutionVMFactory;
        }

        public async Task<MicroorganismViewModel> Handle(GetInstitutionByMicroorganismQuery request, CancellationToken cancellationToken)
        {
            var microorganism = await _microorganismRepository
                 .GetQueryable()
                .Include(a => a.Institutions)
                .FirstOrDefaultAsync(a => a.Name.ToLower() == request.MicroorganismName.ToLower(), cancellationToken);


            var viewModel = new MicroorganismViewModel
            {
                Id = microorganism.Id,
                Name = microorganism.Name,
                Institutions = microorganism.Institutions
                    .Select(m => _institutionVMFactory.CreateViewModel(m))
                    .ToList()
            };

            return viewModel;
        }
    }
}
