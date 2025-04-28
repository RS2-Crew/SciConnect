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
        private readonly ILogger<CreateAnalysisCommandHandler> _logger;

        public CreateAnalysisCommandHandler(
            IAnalysisRepository analysisRepository,
            IEmployeeRepository employeeRepository,
            IMicroorganismRepository microorganismRepository,
            IKeywordRepository keywordRepository,
            IInstitutionRepository institutionRepository,
            IAnalysisFactory factory,
            ILogger<CreateAnalysisCommandHandler> logger)
        {
            _analysisRepository = analysisRepository;
            _employeeRepository = employeeRepository;
            _microorganismRepository = microorganismRepository;
            _keywordRepository = keywordRepository;
            _institutionRepository = institutionRepository;
            _factory = factory;
            _logger = logger;
        }

        public async Task<int> Handle(CreateAnalysisCommand request, CancellationToken cancellationToken)
        {
            // Hardkodirani EmployeeId dok ne povežemo Identity
            var employeeId = 1;
            var employee = await _employeeRepository
                .GetQueryable()
                .Include(e => e.Institution)
                .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken)
                ?? throw new Exception("Employee not found.");


            //var institution = await _institutionRepository.GetByIdAsync(employee.Institution.Id)
            //    ?? throw new Exception("Employee does not belong to any institution.");

            var institution = await _institutionRepository
               .GetQueryable()
               .Include(i => i.Analyses)
               .Include(i => i.Microorganisms)
               .FirstOrDefaultAsync(i => i.Id == employee.Institution.Id, cancellationToken)
               ?? throw new Exception("Employee does not belong to any institution.");

            var analysis = _factory.Create(request);

            var addedMicroorganisms = new List<Microorganism>();

            // Povezivanje postojećih mikroorganizama
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

            //// Povezivanje postojećih ključnih reči
            //if (request.KeywordIds?.Any() == true)
            //{
            //    foreach (var keywordId in request.KeywordIds)
            //    {
            //        var keyword = await _keywordRepository.GetByIdAsync(keywordId);
            //        if (keyword == null)
            //            throw new Exception($"Keyword with ID {keywordId} not found.");

            //        analysis.Keywords.Add(keyword);
            //    }
            //}

            await _analysisRepository.AddAsync(analysis);


            if (!institution.Analyses.Any(a => a.Name == analysis.Name))
            {
                institution.AddAnalysis(analysis);
            }
            // Dodaj analizu u instituciju
            institution.AddAnalysis(analysis);

            foreach (var mi in addedMicroorganisms)
            {

                if (!institution.Microorganisms.Any(m => m.Id == mi.Id))
                {
                    institution.AddMicroorgnaism(mi);
                }

            }

            // Sačuvaj izmene
            await _institutionRepository.UpdateAsync(institution);

            return analysis.Id;
        }
    }
}
