using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IServiceDAO
{
    public interface IAdministradorUnidadCentralService
    {
        Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
            string? noAsunto, DateTime? fechaDesde, DateTime? fechaHasta, string? rfc, string? promovente,
            List<int>? TipoAsunto, List<int>? EstadoTarea, List<int>? TipoModalidad, List<int>? EstadoProcesal, UserInformationView userInformationView);

    }
}
