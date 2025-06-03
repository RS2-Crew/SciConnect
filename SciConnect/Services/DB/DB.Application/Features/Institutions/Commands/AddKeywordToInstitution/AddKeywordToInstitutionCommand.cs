using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DB.Application.Features.Institutions.Commands.AddKeywordToInstitution
{
    public class AddKeywordToInstitutionCommand : IRequest<Unit>
    {
        public int InstitutionId { get; }
        public int KeywordId { get; }

        public AddKeywordToInstitutionCommand(int institutionId, int keywordId)
        {
            InstitutionId = institutionId;
            KeywordId = keywordId;
        }
    }
}
