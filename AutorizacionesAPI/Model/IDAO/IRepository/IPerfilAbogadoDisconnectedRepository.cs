using AutorizacionesAPI.Model.DTO;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IPerfilAbogadoDisconnectedRepository
    {
        Task<int?> GetBandejaPendientesCountAsync(
            string? noAsunto,
            DateTime? fechaPresentacionDesde,
            DateTime? fechaPresentacionHasta,
            DateTime? fechaVencimientoDesde,
            DateTime? fechaVencimientoHasta,
            string? rfc,
            string? promovente,
            List<int>? idTema,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativaCentral = null!,
            int? idUnidadAdmistrativa = null!,
            int? idSubadministracion = null!,
            string? idAbogado = null!);

        Task<List<ResponseAutorizacionesAbogadoBandeja>> GetBandejaPendientesAsync(
            int pageSize,
            int page,
            string? OrderByColumn,
            bool OrderDesc,
            string? noAsunto,
            DateTime? fechaPresentacionDesde,
            DateTime? fechaPresentacionHasta,
            DateTime? fechaVencimientoDesde,
            DateTime? fechaVencimientoHasta,
            string? rfc,
            string? promovente,
            List<int>? idTema,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativaCentral = null!,
            int? idUnidadAdmistrativa = null!,
            int? idSubadministracion = null!,
            string? idAbogado = null!
        );

        Task<int?> GetHistoricoCountAsync(
           string? noAsunto,
           DateTime? fechaPresentacion,
           DateTime? fechaVencimiento,
           string? rfc,
           string? promovente,
           List<int>? tipoAsuntoList,
           List<int>? estadoTareaList,
           List<int>? tipoModalidadList,
           List<int>? estadoProcesalList,
           List<int>? alertaList,
           int? idUnidadAdmistrativaCentral = null!,
           int? idUnidadAdmistrativa = null!
       );

        Task<List<ResponseAutorizacionesAbogadoHistorico>> GetHistoricoAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string? noAsunto,
            DateTime? fechaPresentacion,
            DateTime? fechaVencimiento,
            string? rfc,
            string? promovente,
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            List<int>? alertaList,
            int? idUnidadAdmistrativaCentral = null!,
            int? idUnidadAdmistrativa = null!
        );
    }
}
