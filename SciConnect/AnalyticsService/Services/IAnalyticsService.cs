using AnalyticsService.DTOs;

namespace AnalyticsService.Services
{
    public interface IAnalyticsService
    {
        Task<SummaryAnalyticsResponse> GetAnalyticsSummaryAsync();
        Task<InstitutionBreakdownResponse?> GetInstitutionBreakdownAsync(int institutionId);
        Task<List<TopInstitutionResponse>> GetTopInstitutionsAsync(int limit);
    }
}
