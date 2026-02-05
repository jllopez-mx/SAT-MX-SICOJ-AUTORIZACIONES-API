using AutorizacionesAPI.Model.DTO;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IPerfilAdministradorUnidadCentralDisconnectedRepository
    {
        Task<List<ResponseAutorizacionesAdministradorUAHistorico>> GetHistoricoAsync(
           int pageSize,
           int page,
           string? OrderByColumn,
           bool OrderDesc,
           string? noAsunto,
           DateTime? fechaPresentacion,
           DateTime? fechaVencimiento,
           string? rfc,
           string? promovente,
           List<int>? TipoAsunto,
           List<int>? EstadoTarea,
           List<int>? TipoEntrada,
           List<int>? EstadoProcesal,
           int? idUnidadAdmistrativa = null!
       );

        Task<int?> GetHistoricoCountAsync(
            string? noAsunto,
            DateTime? fechaPresentacion,
            DateTime? fechaVencimiento,
            string? rfc,
            string? promovente,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativa = null!
        );
    }
}
