using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IServiceDAO
{
    public interface IAdministradorGlobalService
    {
        Task<Abogado> GetAbogadoByIdAutorizacionAsync(int? idAutorizacion, int? idCumplimentacion);

        Task<ResultOperation> GetHistoricoAsync(
            int Fetch,
            int Page,
            string? OrderByColumn,
            bool OrderDesc,
            string? noAsunto,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? rfc,
            string? promovente,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? alerta,
            List<int>? EstadoProcesal,
            UserInformationView userInformationView);

        Task<ResultOperation> GetComercioExteriorByIdDisconnected(int id);
        Task<ResultOperation> GetImpuestosInternosByIdDisconnected(int id);

        Task<ResultOperation<ResponseReactivar>> ReactivarAsync(Autorizacion autorizacion, Cumplimentacion cumplimentacion, Abogado entityAbogado, Reactivar entityReactivar);
        
        #region Resolución
        Task<ResultOperation<ResponseResolucionDescartar>> ResolucionDescartarAsync(Autorizacion entityAutorizaciones, Descartar entity, List<int>? listaSecciones);
        #endregion

        #region Requerimiento
        Task<ResultOperation<ResponseRequerimientoDescartar>> RequerimientoDescartarAsync(Autorizacion entityAutorizaciones, Descartar entity, List<int>? listaSecciones);

        Task<ResultOperation<ResponseRequerimientoDescartar>> RequerimientoDescartarUltimoAsync(Autorizacion entityAutorizaciones, Requerimiento entity, int idSeccionRequerimiento);
        #endregion
    }
}
