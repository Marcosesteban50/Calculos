using CalculadoraDePrestamos.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AppMultiUsos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculadoraController : ControllerBase
    {

        [HttpPost]
        public IActionResult Calcular(CalculadoraDTO dto)
        {
            decimal resultado;

            switch (dto.Operacion)
            {
                case "+":
                    resultado = dto.Numero1 + dto.Numero2;
                    break;

                case "-":
                    resultado = dto.Numero1 - dto.Numero2;
                    break;

                case "*":
                    resultado = dto.Numero1 * dto.Numero2;
                    break;

                case "/":

                    if (dto.Numero2 == 0)
                        return BadRequest("No se puede dividir entre cero.");

                    resultado = dto.Numero1 / dto.Numero2;
                    break;

                default:
                    return BadRequest("Operación inválida.");
            }

            return Ok(resultado);
        }
    }
}
