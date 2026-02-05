using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IAvisoConRespuestaRepository
    {
        Task<AvisoConRespuesta> GetByIdAutorizacion(int idAutorizacion);
    }
}
