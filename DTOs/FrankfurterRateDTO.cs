namespace CalculosApp.DTOs
{
    public class FrankfurterRateDTO
    {
        public string Date { get; set; } = string.Empty;

        public string Base { get; set; } = string.Empty;

        public string Quote { get; set; } = string.Empty;

        public decimal Rate { get; set; }
    }
}
