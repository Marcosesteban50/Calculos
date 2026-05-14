using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CalculosApp.Validaciones;
using CalculadoraDePrestamos.Validaciones;

namespace AppMultiUsos.Modelos.CalculadoraDivisas
{
    public class CalcularDivisas
    {

        [Required(ErrorMessage = "El {0} es requerido")]
        [RangoDivisas]
        [DefaultValue(100)]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El {0} es requerido")]
        [DefaultValue("USD")]
        public string? Desde { get; set; }

        [Required(ErrorMessage = "El {0} es requerido")]
        [DefaultValue("EUR")]
        public string? Hacia { get; set; }
    }
}
