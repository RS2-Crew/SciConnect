using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Microorganisms.Commands.DeleteMicroorganism;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Keywords.Commands.DeleteKeyword
{
    public class DeleteKeywordCommandHandler : IRequestHandler<DeleteKeywordCommand, Unit>
    {
        private readonly IKeywordRepository _repository;
        private readonly ILogger<DeleteKeywordCommandHandler> _logger;

        public DeleteKeywordCommandHandler(IKeywordRepository repository, ILogger<DeleteKeywordCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteKeywordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError("Keyword name must be provided.");
                throw new ArgumentException("Keyword name must be provided.");
            }

            var keyword = await _repository.GetByNameAsync(request.Name.Trim());

            if (keyword == null)
            {
                _logger.LogError($"Keyword with name {request.Name} not found.");
                throw new ArgumentException($"Keyword with name {request.Name} not found.");
            }

            await _repository.DeleteAsync(keyword);

            _logger.LogInformation($"Keyword with name {request.Name} successfully deleted.");

            return Unit.Value;
        }
    }
}
