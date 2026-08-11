using System.Text.Json; //JSON C# 
using DepremApi.DTOs;

namespace DepremApi.Services;

public class DepremService //external api communication
{
    private readonly HttpClient _httpClient; //for GET request, readonly=not changeable

    public DepremService(HttpClient httpClient) //Constructor works otomatically
    {
        _httpClient = httpClient;
    }

    public async Task<List<DepremDto>> DepremleriGetirAsync(
        string? startDate, 
        string? endDate,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) // && = AND, ||= OR
        {
            startDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd 00:00:00" ); // yesterday's date
            endDate = DateTime.Today.ToString("yyyy-MM-dd 23:59:59"); // today's date
        }
        else if (string.IsNullOrEmpty(startDate))
        {
            startDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd 00:00:00"); // yesterday's date
        }
        else if (string.IsNullOrEmpty(endDate))
        {
            endDate = DateTime.Today.ToString("yyyy-MM-dd 23:59:59"); // today's date
        }

        var url = $"https://deprem.afad.gov.tr/apiv2/event/filter?start={startDate}&end={endDate}"; 

        var response = await _httpClient.GetAsync(url, cancellationToken); //API call, cancellationToken = cancel request if needed

        response.EnsureSuccessStatusCode(); //unsuccessful response = exception throw

        var json = await response.Content.ReadAsStringAsync(); //JSON'ı string olarak okuyor

        var options = new JsonSerializerOptions //JSON C# translation settings
        {
            PropertyNameCaseInsensitive = true //upper-lowercase letters
        };

        var depremler = JsonSerializer.Deserialize<List<DepremDto>>(json, options); // main JSON C# translation

        return depremler ?? new List<DepremDto>(); //null ise boş liste döndür
    }
}