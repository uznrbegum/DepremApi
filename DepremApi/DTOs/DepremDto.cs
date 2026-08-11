namespace DepremApi.DTOs;

using System.Text.Json;
using System.Text.Json.Serialization;

// DTO = Data Transfer Object
public class DepremDto
{
    [JsonPropertyName("location")]
    public string? Konum { get; set; }

    [JsonPropertyName("magnitude")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public double Buyukluk { get; set; }

    [JsonPropertyName("depth")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public double Derinlik { get; set; }

    [JsonPropertyName("date")]
    public string? Tarih { get; set; }
}