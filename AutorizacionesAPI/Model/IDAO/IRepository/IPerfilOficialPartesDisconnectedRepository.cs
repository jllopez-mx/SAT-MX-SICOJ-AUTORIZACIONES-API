using AutorizacionesAPI.Model.DTO;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IPerfilOficialPartesDisconnectedRepository
    {
        Task<List<ResponseAutorizacionesOficialPartesBandeja>> GetBandejaPendientesAsync(
            int pageSize,
            int page,            
            string? orderByColumn,
            bool orderDesc,
            List<int>? TipoAsunto,
            int? idUnidadAdmistrativaCentral = null!
        );

        Task<int?> GetBandejaPendientesCountAsync(
            List<int>? TipoAsunto, int? idUnidadAdmistrativaCentral = null!
            );

        Task<List<ResponseAutorizacionesOficialPartesHistorico>> GetHistoricoAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string? noAsunto,
            DateTime? desdeRecepcion,
            DateTime? hastaRecepcion,
            string? rfc,
            string? promovente,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativaCentral = null!
        );

        Task<int?> GetHistoricoCountAsync(
            string? noAsunto,
            DateTime? desdeRecepcion,
            DateTime? hastaRecepcion,
            string? rfc,
            string? promovente,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativaCentral = null!
        );
    }
}
