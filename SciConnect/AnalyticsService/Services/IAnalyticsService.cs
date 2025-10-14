using AnalyticsService.DTOs;

namespace AnalyticsService.Services
{
    public interface IAnalyticsService
    {
        Task<SummaryAnalyticsResponse> GetAnalyticsSummaryAsync();
        Task<List<DetailedAnalyticsResponse>> GetDetailedAnalyticsAsync(string entityType);
    }
}
