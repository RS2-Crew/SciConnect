using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Analyses.Commands.CreateAnalysis
{
    public class CreateAnalysisCommandHandler : IRequestHandler<CreateAnalysisCommand, int>
    {
        private readonly IAnalysisRepository _repository;
        private readonly IAnalysisFactory _factory;
        private readonly ILogger<CreateAnalysisCommandHandler> _logger;

        public CreateAnalysisCommandHandler(IAnalysisRepository repository, IAnalysisFactory factory, ILogger<CreateAnalysisCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(CreateAnalysisCommand request, CancellationToken cancellationToken)
        {
            var analysisEntity = _factory.Create(request);
            var newAnalysis = await _repository.AddAsync(analysisEntity);

            _logger.LogInformation($"Analysis {newAnalysis.Id} successfully created.");

            return newAnalysis.Id;
        }
    }
}
