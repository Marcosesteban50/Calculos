using CalculosApp.DTOs.GoogleDTO;
using CalculosApp.Servicios.IA;
using Microsoft.AspNetCore.Mvc;

namespace CalculosApp.Controllers
{

    [ApiController]
    [Route("api/[Controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiService _gemini;

        public GeminiController(GeminiService gemini)
        {
            _gemini = gemini;
        }

        [HttpPost("preguntar")]
        public async Task<IActionResult> Preguntar([FromBody] PreguntaDTO pregunta)
        {
            var respuesta = await _gemini.PreguntarIA(pregunta.Pregunta);
            return Ok(respuesta);
        }

        [HttpGet("recomendar")]
        public async Task<IActionResult> ObtenerRecomendacion()
        {
            var respuesta = await _gemini.GenerarRecomendacionPrestamo();
            return Ok(respuesta);
        }
    }
}
