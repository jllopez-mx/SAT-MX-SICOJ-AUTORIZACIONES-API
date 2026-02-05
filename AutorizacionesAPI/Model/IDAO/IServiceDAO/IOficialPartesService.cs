using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IServiceDAO
{
    public interface IOficialPartesService
    {
        Task<ResultOperation> GetBandejaPendientesAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, UserInformationView userInformationView);
        Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, 
            string? noAsunto, DateTime? fechaDesde, DateTime? fechaHasta, string? rfc, string? promovente, 
            List<int>? TipoAsunto, List<int>? EstadoTarea, List<int>? TipoModalidad, List<int>? EstadoProcesal, UserInformationView userInformationView);
        Task<ResultOperation> GetAutorizacionByIdDisconnected(int id);
        Task<ResultOperation> GetImpuestosInternosByIdDisconnected(int id);
        Task<ResultOperation<int>> AddAutorizacionFisico(Autorizacion entity, UserInformationView userInformationView);
        Task<ResultOperation<int>> UpdateAutorizacion(Autorizacion entity, UserInformationView userInformationView);
        Task<ResultOperation<ResponseTurnado>> TurnarAsync(Autorizacion entity);
        Task<ResultOperation<bool>> DeleteAutorizacionAsync(Autorizacion entity);

        #region Cumplimentacion
        Task<ResultOperation> AddCumplimentacionAsync(Autorizacion entityAutorizacion, Cumplimentacion entityCumplimentacion, UserInformationView userInformationView);

        Task<ResultOperation<int>> UpdateCumplimentacion(Autorizacion entityAutorizacion, Cumplimentacion entityCumplimentacion, UserInformationView userInformationView);
        
        Task<ResultOperation> GetCumplimentacionByIdDisconnectedAsync(int id);

        Task<ResultOperation> GetCumplimentacionBuscarNoAsuntoAsync(string noAsunto, UserInformationView userInformationView);

        Task<Autorizacion> GetAutorizacionByNoAsunto(string noAsunto);

        Task<ResultOperation<ResponseTurnado>> TurnarCumplimentacionAsync(Cumplimentacion entity);

        #endregion
    }
}
