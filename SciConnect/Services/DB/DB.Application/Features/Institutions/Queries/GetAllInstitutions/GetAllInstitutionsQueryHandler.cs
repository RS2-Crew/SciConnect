using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetAllInstitutions
{
    public class GetAllInstitutionsQueryHandler : IRequestHandler<GetAllInstitutionsQuery, IReadOnlyList<InstitutionViewModel>>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IInstitutionViewModelFactory _viewModelFactory;

        public GetAllInstitutionsQueryHandler(IInstitutionRepository institutionRepository, IInstitutionViewModelFactory viewModelFactory)
        {
            _institutionRepository = institutionRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyList<InstitutionViewModel>> Handle(GetAllInstitutionsQuery request, CancellationToken cancellationToken)
        {
            var institutions = await _institutionRepository.GetAllAsync();
            return institutions.Select(_viewModelFactory.CreateViewModel).ToList();
        }
    }
}