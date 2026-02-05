using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IRequerimientoRepository
    {
        Task<ResultTransaction> AddAsync(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultTransaction> UpdateAsync(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile, int idSeccion);
        Task<Requerimiento> GetById(int id);
        Task<ResponseRequerimiento> GetByIdDisconnected(int id);
        Task<List<ResponseRequerimiento>> GetByIdAutorizacionDisconnected(int idAutorizacion);
        Task<List<Requerimiento>> GetByIdAutorizacion(int id);
        Task<ResultTransaction> DescartarAsync(Autorizacion entityAutorizaciones, Descartar entity, List<int>? listaSecciones);
        Task<ResultTransaction> DescartarUltimoAsync(Requerimiento entity, int idSeccionRequerimiento);
    }
}
