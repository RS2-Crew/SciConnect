using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Application.Features.Keywords.Commands.CreateKeyword;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DB.Application.Features.Keywords.Commands.CreateKeyword
{
    public class CreateKeywordCommandHandler : IRequestHandler<CreateKeywordCommand, int>
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IKeywordFactory _keywordFactory;
        private readonly ILogger<CreateKeywordCommandHandler> _logger;

        public CreateKeywordCommandHandler(IKeywordRepository keywordRepository, IKeywordFactory keywordFactory, ILogger<CreateKeywordCommandHandler> logger)
        {
            _keywordRepository = keywordRepository;
            _keywordFactory = keywordFactory;
            _logger = logger;
        }

        public async Task<int> Handle(CreateKeywordCommand request, CancellationToken cancellationToken)
        {
            var keyword = _keywordFactory.Create(request);

            await _keywordRepository.AddAsync(keyword);

            return keyword.Id;
        }
    }
}
