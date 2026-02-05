using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IServiceDAO
{
    public interface IAdministradorService
    {
        Task<ResultOperation> GetBandejaPendientesAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, UserInformationView userInformationView);
        Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
            string? noAsunto, DateTime? fechaDesde, DateTime? fechaHasta, string? rfc, string? promovente,
            List<int>? TipoAsunto, List<int>? EstadoTarea, List<int>? TipoModalidad, List<int>? alerta, List<int>? EstadoProcesal, UserInformationView userInformationView);
        Task<ResultOperation> GetComercioExteriorByIdDisconnected(int id);
        Task<ResultOperation> GetImpuestosInternosByIdDisconnected(int id);
        Task<ResultOperation<ResponseAsignar>> AsignarAsync(Autorizacion entity, Cumplimentacion cumplimentacion, Abogado entityAbogado);
        Task<ResultOperation<bool>> RemitirAsync(Autorizacion entity, Remision entityRemision, Documento entityDocumento, DataFile dataFile);
        Task<ResultOperation> GetRemisionAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int idComercioExterior);
        Task<ResultOperation<int>> RechazarAvisoSinRespuestaAsync(Autorizacion entity, List<int> secciones);
        Task<ResultOperation<int>> AprobarAvisoSinRespuestaAsync(Autorizacion entity, AvisoSinRespuesta entityAviso);
    }
}
