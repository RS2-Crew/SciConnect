namespace AnalyticsService.DTOs
{
    public class DetailedAnalyticsResponse
    {
        public string EntityType { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class SummaryAnalyticsResponse
    {
        public int TotalInstitutions { get; set; }
        public int TotalAnalyses { get; set; }
        public int TotalResearchers { get; set; }
        public int TotalInstruments { get; set; }
        public int TotalKeywords { get; set; }
        public int TotalMicroorganisms { get; set; }
    }
}
