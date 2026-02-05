using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IAvisoSinRespuestaRepository
    {
        Task<ResultTransaction> AddAsync(Autorizacion entityAutorizaciones, AvisoSinRespuesta entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultTransaction> UpdateAsync(Autorizacion entityAutorizaciones, AvisoSinRespuesta entityAviso, List<string> folios, Documento entityDocumento, DataFile dataFile);
        Task<List<ResponseAvisoSinRespuestaRelacionado>> GetAutorizacionRelacionada(int idA);
        Task<AvisoSinRespuesta> GetById(int id);
        Task<AvisoSinRespuesta> GetByIdAutorizacion(int idAutorizacion);
        Task<ResponseAvisoSinRespuestaRelacionado> GetByIdDisconnected(int id);
        Task<ResponseAvisoSinRespuestaRelacionado> GetByIdAutorizacionDisconnected(int id);
        Task<ResponseAvisoSinRespuestaRelacionado> GetByIdAsuntoDisconnected(int id);
        Task<List<AvisoSinRespuestaFolio>> GetFolios(int[] idAvisoSinRespuesa);
    }
}
