using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetAllInstitutions
{
    public class GetAllInstitutionsQueryHandler : IRequestHandler<GetAllInstitutionsQuery, PagedResult<InstitutionViewModel>>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IInstitutionViewModelFactory _viewModelFactory;

        public GetAllInstitutionsQueryHandler(IInstitutionRepository institutionRepository, IInstitutionViewModelFactory viewModelFactory)
        {
            _institutionRepository = institutionRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<PagedResult<InstitutionViewModel>> Handle(GetAllInstitutionsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _institutionRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var viewModels = pagedResult.Items.Select(_viewModelFactory.CreateViewModel).ToList().AsReadOnly();
            return new PagedResult<InstitutionViewModel>(viewModels, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }
    }
}