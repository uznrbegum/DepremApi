using System.Text.Json;
using DepremApi.DTOs;

namespace DepremApi.Services;

public class DepremService // External API communication
{
    private readonly HttpClient _httpClient;

    public DepremService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DepremDto>> DepremleriGetirAsync(
        string? startDate,
        string? endDate,
        double? minMagnitude,
        double? maxMagnitude,
        string? location,
        CancellationToken cancellationToken)
    {
        // Tarih aralığını belirliyoruz
        DateTime start;
        DateTime end;

        if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
        {
            // İki tarih de verilmezse:
            // Dün 00:00 -> Bugün 23:59:59
            start = DateTime.Today.AddDays(-1);
            end = DateTime.Today.AddDays(1).AddTicks(-1);
        }
        else if (string.IsNullOrEmpty(startDate))
        {
            // Başlangıç tarihi yoksa:
            // Dün 00:00 -> Verilen bitiş tarihinin sonu
            start = DateTime.Today.AddDays(-1);
            end = DateTime.Parse(endDate!).Date.AddDays(1).AddTicks(-1);
        }
        else if (string.IsNullOrEmpty(endDate))
        {
            // Bitiş tarihi yoksa:
            // Verilen başlangıç tarihinin başlangıcı -> Bugün 23:59:59
            start = DateTime.Parse(startDate!).Date;
            end = DateTime.Today.AddDays(1).AddTicks(-1);
        }
        else
        {
            // İki tarih de verilirse:
            // Başlangıç günü 00:00 -> Bitiş günü 23:59:59
            start = DateTime.Parse(startDate!).Date;
            end = DateTime.Parse(endDate!).Date.AddDays(1).AddTicks(-1);
        }

        // AFAD API URL'sini oluşturuyoruz
        var url =
            $"https://deprem.afad.gov.tr/apiv2/event/filter" +
            $"?start={Uri.EscapeDataString(start.ToString("yyyy-MM-dd HH:mm:ss"))}" +
            $"&end={Uri.EscapeDataString(end.ToString("yyyy-MM-dd HH:mm:ss"))}";

        // AFAD API'ye GET isteği gönderiyoruz
        var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        // İstek başarısızsa exception oluştur
        response.EnsureSuccessStatusCode();

        // Gelen JSON verisini string olarak okuyoruz
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        // JSON -> C# ayarları
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // JSON verisini DepremDto listesine çeviriyoruz
        var depremler =
            JsonSerializer.Deserialize<List<DepremDto>>(json, options);

        // AFAD'dan gelen verileri tekrar bizim tarih aralığımıza göre filtreliyoruz
        depremler = depremler?
            .Where(d =>
            {
                if (!DateTime.TryParse(d.Tarih, out var depremTarihi))
                    return false;

                return depremTarihi >= start &&
                       depremTarihi <= end;
            })
            .ToList();

        // Minimum magnitude filtresi
        if (minMagnitude.HasValue)
        {
            depremler = depremler?
                .Where(d => d.Buyukluk >= minMagnitude.Value)
                .ToList();
        }

        // Maximum magnitude filtresi
        if (maxMagnitude.HasValue)
        {
            depremler = depremler?
                .Where(d => d.Buyukluk <= maxMagnitude.Value)
                .ToList();
        }

        // Location filtresi
        if (!string.IsNullOrEmpty(location))
        {
            depremler = depremler?
                .Where(d =>
                    d.Konum != null &&
                    d.Konum.Contains(
                        location,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Sonuç null ise boş liste döndür
        return depremler ?? new List<DepremDto>();
    }
}