using AnalyticsService.Data;
using AnalyticsService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Services
{
    public class AnalyticsServiceImpl : IAnalyticsService
    {
        private readonly AnalyticsContext _context;
        private readonly ILogger<AnalyticsServiceImpl> _logger;

        public AnalyticsServiceImpl(AnalyticsContext context, ILogger<AnalyticsServiceImpl> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SummaryAnalyticsResponse> GetAnalyticsSummaryAsync()
        {
            try
            {
                var summary = new SummaryAnalyticsResponse
                {
                    TotalInstitutions = await _context.Institutions.AsNoTracking().CountAsync(),
                    TotalAnalyses = await _context.Analyses.AsNoTracking().CountAsync(),
                    TotalResearchers = await _context.Employees.AsNoTracking().CountAsync(),
                    TotalInstruments = await _context.Instruments.AsNoTracking().CountAsync(),
                    TotalKeywords = await _context.Keywords.AsNoTracking().CountAsync(),
                    TotalMicroorganisms = await _context.Microorganisms.AsNoTracking().CountAsync()
                };

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics summary");
                throw;
            }
        }

        public async Task<InstitutionBreakdownResponse?> GetInstitutionBreakdownAsync(int institutionId)
        {
            try
            {
                var institution = await _context.Institutions
                    .AsNoTracking()
                    .Include(i => i.Analyses)
                    .Include(i => i.Employees)
                    .Include(i => i.Instruments)
                    .FirstOrDefaultAsync(i => i.Id == institutionId);

                if (institution == null)
                {
                    return null;
                }

                return new InstitutionBreakdownResponse
                {
                    InstitutionId = institution.Id,
                    InstitutionName = institution.Name,
                    TotalAnalyses = institution.Analyses.Count,
                    TotalResearchers = institution.Employees.Count,
                    TotalInstruments = institution.Instruments.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting institution breakdown for institution {InstitutionId}", institutionId);
                throw;
            }
        }

        public async Task<List<TopInstitutionResponse>> GetTopInstitutionsAsync(int limit)
        {
            try
            {
                var topInstitutions = await _context.Institutions
                    .AsNoTracking()
                    .Include(i => i.Analyses)
                    .Select(i => new TopInstitutionResponse
                    {
                        InstitutionId = i.Id,
                        InstitutionName = i.Name,
                        AnalysisCount = i.Analyses.Count
                    })
                    .OrderByDescending(i => i.AnalysisCount)
                    .Take(limit)
                    .ToListAsync();

                return topInstitutions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top institutions");
                throw;
            }
        }
    }
}
