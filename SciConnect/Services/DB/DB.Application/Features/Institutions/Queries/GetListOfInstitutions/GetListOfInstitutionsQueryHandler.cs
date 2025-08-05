using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Institutions.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Institutions.Queries.GetListOfInstitutions
{
    public class GetListOfInstitutionsQueryHandler : IRequestHandler<GetListOfInstitutionsQuery, List<InstitutionViewModel>>
    {
        private readonly IInstitutionViewModelFactory _factory;
        private readonly IInstitutionRepository _repository;

        public GetListOfInstitutionsQueryHandler(IInstitutionViewModelFactory factory, IInstitutionRepository repository)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<InstitutionViewModel>> Handle(GetListOfInstitutionsQuery request, CancellationToken cancellationToken)
        {
            var institutionsList = await _repository.GetInstitutionByName(request.Name);
            return institutionsList.Select(i => _factory.CreateViewModel(i)).ToList();
        }
    }
}