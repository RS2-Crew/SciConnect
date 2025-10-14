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
                    TotalInstitutions = await _context.Institutions.CountAsync(),
                    TotalAnalyses = await _context.Analyses.CountAsync(),
                    TotalResearchers = await _context.Employees.CountAsync(),
                    TotalInstruments = await _context.Instruments.CountAsync(),
                    TotalKeywords = await _context.Keywords.CountAsync(),
                    TotalMicroorganisms = await _context.Microorganisms.CountAsync()
                };

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics summary");
                throw;
            }
        }

        public async Task<List<DetailedAnalyticsResponse>> GetDetailedAnalyticsAsync(string entityType)
        {
            try
            {
                var details = new List<DetailedAnalyticsResponse>();

                switch (entityType.ToLower())
                {
                    case "institution":
                        details = await _context.Institutions
                            .Select(x => new DetailedAnalyticsResponse
                            {
                                EntityType = "Institution",
                                EntityName = x.Name,
                                Count = 1
                            })
                            .ToListAsync();
                        break;
                    case "analysis":
                        details = await _context.Analyses
                            .Select(x => new DetailedAnalyticsResponse
                            {
                                EntityType = "Analysis",
                                EntityName = x.Name,
                                Count = 1
                            })
                            .ToListAsync();
                        break;
                    case "researcher":
                        details = await _context.Employees
                            .Select(x => new DetailedAnalyticsResponse
                            {
                                EntityType = "Researcher",
                                EntityName = $"{x.FirstName} {x.LastName}",
                                Count = 1
                            })
                            .ToListAsync();
                        break;
                    case "instrument":
                        details = await _context.Instruments
                            .Select(x => new DetailedAnalyticsResponse
                            {
                                EntityType = "Instrument",
                                EntityName = x.Name,
                                Count = 1
                            })
                            .ToListAsync();
                        break;
                    case "keyword":
                        details = await _context.Keywords
                            .Select(x => new DetailedAnalyticsResponse
                            {
                                EntityType = "Keyword",
                                EntityName = x.Name,
                                Count = 1
                            })
                            .ToListAsync();
                        break;
                    case "microorganism":
                        details = await _context.Microorganisms
                            .Select(x => new DetailedAnalyticsResponse
                            {
                                EntityType = "Microorganism",
                                EntityName = x.Name,
                                Count = 1
                            })
                            .ToListAsync();
                        break;
                    default:
                        throw new ArgumentException($"Unknown entity type: {entityType}");
                }

                return details;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting detailed analytics for {EntityType}", entityType);
                throw;
            }
        }
    }
}
