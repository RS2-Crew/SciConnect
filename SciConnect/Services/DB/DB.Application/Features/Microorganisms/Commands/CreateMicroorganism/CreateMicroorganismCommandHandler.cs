using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Microorganisms.Commands.CreateMicroorganism
{
    class CreateMicroorganismCommandHandler : IRequestHandler<CreateMicroorganismCommand, int>
    {
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IMicroorganismFactory _microorganismFactory;
        private readonly ILogger<CreateMicroorganismCommandHandler> _logger;

        public CreateMicroorganismCommandHandler(IMicroorganismRepository microorganismRepository, IMicroorganismFactory microorganismFactory, ILogger<CreateMicroorganismCommandHandler> logger)
        {
            _microorganismRepository = microorganismRepository;
            _microorganismFactory = microorganismFactory;
            _logger = logger;
        }

        public async Task<int> Handle(CreateMicroorganismCommand request, CancellationToken cancellationToken)
        {
            var microorganism = _microorganismFactory.Create(request);

            await _microorganismRepository.AddAsync(microorganism);

            return microorganism.Id;
        }
    }
}
