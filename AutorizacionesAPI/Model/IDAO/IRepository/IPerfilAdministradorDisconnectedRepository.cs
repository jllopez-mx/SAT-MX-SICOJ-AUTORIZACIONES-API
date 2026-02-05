using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IPerfilAdministradorDisconnectedRepository
    {
        Task<int?> GetBandejaPendientesCountAsync(List<int>? TipoAsunto, int? idUnidadAdmistrativaCentral = null!, int? idUnidadAdmistrativa = null!);
        Task<List<ResponseAutorizacionesAdministradorBandeja>> GetBandejaPendientesAsync(
          int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
          List<int>? TipoAsunto, int? idUnidadAdmistrativaCentral = null!, int? idUnidadAdmistrativa = null!);
        Task<List<ResponseAutorizacionesAdministradorHistorico>> GetHistoricoAsync(
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
           List<int>? TipoEntrada,
           List<int>? EstadoProcesal,
           int? idUnidadAdmistrativaCentral = null!,
           int? idUnidadAdmistrativa = null!
       );

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
    }
}
