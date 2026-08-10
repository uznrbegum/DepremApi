using System.Text.Json;
using DepremApi.DTOs;

namespace DepremApi.Services;

public class DepremService
{
    private readonly HttpClient _httpClient;

    public DepremService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DepremDto>> DepremleriGetirAsync()
    {
        var url = "https://deprem.afad.gov.tr/apiv2/event/filter?start=2026-08-09%2000:00:00&end=2026-08-10%2023:59:59";

        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var depremler = JsonSerializer.Deserialize<List<DepremDto>>(json, options);

        return depremler ?? new List<DepremDto>();
    }
}