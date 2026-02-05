using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IServiceDAO
{
    public interface IGenericService
    {
        #region Consultas Asunto
        Task<Autorizacion> GetAutorizacionById(int id);
        Task<Autorizacion> GetAutorizacion(int? id);
        Task<ResultOperation<int>> UpdateAutorizacionAsync(Autorizacion entity, AvisoSinRespuesta avisoSinRespuesta, AvisoConRespuesta avisoConRespuesta, UserInformationView userInformationView, int? idSeccion = null);
        #endregion

        #region Documentos
        Task<List<Documento>> GetArchivosAutorizacionesByIds(int[] ids);
        Task<List<Documento>> GetDocumentosByRenglonTipoAsync(int? idAutorizacio, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion, int? idTipoDocumento);
        Task<ResultOperation<ResponseDocumentoCreate>> AddArchivo(Documento entity, DataFile dataFile);
        Task<ResultOperation<int>> UpdateArchivo(Documento entity, DataFile dataFile);
        Task<ResultOperation<bool>> DeleteArchivo(int[] ids, string usuarioModificacion);
        Task<ResultOperation> GetDocumentosAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAutorizacio, int? idCumplimentacion);
        Task<ResultOperation> GetDocumentosSeccionPaginadoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAutorizacio, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion);
        Task<ResultOperation> GetDocumentosSeccionAsync(int? idAutorizacio, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion);
        #endregion

        #region Reasignar
        Task<ResultOperation<ResponseReasignar>> ReasignarAsync(int[] idList, UserInformationView userInformationView, string rfcAbogado = null!, string rfcAdministrador = null!);
        #endregion

        #region Modificacion
        Task<ResultOperation<int>> AddModificacionAsync(Autorizacion entityAutorizacion, Modificacion entity);
        Task<List<Modificacion>> GetModificacionByIdAsunto(int? idAutorizacion, int? idCumplimentacion);
        #endregion

        #region Descartar
        Task<ResultOperation<int>> DescartarAsync(Autorizacion autorizacion, Cumplimentacion cumplimentacion, Descartar descartar, List<int>? listaSecciones);
        Task<List<Descartar>> GetDescartarByIdAsunto(int? idAutorizacion, int? idCumplimentacion);
        #endregion

        #region Aviso Sin Respuesta
        Task<ResultOperation> GetAvisoSinRespuestaRelacionadoAsync(int id);
        Task<AvisoSinRespuesta> GetAvisoSinRespuestaById(int id);
        Task<AvisoSinRespuesta> GetAvisoSinRespuestaByIdAutorizacion(int idAutorizacion);
        Task<ResultOperation> GetAvisoSinRespuestaByIdDisconnected(int id);
        Task<ResultOperation> GetAvisoSinRespuestaPendienteByIdAuntoDisconnected(int id);
        Task<ResultOperation> GetAvisoSinRespuestaByIdAsuntoDisconnected(int id);
        #endregion

        #region Cumplimentacion
        Task<Cumplimentacion> GetCumplimentacionById(int? id, int? idAutorizacion);

        Task<ResultOperation<int>> DescartarCumplimentacionPorImprocedenciaAsync(Cumplimentacion cumplimentacion, Descartar descartar);
        Task<ResultOperation<int>> DescartarCumplimentacionNoAsuntoAsync(Cumplimentacion cumplimentacion, Descartar descartar);
        #endregion

        Task<AvisoConRespuesta> GetAvisoConRespuestaByIdAutorizacion(int idAutorizacion);
    }
}
