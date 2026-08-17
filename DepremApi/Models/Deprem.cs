//DTO değil Entity

namespace DepremApi.Models;

public class Deprem
{
    public string? EventId { get; set; }

    public int Id { get; set; }

    public string? Konum { get; set; }

    public double Buyukluk { get; set; }

    public double Derinlik { get; set; }

    public DateTime Tarih { get; set; }
}