using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface ICumplimentacionRepository
    {
        Task<ResultTransaction> AddCumplimentacionAsync(Autorizacion entity, Cumplimentacion entityCumplimentacion);

        Task<ResultTransaction> UpdateAsync(Autorizacion entity, Cumplimentacion entityCumplimentacion);

        Task<ResponseCumplimentacionOficialPartesById> GetCumplimentacionByIdDisconnectedAsync(int id);

        Task<Cumplimentacion> GetCumplimentacionByIdAsync(int? id, int? idAutorizacion);

        Task<ResultTransaction> TurnarCumplimentacionAsync(Cumplimentacion entity);

        Task<ResponseCumplimentacionById> GetCumplimentacionDisconnectedAsync(int id);

        Task<ResultTransaction> UpdateCumplimentacionAsync(Autorizacion entity, Cumplimentacion entityCumplimentacion);

        Task<ResponseResolucionCumplimentacion> GetResolucionCumplimentacionDisconnectedAsync(int id);

        Task<List<ResponseResolucionCumplimentacion>> GetResolucionCumplimentacions(int idCumplimentacion);

        Task<ResultTransaction> UpdateImprocedenciaAsync(Cumplimentacion entityCumplimentacion, Documento entityDocumento, DataFile dataFile);
    }
}
