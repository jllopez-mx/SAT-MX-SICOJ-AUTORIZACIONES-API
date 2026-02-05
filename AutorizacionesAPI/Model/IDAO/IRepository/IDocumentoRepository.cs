using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IDocumentoRepository
    {
        Task<ResultTransaction> AddAsync(Documento entity, DataFile dataFile);
        Task<ResultTransaction> UpdateAsync(Documento entityDocumento, DataFile dataFile);
        Task<List<Documento>> GetByIdsAsync(int[] id);
        Task<List<ResponseDocumentosConFolio>> GetDocumentosFolioDisconnected(bool paginado, int? idAutorizacion, int? idCumplimentacion, List<int>? idSeccion, int? idRenglonSeccion, bool activo, int? Fetch = null, int? Page = null!, string? OrderByColumn = null!, bool? OrderDesc = null!);
        Task<int?> GetDocumentosFolioDisconnectedCount(int? idAutorizacion, int? idCumplimentacion, List<int>? idSeccion, int? idRenglonSeccion, bool activo);
        Task<List<ResponseDocumentoHistorico>> GetDocumentosHistoricoDisconnected(bool paginado, int? idAutorizacion, int? idCumplimentacion, List<int>? idSeccion, int? idRenglonSeccion, bool activo, int? Fetch = null, int? Page = null!, string? OrderByColumn = null!, bool? OrderDesc = null!);
        Task<ResultTransaction> DeleteAsync(int[] ids, string usuarioModificacion);
        Task<List<Documento>> GetDocumentosByRenglonTipoAsync(int? idAutorizacion, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion, int? idTipoDocumento);
    }
}