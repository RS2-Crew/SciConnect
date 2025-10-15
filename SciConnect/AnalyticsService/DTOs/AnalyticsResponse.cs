namespace AnalyticsService.DTOs
{
    public class SummaryAnalyticsResponse
    {
        public int TotalInstitutions { get; set; }
        public int TotalAnalyses { get; set; }
        public int TotalResearchers { get; set; }
        public int TotalInstruments { get; set; }
        public int TotalKeywords { get; set; }
        public int TotalMicroorganisms { get; set; }
    }

    public class InstitutionBreakdownResponse
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
        public int TotalAnalyses { get; set; }
        public int TotalResearchers { get; set; }
        public int TotalInstruments { get; set; }
    }

    public class TopInstitutionResponse
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
        public int AnalysisCount { get; set; }
    }
}
