using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Application.Contracts.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Analyses.Commands.DeleteAnalysis
{
    public class DeleteAnalysisCommandHandler : IRequestHandler<DeleteAnalysisCommand, Unit>
    {
        private readonly IAnalysisRepository _repository;
        private readonly ILogger<DeleteAnalysisCommandHandler> _logger;

        public DeleteAnalysisCommandHandler(IAnalysisRepository repository, ILogger<DeleteAnalysisCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteAnalysisCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError("Analysis name must be provided.");
                throw new ArgumentException("Analysis name must be provided.");
            }

            var analysis = await _repository.GetByNameAsync(request.Name.Trim());

            if (analysis == null)
            {
                _logger.LogError($"Analysis with name '{request.Name}' not found.");
                throw new ArgumentException($"Analysis with name '{request.Name}' not found.");
            }

            await _repository.DeleteAsync(analysis);

            _logger.LogInformation($"Analysis with name '{request.Name}' successfully deleted.");

            return Unit.Value;
        }
    }
}
