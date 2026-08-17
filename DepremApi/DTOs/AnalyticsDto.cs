namespace DepremApi.DTOs;

public class AnalyticsDto
{
    public int TotalEarthquakes { get; set; }

    public double AverageMagnitude { get; set; }

    public double MaximumMagnitude { get; set; }

    public double AverageDepth { get; set; }

    public int EarthquakesLast24Hours { get; set; }

    public int EarthquakesMagnitude4Plus { get; set; }
}