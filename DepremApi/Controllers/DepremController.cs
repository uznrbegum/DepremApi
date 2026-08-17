using DepremApi.Services;
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
    public async Task<IActionResult> DepremleriGetir(
        [FromQuery] string? startDate,
        [FromQuery] string? endDate,
        [FromQuery] double? minMagnitude,
        [FromQuery] double? maxMagnitude,
        [FromQuery] string? location,
        CancellationToken cancellationToken)
    {
        var depremler =
            await _depremService.DepremleriDbdenGetirAsync(
                startDate,
                endDate,
                minMagnitude,
                maxMagnitude,
                location,
                cancellationToken);

        return Ok(depremler);
    }
}