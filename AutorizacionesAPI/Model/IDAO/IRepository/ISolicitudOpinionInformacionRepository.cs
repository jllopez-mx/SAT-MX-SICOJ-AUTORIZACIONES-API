using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;


namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface ISolicitudOpinionInformacionRepository
    {
        Task<ResultTransaction> AddComercioExteriorSolicitudOpinionInformacionAsync(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile, Autorizacion entityComercio);
        Task<ResultTransaction> UpdateComercioExteriorSolicitudOpinioninInformacionAsync(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile);
        Task<ResponseSolicitudOpinionInformacionAbogadoById> GetComercioExteriorSolicitudOpinionInformacionByIdAsync(int id);
        Task<SolicitudOpinionInformacion> GetComercioExteriorSolicitudOpinionByIdAsync(int id);
        Task<List<ResponseSolicitudOpinionInformacionAbogadoById>> GetComercioExteriorSolicitudOpinionInformacionByIdsAsync(int ids);
        Task<ResultTransaction> UpdateComercioExteriorNoSolicitudOpinionInformacionAsync(Autorizacion entity);
       
    }
}
