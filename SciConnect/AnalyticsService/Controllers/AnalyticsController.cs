using AnalyticsService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AnalyticsService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("summary")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAnalyticsSummary()
        {
            try
            {
                var result = await _analyticsService.GetAnalyticsSummaryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics summary");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("institution/{institutionId}/breakdown")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInstitutionBreakdown(int institutionId)
        {
            try
            {
                var result = await _analyticsService.GetInstitutionBreakdownAsync(institutionId);
                if (result == null)
                {
                    return NotFound($"Institution with ID {institutionId} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting institution breakdown for {InstitutionId}", institutionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("institutions/top")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopInstitutions([FromQuery] int limit = 5)
        {
            try
            {
                if (limit <= 0 || limit > 200)
                {
                    return BadRequest("Limit must be between 1 and 200");
                }

                var result = await _analyticsService.GetTopInstitutionsAsync(limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top institutions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
