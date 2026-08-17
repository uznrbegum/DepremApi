using DepremApi.Data;
using DepremApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepremApi.Services;

public class AnalyticsService
{
    private readonly DepremDbContext _dbContext;

    public AnalyticsService(DepremDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnalyticsDto> GetAnalyticsAsync(
        CancellationToken cancellationToken)
    {
        var depremler = _dbContext.Depremler;

        var total = await depremler.CountAsync(cancellationToken);

        var averageMagnitude = total > 0
            ? await depremler.AverageAsync(
                d => d.Buyukluk,
                cancellationToken)
            : 0;

        var maximumMagnitude = total > 0
            ? await depremler.MaxAsync(
                d => d.Buyukluk,
                cancellationToken)
            : 0;

        var averageDepth = total > 0
            ? await depremler.AverageAsync(
                d => d.Derinlik,
                cancellationToken)
            : 0;

        var last24Hours = DateTime.Now.AddHours(-24);

        var earthquakesLast24Hours =
            await depremler
                .CountAsync(
                    d => d.Tarih >= last24Hours,
                    cancellationToken);

        var earthquakesMagnitude4Plus =
            await depremler
                .CountAsync(
                    d => d.Buyukluk >= 4,
                    cancellationToken);

        return new AnalyticsDto
        {
            TotalEarthquakes = total,
            AverageMagnitude = Math.Round(averageMagnitude, 2),
            MaximumMagnitude = Math.Round(maximumMagnitude, 2),
            AverageDepth = Math.Round(averageDepth, 2),
            EarthquakesLast24Hours = earthquakesLast24Hours,
            EarthquakesMagnitude4Plus = earthquakesMagnitude4Plus
        };
    }
}