using AutorizacionesAPI.Model.DTO;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IPerfilAdministradorGlobalDisconnectedRepository
    {
        Task<int?> GetHistoricoCountAsync(
            string? noAsunto,
            DateTime? fechaPresentacion,
            DateTime? fechaVencimiento,
            string? rfc,
            string? promovente,
            List<int>? alerta,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativaCentral = null!,
            int? idUnidadAdmistrativa = null!
        );
        Task<List<ResponseAutorizacionesAdministradorGlobalHistorico>> GetHistoricoAsync(
           int pageSize,
           int page,
           string? OrderByColumn,
           bool OrderDesc,
           string? noAsunto,
           DateTime? fechaPresentacion,
           DateTime? fechaVencimiento,
           string? rfc,
           string? promovente,
           List<int>? alerta,
           List<int>? TipoAsunto,
           List<int>? EstadoTarea,
           List<int>? TipoModalidad,
           List<int>? EstadoProcesal,
           int? idUnidadAdmistrativaCentral = null!,
           int? idUnidadAdmistrativa = null!
        );
    }
}
