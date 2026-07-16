using System.ComponentModel.DataAnnotations;

namespace CalculadoraDePrestamos.DTOs
{
    public class CalculadoraDTO
    {
        public decimal Numero1 { get; set; }
        public decimal Numero2 { get; set; }
        public string Operacion { get; set; } = "";
    }
}
