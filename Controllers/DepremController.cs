using DepremApi.Services;
using DepremApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DepremApi.Controllers;

[ApiController] 
[Route("api/[controller]")]

public class DepremController // API'nin deprem verilerini yönetir

{
    private readonly DepremService _depremService;

    public DepremController(DepremService depremService)
    {
        _depremService = depremService;
    }
   
    [HttpGet]
    
    public async Task<List<DepremDto>> GetDepremlerAsync()
    {
        return await _depremService.DepremleriGetirAsync();
    }
}