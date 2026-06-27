using System.Text;
using System.Text.Json;

namespace CalculosApp.Servicios.IA
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;


        public GeminiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

        }

        public async Task<string> GenerarRecomendacionPrestamo()
        {
            //Llave de el api
            var apiKey = _config["Gemini:ApiKey"];

            //Url de la ia
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            //Prompt de funcionamiento de la Ia
            var prompt = @"
Eres un asesor financiero.

Devuelve SOLO un JSON válido sin texto adicional con esta estructura:

{
  ""monto"": number,
  ""plazoAnios"": number,
  ""tasaInteres"": number
}

Reglas:
- monto entre 1000 y 500000
- plazo entre 1 y 5 años
- tasa entre 8 y 18
";

            //Body de mensaje a la IA en JSON
            var body = new
            {
                //contenido
                contents = new[]
                {
           // Respuesta Sistema
            //Respuesta de Ia ante peticion del prompt forzamos al modelo de IA  a aceptar las reglas del prompt
        new
        {

            role = "model",
            parts = new[] { new { text = "Entendido. Solo asistiré con rellenar los datos numericos para calculos de prestamos." } }
        },
        new
        {
            role = "user",
            parts = new[] { new { text = prompt } }
        },
                }
            };

            //Convertimos a json
            var json = JsonSerializer.Serialize(body);


            //respuesta de la Ia
            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            //resultado
            var result = await response.Content.ReadAsStringAsync();


           

            // Debug
            Console.WriteLine("===== STATUS =====");
            Console.WriteLine(response.StatusCode);

            Console.WriteLine("===== RESPUESTA GEMINI =====");
            Console.WriteLine(result);
            Console.WriteLine(apiKey);

            //parseamos el resultado a Json
            using var doc = JsonDocument.Parse(result);

           
                // verificando si gemini respondio sin errores
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    return candidates[0]
                        
                        //mensajes de chat = content
                        .GetProperty("content")
                        //Contenido del mensaje = parts
                        .GetProperty("parts")[0]
                        //Mensaje como tal
                        .GetProperty("text")
                        //Convertimos estos JSOn a string
                        .GetString()!;
                }

            



            return "No response generated or content was blocked.";

        }


        public async Task<string> PreguntarIA(string pregunta)
        {

            var _systemPrompt = @"
Eres el asistente inteligente de 'CalculosApp'. Solo tienes permitido ayudar con:
1. Calculadora General: Operaciones matemáticas básicas.
2. Calculadora de Divisas: Conversión de monedas.
3. Préstamos: Cálculos de cuotas, intereses y gestión de préstamos de usuarios.


Si el usuario pregunta algo que no esté relacionado con estos temas, responde educadamente 
que solo estás capacitado para asistir en las funciones de CalculosApp.";

            //Llave
            var apiKey = _config["Gemini:ApiKey"];


            //Url de Gemini IA
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";




            //Body de mensaje a la IA en JSON
            var body = new
            {
                contents = new[]
             {
        // Respuesta Sistema
        new
        {
            role = "user",
            parts = new[] { new { text = _systemPrompt } }
        },
        //Respuesta de Ia ante peticion del prompt forzamos al modelo de IA  a aceptar las reglas del prompt
        new
        {

            role = "model",
            parts = new[] { new { text = "Entendido. Solo asistiré con las funciones de Calculadora, Divisas, Préstamos." } }
        },
        // 2. La pregunta real del usuario
        new
        {
            //Mandamos lo que escribio el usuario
            role = "user",
            parts = new[] { new { text = pregunta } }
        }
    }
            };

            //Convirtiendo a JsonString el Body
            var json = JsonSerializer.Serialize(body);

            //Enviando la peticion
            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            //Leyendo la respuesta como string asyncronamente
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

            //Convirtiendo a Json document  el resultado
            using var doc = JsonDocument.Parse(result);

            // verificando si gemini respondio sin errores
            if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                return candidates[0]
                    //mensajes de chat = content
                    .GetProperty("content")
                    //Contenido del mensaje = parts
                    .GetProperty("parts")[0]
                    //Mensaje como tal
                    .GetProperty("text")
                    //Convertimos estos JSOn a string
                    .GetString()!;
            }

            return "No response generated or content was blocked.";
        }
    }
}
