using DepremApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DepremApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;

    public AnalyticsController(
        AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAnalytics(
        CancellationToken cancellationToken)
    {
        var analytics =
            await _analyticsService.GetAnalyticsAsync(
                cancellationToken);

        return Ok(analytics);
    }
}