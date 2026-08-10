namespace DepremApi.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DepremDto
{
    [JsonPropertyName("location")] //JSON'daki "location" alanını bendeki Konum ile eşler
    public string? Konum { get; set; }

    [JsonPropertyName("magnitude")]
    public JsonElement Buyukluk { get; set; }

    [JsonPropertyName("depth")]
    public JsonElement Derinlik { get; set; }

    [JsonPropertyName("date")]
    public JsonElement Tarih { get; set; }
}