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
    public class GetListOfMicroorganismsQueryHandler : IRequestHandler<GetAllMicroorganismsQuery, IReadOnlyList<MicroorganismViewModel>>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IMicroorganismViewModelFactory _viewModelFactory;

        public GetListOfMicroorganismsQueryHandler(IMicroorganismRepository microorganismRepository, IMicroorganismViewModelFactory viewModelFactory)
        {
            _microorganismRepository = microorganismRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyList<MicroorganismViewModel>> Handle(GetAllMicroorganismsQuery request, CancellationToken cancellationToken)
        {
            var microorganisms = await _microorganismRepository.GetAllAsync();
            return microorganisms.Select(_viewModelFactory.CreateViewModel).ToList();
        }

    }
}
