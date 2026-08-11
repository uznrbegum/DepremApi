// Accept GET request, call DepremService, send response to frontend

using DepremApi.Services;
using DepremApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DepremApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepremController : ControllerBase
{
    private readonly DepremService _depremService;

    public DepremController(DepremService depremService)
    {
        _depremService = depremService;
    }

    [HttpGet]
    public async Task<IActionResult> DepremleriGetirAsync(
        [FromQuery] string? startDate,
        [FromQuery] string? endDate,
        [FromQuery] double? minMagnitude,
        [FromQuery] double? maxMagnitude,
        [FromQuery] string? location,
        CancellationToken cancellationToken)
    {
        try
        {
            var depremler =
                await _depremService.DepremleriGetirAsync(
                    startDate,
                    endDate,
                    minMagnitude,
                    maxMagnitude,
                    location,
                    cancellationToken);

            return Ok(depremler);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                503,
                "Deprem verileri alınamadı. Lütfen daha sonra tekrar deneyin.");
        }
    }
}