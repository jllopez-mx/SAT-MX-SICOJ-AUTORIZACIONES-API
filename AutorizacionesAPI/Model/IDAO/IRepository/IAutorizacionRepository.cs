using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IAutorizacionRepository
    {
        Task<ResponseAutorizacionGenericById> GetDisconnectedById(int id);
        Task<Autorizacion> GetByIdAsync(int id);
        Task<Autorizacion> GeAutorizacionAsync(int? id);
        Task<List<Autorizacion>> GetListByIdsAsync(int[] ids);
        Task<Autorizacion> GetByNumeroAsuntoAsync(string numeroAsunto);
        Task<List<Autorizacion>> GetListByNumerosAsuntoAsync(string[] numerosAsunto);
        Task<ResultTransaction> AddAsync(Autorizacion entity);
        Task<ResultTransaction> TurnarAsync(Autorizacion entity);
        Task<ResultTransaction> DeleteAsync(Autorizacion entity);
        Task<ResultTransaction> AsignarAsync(Autorizacion entity, Cumplimentacion cumplimentacion, Abogado entityAbogado);
        Task<ResultTransaction> RemitirAsync(Autorizacion entity, Remision entityRemision, Documento entityDocumento, DataFile dataFile);
        Task<ResultTransaction> ReasignarAsync(List<Reasignar> listReasignacion);
        Task<ResultTransaction> UpdateAsync(Autorizacion entity, AvisoSinRespuesta avisoSinRespuesta, AvisoConRespuesta avisoConRespuesta, int? idSeccion = null!);
        Task<ResultTransaction> UpdateNoRequerimientoAsync(Autorizacion entity);
        Task<ResultTransaction> ConcluirAsync(Autorizacion entity, Cumplimentacion cumplimentacion);
        Task<ResultTransaction> ReactivarAsync(Autorizacion autorizacion, Cumplimentacion cumplimentacion, Abogado entityAbogado, Reactivar entityReactivar);
        Task<ResultTransaction> RechazarAvisoSinRespuestaAsync(Autorizacion entity, int[] secciones);
        Task<ResultTransaction> AprobarAvisoSinRespuestaAsync(Autorizacion entity, AvisoSinRespuesta entityAviso);
        Task<List<ResponseNumeroAsunto>> GetListByNoAsuntoAsync(string noAsunto, int idTipoAsunto);
        Task<Autorizacion> GetAvisoNoAsuntoExternoAsync(string? noAsunto);
    }
}
