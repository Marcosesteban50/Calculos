using System.Text.Json.Serialization;

namespace CalculosApp.DTOs
{
    public class CurrencyDTO
    {
        [JsonPropertyName("iso_code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("iso_numeric")]
        public string IsoNumeric { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
