
using AppMultiUsos.Modelos.CalculadoraDivisas;
using AutoMapper;
using CalculadoraDePrestamos.DTOs;
using CalculadoraDePrestamos.DTOs.DTOsUsuarios;

using CalculadoraDePrestamos.Modelos.CalculadoraPrestamos;
using CalculadoraDePrestamos.Modelos.CalculadoraPrestamosUsuarios;


namespace CalculadoraDePrestamos.Utilidades
{
    public class AutoMapperProfiles : Profile
    {



        public AutoMapperProfiles()
        {
            SolicitudPrestamo();
            SolicitudPrestamoUsuario();

        }


        private void SolicitudPrestamo()
        {
            CreateMap<SolicitudPrestamoDTO, SolicitudPrestamo>();
            CreateMap<SolicitudPrestamo,SolicitudPrestamoDTO>();
        } 


        private void SolicitudPrestamoUsuario()
        {
            CreateMap<SolicitudPrestamoUsuarioCreacionDTO, SolicitudPrestamoUsuario>();
            CreateMap<SolicitudPrestamoUsuario,SolicitudPrestamoUsuarioDTO>();
           
        }

        
      


    }
}
