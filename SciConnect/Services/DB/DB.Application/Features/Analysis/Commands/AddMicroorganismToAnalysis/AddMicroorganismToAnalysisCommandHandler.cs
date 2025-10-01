using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;

namespace DB.Application.Features.Analysis.Commands.AddMicroorganismToAnalysis
{
    public class AddMicroorganismToAnalysisCommandHandler
        : IRequestHandler<AddMicroorganismToAnalysisCommand, Unit>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IPublishEndpoint _bus; 

        public AddMicroorganismToAnalysisCommandHandler(
            IMicroorganismRepository microorganismRepository,
            IAnalysisRepository analysisRepository,
            IPublishEndpoint bus)              
        {
            _microorganismRepository = microorganismRepository;
            _analysisRepository = analysisRepository;
            _bus = bus;                        
        }

        public async Task<Unit> Handle(AddMicroorganismToAnalysisCommand request, CancellationToken cancellationToken)
        {
            var microorganism = await _microorganismRepository.GetByIdAsync(request.MicroorganismId);
            var analysis = await _analysisRepository.GetByIdAsync(request.AnalysisId);

            if (microorganism == null || analysis == null)
                throw new ArgumentException("Invalid analysis or microorganism ID.");

            analysis.AddMicroorgnaism(microorganism);
            await _analysisRepository.UpdateAsync(analysis); 

           
            await _bus.Publish(new AutocompleteEntityChanged
            {
                Type = EntityType.Analysis,
                Kind = ChangeKind.Updated,
                Id = analysis.Id,
                Name = analysis.Name
            }, cancellationToken);

            return Unit.Value;
        }
    }
}