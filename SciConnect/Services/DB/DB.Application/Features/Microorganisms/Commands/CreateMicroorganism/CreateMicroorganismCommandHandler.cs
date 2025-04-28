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
        //private readonly IMicroorganismRepository _microorganismRepository;
        //private readonly IMicroorganismFactory _microorganismFactory;
        //private readonly ILogger<CreateMicroorganismCommandHandler> _logger;

        //public CreateMicroorganismCommandHandler(IMicroorganismRepository microorganismRepository, IMicroorganismFactory microorganismFactory, ILogger<CreateMicroorganismCommandHandler> logger)
        //{
        //    _microorganismRepository = microorganismRepository;
        //    _microorganismFactory = microorganismFactory;
        //    _logger = logger;
        //}

        //public async Task<int> Handle(CreateMicroorganismCommand request, CancellationToken cancellationToken)
        //{
        //    var microorganism = _microorganismFactory.Create(request);

        //    await _microorganismRepository.AddAsync(microorganism);

        //    return microorganism.Id;
        

        private readonly IAnalysisRepository _analysisRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMicroorganismRepository _microorganismRepository;
        private readonly IKeywordRepository _keywordRepository;
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IMicroorganismFactory _factory;
        private readonly ILogger<CreateMicroorganismCommandHandler> _logger;

        public CreateMicroorganismCommandHandler(
            IAnalysisRepository analysisRepository,
            IEmployeeRepository employeeRepository,
            IMicroorganismRepository microorganismRepository,
            IKeywordRepository keywordRepository,
            IInstitutionRepository institutionRepository,
            IMicroorganismFactory factory,
            ILogger<CreateMicroorganismCommandHandler> logger)
        {
            _analysisRepository = analysisRepository;
            _employeeRepository = employeeRepository;
            _microorganismRepository = microorganismRepository;
            _keywordRepository = keywordRepository;
            _institutionRepository = institutionRepository;
            _factory = factory;
            _logger = logger;
        }

        public async Task<int> Handle(CreateMicroorganismCommand request, CancellationToken cancellationToken)
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


            var microorganism = _factory.Create(request);

            var addedAnalyses = new List<DB.Domain.Entities.Analysis>();

            // Povezivanje postojećih mikroorganizama
            if (request.Analisys?.Any() == true)
            {
                foreach (var analysisName in request.Analisys)
                {
                    var analysis = await _analysisRepository.GetByNameAsync(analysisName);
                    if (analysis == null)
                        throw new Exception($"Analysis with name {analysisName} not found.");

                    addedAnalyses.Add(analysis);
                    microorganism.AddAnalysis(analysis);
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

            await _microorganismRepository.AddAsync(microorganism);


            if (!institution.Microorganisms.Any(m => m.Name == microorganism.Name))
            {
                institution.AddMicroorgnaism(microorganism);
            }

            // Dodaj analizu u instituciju

            foreach (var an in addedAnalyses)
            {

                if (!institution.Analyses.Any(a => a.Id == an.Id))
                {
                    institution.AddAnalysis(an);
                }

            }

            // Sačuvaj izmene
            await _institutionRepository.UpdateAsync(institution);

            return microorganism.Id;
        }
    }
}
