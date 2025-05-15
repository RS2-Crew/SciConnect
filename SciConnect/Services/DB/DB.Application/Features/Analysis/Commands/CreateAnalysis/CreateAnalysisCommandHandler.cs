using DB.Application.Common;
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
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IKeywordRepository _keywordRepository;
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IAnalysisFactory _factory;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<CreateAnalysisCommandHandler> _logger;

        public CreateAnalysisCommandHandler(
            IAnalysisRepository analysisRepository,
            IEmployeeRepository employeeRepository,
            IMicroorganismRepository microorganismRepository,
            IKeywordRepository keywordRepository,
            IInstitutionRepository institutionRepository,
            IAnalysisFactory factory,
            ICurrentUserService currentUser,
            ILogger<CreateAnalysisCommandHandler> logger)
        {
            _analysisRepository = analysisRepository;
            _employeeRepository = employeeRepository;
            _microorganismRepository = microorganismRepository;
            _keywordRepository = keywordRepository;
            _institutionRepository = institutionRepository;
            _factory = factory;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<int> Handle(CreateAnalysisCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUser.Username;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var employee = await _employeeRepository
                .GetQueryable()
                .Include(e => e.Institution)
                .FirstOrDefaultAsync(e => e.Username == username, cancellationToken)
                ?? throw new Exception("Employee not found.");

            var institution = await _institutionRepository
                .GetQueryable()
                .Include(i => i.Analyses)
                .Include(i => i.Microorganisms)
                .FirstOrDefaultAsync(i => i.Id == employee.Institution.Id, cancellationToken)
                ?? throw new Exception("Employee does not belong to any institution.");

            var analysis = _factory.Create(request);

            var addedMicroorganisms = new List<Microorganism>();

            if (request.Microorganism?.Any() == true)
            {
                foreach (var microorganismName in request.Microorganism)
                {
                    var microorganism = await _microorganismRepository.GetByNameAsync(microorganismName);
                    if (microorganism == null)
                        throw new Exception($"Microorganism with name {microorganismName} not found.");

                    addedMicroorganisms.Add(microorganism);
                    analysis.AddMicroorgnaism(microorganism);
                }
            }

            await _analysisRepository.AddAsync(analysis);

            if (!institution.Analyses.Any(a => a.Name == analysis.Name))
            {
                institution.AddAnalysis(analysis);
            }

            institution.AddAnalysis(analysis);

            foreach (var mi in addedMicroorganisms)
            {
                if (!institution.Microorganisms.Any(m => m.Id == mi.Id))
                {
                    institution.AddMicroorgnaism(mi);
                }
            }

            await _institutionRepository.UpdateAsync(institution);

            return analysis.Id;
        }
    }
}
