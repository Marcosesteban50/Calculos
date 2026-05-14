using AppMultiUsos.DTOs.DTOsUsuarios;


namespace EcommerceAPI.DTOs.GoogleDTO
{
    public class RespuestaAutenticacionCompletaDTO : RespuestaAutenticacionDTO
    {
        public UserInfoDTO Usuario { get; set; } = new();
    }
}
