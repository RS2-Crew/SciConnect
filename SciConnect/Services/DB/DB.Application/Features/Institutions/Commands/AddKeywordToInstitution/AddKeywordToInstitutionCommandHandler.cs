using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Persistance;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddKeywordToInstitution
{
    public class AddKeywordToInstitutionCommandHandler : IRequestHandler<AddKeywordToInstitutionCommand, Unit>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IKeywordRepository _keywordRepository;

        public AddKeywordToInstitutionCommandHandler(IInstitutionRepository institutionRepository, IKeywordRepository keywordRepository)
        {
            _institutionRepository = institutionRepository;
            _keywordRepository = keywordRepository;
        }

        public async Task<Unit> Handle(AddKeywordToInstitutionCommand request, CancellationToken cancellationToken)
        {
            var institution = await _institutionRepository.GetByIdAsync(request.InstitutionId);
            var keyword = await _keywordRepository.GetByIdAsync(request.KeywordId);

            if (institution == null || keyword == null)
            {
                throw new ArgumentException("Invalid keyword or institution ID.");
            }

            institution.AddKeyword(keyword);
            await _institutionRepository.UpdateAsync(institution);

            return Unit.Value;
        }
    }
}
