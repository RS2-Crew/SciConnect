using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Microorganisms.Queries.GetAllMicroorganisms
{
    public class GetListOfMicroorganismsQueryHandler : IRequestHandler<GetAllMicroorganismsQuery, PagedResult<MicroorganismViewModel>>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IMicroorganismViewModelFactory _viewModelFactory;

        public GetListOfMicroorganismsQueryHandler(IMicroorganismRepository microorganismRepository, IMicroorganismViewModelFactory viewModelFactory)
        {
            _microorganismRepository = microorganismRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<PagedResult<MicroorganismViewModel>> Handle(GetAllMicroorganismsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _microorganismRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var viewModels = pagedResult.Items.Select(_viewModelFactory.CreateViewModel).ToList().AsReadOnly();
            return new PagedResult<MicroorganismViewModel>(viewModels, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }
    }
}
