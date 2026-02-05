using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IServiceDAO
{
    public interface IAbogadoService
    {
        Task<ResultOperation> GetBandejaPendientesAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string? noAsunto,
            DateTime? fechaPresentacionDesde,
            DateTime? fechaPresentacionHasta,
            DateTime? fechaVencimientoDesde,
            DateTime? fechaVencimientoHasta,
            string? rfc,
            string? promovente,
            List<int>? temaList,
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            UserInformationView userInformationView);
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
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            List<int>? alertaList,
            UserInformationView userInformationView);
        Task<ResultOperation> GetComercioExteriorByIdDisconnected(int id);
        Task<ResultOperation> GetImpuestosInternosByIdDisconnected(int id);

    }
}