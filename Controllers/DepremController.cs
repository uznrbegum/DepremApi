// accept GET request, call DepremService, send response to F.END
using DepremApi.Services;
using DepremApi.DTOs;
using Microsoft.AspNetCore.Mvc; //MVC

namespace DepremApi.Controllers;

[ApiController] 
[Route("api/[controller]")] // URL pattern (api/Deprem)

public class DepremController: ControllerBase // receives GET request,calls DepremService,response

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
        CancellationToken cancellationToken)
    {
       try
        {
            var depremler = await _depremService.DepremleriGetirAsync(startDate, endDate, cancellationToken);
            
            return Ok(depremler);
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, "Deprem verileri alınamadı. Lütfen daha sonra tekrar deneyin.");
        }
    }
}