using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IModificacionRepository
    {
        Task<ResultTransaction> AddAsync(Autorizacion entityAutorizacion, Modificacion entity);
        Task<List<Modificacion>> GetByIdAsuntoAsync(int? idAutorizacion, int? idCumplimentacion);
    }
}
