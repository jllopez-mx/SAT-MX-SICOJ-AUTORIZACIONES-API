using AutorizacionesAPI.Model.DTO;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IRemisionRepository
    {
        Task<int?> GetRemisionesDisconnectedCount(int idAutorizacionesComercioExterior);

        Task<List<ResponseRemision>> GetRemisionesDisconnected(
           int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int idAutorizacionesComercioExterior);
    }
}
