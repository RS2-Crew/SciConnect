using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Instruments.Queries.GetListOfInstruments;
using DB.Application.Features.Instruments.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using MediatR;

namespace DB.Application.Features.Microorganisms.Queries.GetListOfMicroorganisms
{
    public class GetListOfMicroorganismsQueryHandler : IRequestHandler<GetListOfMicroorganismsQuery, IReadOnlyCollection<MicroorganismViewModel>>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IMicroorganismViewModelFactory _viewModelFactory;

        public GetListOfMicroorganismsQueryHandler(IMicroorganismRepository microorganismRepository, IMicroorganismViewModelFactory viewModelFactory)
        {

            Console.WriteLine("🟢 Handler for GetListOfMicroorganismsQuery is registered and instantiated.");

            _microorganismRepository = microorganismRepository;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IReadOnlyCollection<MicroorganismViewModel>> Handle(GetListOfMicroorganismsQuery request, CancellationToken cancellationToken)
        {
            var microorganisms = await _microorganismRepository.GetMicroorganismByName(request.Name);

            return microorganisms.Select(i => _viewModelFactory.CreateViewModel(i)).ToList().AsReadOnly();
        }
    }
}
