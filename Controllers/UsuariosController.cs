using AppMultiUsos.DTOs.DTOsUsuarios;
using CalculadoraDePrestamos.Data;
using EcommerceAPI.DTOs.GoogleDTO;
using EcommerceAPI.Modelos;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppMultiUsos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<UsuariosController> logger;
        private readonly ApplicationDbContext dbContext;

        public UsuariosController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,ILogger<UsuariosController> logger, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var usuario = new IdentityUser
            {
                Email = credencialesUsuarioDTO.Email,
                UserName = credencialesUsuarioDTO.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(usuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario == null)
            {
                return BadRequest("Login Incorrecto");
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario,
                credencialesUsuarioDTO.Password, lockoutOnFailure: false);


            if (resultado.Succeeded)
            {
                return await ConstruirToken(usuario);

            }
            else
            {
                return BadRequest("Error al logear");

            }
        }

        //El token puede dar problemas por ser corto despues ojo*
        private async Task<RespuestaAutenticacionDTO> ConstruirToken(IdentityUser identityUser)
        {
            var claims = new List<Claim>
            {
              new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim("email",identityUser.Email!),
                //new Claim(JwtRegisteredClaimNames.Sub,identityUser.Email!),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),


            };


            var claimsDB = await userManager.GetClaimsAsync(identityUser);


            //Agregando ClaimsDB A Claims
            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                (configuration["llavejwt"]!));

            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion
            };
        }




    //    [AllowAnonymous]
    //    [HttpPost("google-login")]
    //    public async Task<ActionResult<RespuestaAutenticacionCompletaDTO>> GoogleLogin(
    //[FromBody] GoogleLoginRequestDTO request)
    //    {
    //        try
    //        {
    //            // Validar token de Google
    //            var payload = await GoogleJsonWebSignature.ValidateAsync(
    //                request.Token,
    //                new GoogleJsonWebSignature.ValidationSettings
    //                {
    //                    //Solo aceptamos tokens para ClienteID
    //                    Audience = new[] { configuration["GoogleClientId"] }
    //                });

    //            // Buscar usuario por email o GoogleId
    //            var user = await userManager.FindByEmailAsync(payload.Email);

    //            if (user == null)
    //            {
    //                // Crear nuevo usuario si no existe
    //                user = new IdentityUser
    //                {
    //                    Email = payload.Email,
    //                    UserName = payload.Email,
    //                    EmailConfirmed = true // Google ya confirmó el email
    //                };

    //                var result = await userManager.CreateAsync(user);
    //                if (!result.Succeeded)
    //                {
    //                    return BadRequest("Error al crear usuario: " +
    //                        string.Join(", ", result.Errors.Select(e => e.Description)));
    //                }

    //                // Agregar claim de GoogleId para referencia futura
    //                await userManager.AddClaimAsync(user, new Claim("GoogleId", payload.Subject));

    //                // Hacerlo cliente automáticamente
    //                await userManager.AddClaimAsync(user, new Claim("Cliente", "True"));
    //            }
    //            //Si ya existe en la base de datos
    //            else
    //            {
    //                // Verificar si ya tiene claim de GoogleId
    //                var existingClaims = await userManager.GetClaimsAsync(user);
    //                if (!existingClaims.Any(c => c.Type == "GoogleId"))
    //                {
    //                    await userManager.AddClaimAsync(user, new Claim("GoogleId", payload.Subject));
    //                }
    //            }

    //            // Generar JWT (usa  método existente ConstruirToken)
    //            var authResponse = await ConstruirToken(user);

    //            // Obtener claims del usuario
    //            var claims = await userManager.GetClaimsAsync(user);
    //            var roles = claims.Where(c => c.Type == "Admin" || c.Type == "Vendedor" || c.Type == "Cliente")
    //                             .Select(c => $"{c.Type}: {c.Value}")
    //                             .ToList();

    //            var perfil = new PerfilUsuario
    //            {
    //                UsuarioId = user.Id,
    //                Email = user.Email,
    //                DireccionEnvio = "", // o null
    //                Telefono = null,
    //                NombreCompleto = null
    //            };

    //            await dbContext.PerfilesUsuarios.AddAsync(perfil);
    //            await dbContext.SaveChangesAsync();


    //            return new RespuestaAutenticacionCompletaDTO
    //            {
    //                Token = authResponse.Token, //El JWT
    //                Expiracion = authResponse.Expiracion,
    //                Usuario = new UserInfoDTO
    //                {
    //                    Email = user.Email!,
    //                    Name = payload.Name ?? user.Email!.Split('@')[0],
    //                    Picture = payload.Picture, //Foto de Google
    //                    GoogleId = payload.Subject // Id de Google
    //                }
    //            };
    //        }
    //        catch (InvalidJwtException ex)
    //        {
    //            logger.LogWarning("Token de Google inválido: {Message}", ex.Message);
    //            return Unauthorized(new { mensaje = "Token de Google inválido o expirado" });
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "Error en Google Login");
    //            return StatusCode(500, "Error interno del servidor");
    //        }
    //    }
    }
}
