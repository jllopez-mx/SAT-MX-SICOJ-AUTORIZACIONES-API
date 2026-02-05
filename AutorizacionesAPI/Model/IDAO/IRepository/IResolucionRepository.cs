using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IResolucionRepository
    {
        Task<ResultTransaction> AddAsync(Autorizacion entityAutorizaciones, Cumplimentacion entityCumplimentacio, Resolucion entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultTransaction> UpdateAsync(Autorizacion entityAutorizaciones, Cumplimentacion entityCumplimentacio, Resolucion entity, Documento entityDocumento, DataFile dataFile, int idSeccion);
        Task<List<Resolucion>> GetByIdAusunto(int? idAutorizacion, int? idCumplimentacion);
        Task<List<ResponseResolucion>> GetByIdAsuntoDisconnected(int? idAutorizacion, int? idCumplimentacion);
        Task<List<ResponseAvisoConRespuesta>> GetAvisoConRespuestaDisconnected(int idAutorizacionRelacionada);
        Task<ResponseAvisoConRespuesta> GetAvisoConRespuestaByIdDisconnected(int id);
        Task<ResultTransaction> DescartarAsync(Autorizacion entityAutorizaciones, Descartar entity, List<int>? listaSecciones);
    }
}
