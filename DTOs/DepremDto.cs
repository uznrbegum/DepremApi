namespace DepremApi.DTOs;
using System.Text.Json; //JSONı okuyabilmek için
using System.Text.Json.Serialization; //JSON & c# eşleştirmek için

//DTO=Data Transfer Object
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