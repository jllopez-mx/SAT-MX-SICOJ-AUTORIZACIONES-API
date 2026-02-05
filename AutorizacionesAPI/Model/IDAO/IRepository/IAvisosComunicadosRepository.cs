using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IAvisosComunicadosRepository
    {
        Task<ResultTransaction> AddAsync(AvisosComunicados entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultTransaction> UpdateAsync( AvisosComunicados entity);
        Task<AvisosComunicados> GetById(int id);
        Task<ResponseAvisosComunicados> GetDisconnectedById(int id);
        Task<List<ResponseAvisosComunicados>> GetDisconnectedByIdAutorizacion(int idAutorizacion);
    }
}
