
using AppMultiUsos.Modelos.CalculadoraDivisas;
using AppMultiUsos.Servicios.CalculadoraDivisas;
using AutoMapper;
using CalculosApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace AppMultiUsos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculadoraDivisasController : Controller
    {
        private readonly IMapper mapper;
        private readonly iCalculadoraDivisas calculadoraDivisas;
        private readonly HttpClient httpClient;

        public CalculadoraDivisasController(IMapper mapper, iCalculadoraDivisas calculadoraDivisas,
            HttpClient httpClient)
        {
            this.mapper = mapper;
            this.calculadoraDivisas = calculadoraDivisas;
            this.httpClient = httpClient;
        }



        [HttpPost]
        public IActionResult Post([FromBody] CalcularDivisas modelo)
        {



            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }


            try
            {

                var calculo = calculadoraDivisas.Operar(modelo.Monto, modelo.Desde!, modelo.Hacia!);

                return Ok(new { total = calculo });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }


        


            [HttpGet("monedas")]
            public async Task<ActionResult> Monedas()
        {
            var url = $"https://api.frankfurter.dev/v2/currencies";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { mensaje = "Error obteniendo monedas" });

            }

            var resultado = await response.Content.ReadFromJsonAsync<List<CurrencyDTO>>();



            return Ok(resultado);
        }

        [HttpGet("convertir")]
        public async Task<ActionResult> Convertir(string from, string to, decimal amount)
        {
            // URL de Frankfurter
            var url =
                $"https://api.frankfurter.dev/v2/rate/{from}/{to}";

            // Llamamos API
            var response =
                await httpClient.GetAsync(url);

            // Si falla
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { mensaje = "Error obteniendo tasa" });
            }

            // Convertimos JSON → objeto C#
            var resultado = await response.Content.ReadFromJsonAsync<FrankfurterRateDTO>();

            // Obtenemos tasa
            var tasa = resultado!.Rate;

            // Hacemos cálculo
            var total = amount * tasa;

            // Retornamos datos
            return Ok(new
            {
                MonedaOrigen = from,
                MonedaDestino = to,
                Cantidad = amount,
                Tasa = tasa,
                Total = total
            });
        }



    }
}
