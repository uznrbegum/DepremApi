using System.Text.Json;
using DepremApi.Data;
using DepremApi.DTOs;
using DepremApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DepremApi.Services;

public class DepremService
{
    private readonly HttpClient _httpClient;
    private readonly DepremDbContext _dbContext;

    public DepremService(
        HttpClient httpClient,
        DepremDbContext dbContext)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    // =========================================================
    // 1. AFAD'DAN VERİ AL → SQL SERVER'A KAYDET
    // SADECE BACKGROUND SERVICE KULLANACAK
    // =========================================================

    public async Task DepremleriAfaddanGetirVeKaydetAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken)
    {
        var url =
            "https://deprem.afad.gov.tr/apiv2/event/filter" +
            $"?start={Uri.EscapeDataString(start.ToString("yyyy-MM-dd HH:mm:ss"))}" +
            $"&end={Uri.EscapeDataString(end.ToString("yyyy-MM-dd HH:mm:ss"))}";

        var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var depremler =
            JsonSerializer.Deserialize<List<DepremDto>>(
                json,
                options);

        if (depremler == null || depremler.Count == 0)
        {
            return;
        }

        // AFAD verilerini DB Entity'lerine çevir
        var entities = depremler
            .Where(d => !string.IsNullOrEmpty(d.EventId))
            .Select(d => new Deprem
            {
                EventId = d.EventId,
                Konum = d.Konum,
                Buyukluk = d.Buyukluk,
                Derinlik = d.Derinlik,
                Tarih = DateTime.Parse(d.Tarih!)
            })
            .ToList();

        if (entities.Count == 0)
        {
            return;
        }

        // DB'de zaten bulunan EventId'leri bul
        var eventIds = entities
            .Select(d => d.EventId)
            .ToList();

        var existingEventIds =
            await _dbContext.Depremler
                .Where(d => eventIds.Contains(d.EventId))
                .Select(d => d.EventId)
                .ToListAsync(cancellationToken);

        // SADECE YENİ DEPREMLER
        var yeniDepremler = entities
            .Where(d => !existingEventIds.Contains(d.EventId))
            .ToList();

        if (yeniDepremler.Count == 0)
        {
            Console.WriteLine("Yeni deprem bulunamadı.");
            return;
        }

        _dbContext.Depremler.AddRange(yeniDepremler);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        Console.WriteLine(
            $"{yeniDepremler.Count} yeni deprem DB'ye kaydedildi.");
    }


    // =========================================================
    // 2. SQL SERVER'DAN VERİ AL → CLIENT'A GÖNDER
    // SADECE CONTROLLER KULLANACAK
    // =========================================================

    public async Task<List<DepremDto>> DepremleriDbdenGetirAsync(
        string? startDate,
        string? endDate,
        double? minMagnitude,
        double? maxMagnitude,
        string? location,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Depremler
            .AsNoTracking()
            .AsQueryable();

        // Tarih filtresi
        if (DateTime.TryParse(startDate, out var start))
        {
            query = query.Where(d => d.Tarih >= start);
        }

        if (DateTime.TryParse(endDate, out var end))
        {
            // Bitiş tarihinin tamamını dahil et
            end = end.Date.AddDays(1).AddTicks(-1);

            query = query.Where(d => d.Tarih <= end);
        }

        // Minimum büyüklük
        if (minMagnitude.HasValue)
        {
            query = query.Where(
                d => d.Buyukluk >= minMagnitude.Value);
        }

        // Maximum büyüklük
        if (maxMagnitude.HasValue)
        {
            query = query.Where(
                d => d.Buyukluk <= maxMagnitude.Value);
        }

        // Konum
        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(
                d => d.Konum != null &&
                     d.Konum.Contains(location));
        }

        var depremler = await query
            .OrderByDescending(d => d.Tarih)
            .Select(d => new DepremDto
            {
                EventId = d.EventId,
                Konum = d.Konum,
                Buyukluk = d.Buyukluk,
                Derinlik = d.Derinlik,
                Tarih = d.Tarih.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync(cancellationToken);

        return depremler;
    }


    // =========================================================
    // 3. DB'DEKİ SON DEPREM TARİHİNİ BUL
    // BACKGROUND SERVICE KULLANACAK
    // =========================================================

    public async Task<DateTime?> SonDepremTarihiniGetirAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Depremler
            .OrderByDescending(d => d.Tarih)
            .Select(d => (DateTime?)d.Tarih)
            .FirstOrDefaultAsync(cancellationToken);
    }
}