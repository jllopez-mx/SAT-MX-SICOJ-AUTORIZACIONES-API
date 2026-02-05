using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IRequerimientoProdeconRepository
    {
        Task<ResultTransaction> AddAsync(RequerimientoProdecon entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultTransaction> UpdateAsync(RequerimientoProdecon entity);
        Task<List<ResponseRequerimientoProdecon>> GetRequerimientoProdecon(int idAutorizacion);
        Task<ResponseRequerimientoProdecon> GetRequerimientoProdeconByIdDisconnected(int id);
        Task<RequerimientoProdecon> GetRequerimientoProdeconById(int id);
        Task<int> GetByIdAutorizacionCount(int idAutorizacion);
    }
}
