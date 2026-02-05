using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IServiceDAO
{
    public interface IAdminAbogadoService
    {
        #region Requerimiento PRODECON

        Task<ResultOperation> GetComercioExteriorRequerimientoProdeconByIdDisconnected(int id);
        Task<RequerimientoProdecon> GetComercioExteriorRequerimientoProdeconById(int id);
        Task<ResultOperation<int>> AddComercioExteriorRequerimientoProdecon(RequerimientoProdecon entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultOperation<int>> UpdateComercioExteriorRequerimientoProdecon(RequerimientoProdecon entity);
        Task<ResultOperation> GetComercioExteriorRequerimientoProdeconAsync(int idAutorizacion);
        #endregion

        #region Avisos y Comunicados        
        Task<ResultOperation<int>> AddComercioExteriorAvisosComunicados(AvisosComunicados entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultOperation<int>> UpdateComercioExteriorAvisosComunicados(AvisosComunicados entity);
        Task<ResultOperation> GetComercioExteriorAvisosComunicadosAsync(int idAutorizacion);
        Task<ResultOperation> GetComercioExteriorAvisosComunicadosByIdDisconnected(int id);
        Task<AvisosComunicados> GetComercioExteriorAvisosComunicadosById(int id);
        #endregion

        #region Requerimiento
        Task<ResultOperation<int>> AddComercioExteriorRequerimiento(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultOperation<int>> UpdateComercioExteriorRequerimiento(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile, int idSeccion);
        Task<Requerimiento> GetRequerimientoById(int id);
        Task<ResultOperation> GetComercioExteriorRequerimientoByIdDisconnected(int id);
        Task<ResultOperation> GetRequerimientoByIdAutorizacionDisconnectedAsync(int idAutorizacion);
        Task<ResultOperation> GetRequerimientoByIdDisconnected(int idAutorizacion);
        Task<List<Requerimiento>> GetRequerimientoByIdAutorizacion(int idAutorizacion);
        #endregion

        #region Resolucion
        Task<ResultOperation<bool>> UpdateImpuestosInternosNoRequerimiento(Autorizacion entity);
        Task<List<Resolucion>> GetResolucionByIdAsuntoAsync(int? idAutorizacion, int? idCumplimentacion);
        Task<ResultOperation> GetResolucionByIdAsuntoDisconnectedAsync(int? idAutorizacion, int? idCumplimentacion, int? idResolucion);
        Task<ResultOperation<int>> AddImpuestosInternosResolucionAsync(Autorizacion entityAutorizaciones, Cumplimentacion entityCumplimentacion, Resolucion entity, Documento entityDocumentoOficio, DataFile dataFile);
        Task<ResultOperation<int>> UpdateImpuestosInternosResolucionAsync(Autorizacion entityAutorizaciones, Cumplimentacion entityCumplimentacion, Resolucion entity, Documento entityDocumento, DataFile dataFile, int idSeccion);
        Task<ResultOperation<int>> UpdateConcluirAsync(Autorizacion entityAutorizaciones, Cumplimentacion cumplimentacion);
        Task<ResultOperation> GetAvisoConRespuestaRelacionadoAsync(int idAutorizacionRelacionada);
        #endregion

        #region Aviso Sin Respuesta
        Task<ResultOperation<int>> AddImpuestosInternosAvisoSinRespuesta(Autorizacion entityAutorizaciones, AvisoSinRespuesta entity, Documento entityDocumento, DataFile dataFile);

        Task<ResultOperation<int>> UpdateAvisoSinRespuesta(Autorizacion entityAutorizaciones, AvisoSinRespuesta entityAviso, List<string> folios, Documento entityDocumento, DataFile dataFile);

        Task<ResultOperation> GetNumeroAsuntoImpuestosInternosListAsync(string noAsunto, int idTipoAsunto);

        Task<Autorizacion> GetAvisoNumeroAsunto(string? noAsunto);
        #endregion

        #region Solicitud de Opinión
        Task<ResultOperation> AddComercioExteriorSolicitudOpinionInformacion(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile, Autorizacion entityComercio);
        Task<ResultOperation> UpdateComercioExteriorSolicitudOpinionInformacion(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile);
        Task<ResultOperation> GetComercioExteriorSolicitudOpinionInformacionByIdDisconnected(int id);
        Task<SolicitudOpinionInformacion> GetComercioExteriorSolicitudOpinionInformacionById(int id);
        Task<ResultOperation> GeSolicitudOpinionInformacionComercioExteriorByIdAutorizacion(int ids);
        Task<ResultOperation<int>> UpdateComercioExteriorNoSolicitudOpinionInformacion(Autorizacion entity);

        #endregion

        #region Personas Autorizadas
        Task<ResultOperation<int>> AddPersonasAutorizadas(PersonasAutorizadas entity);
        Task<ResultOperation> GetPersonasAutorizadasByIdDisconnected(int id);
        Task<ResultOperation> GetPersonasAutorizadasByIdAutorizacion(int ids);
        Task<PersonasAutorizadas> GetPersonasAutorizadasById(int id);
        Task<ResultOperation<int>> UpdatePersonasAutorizadas(PersonasAutorizadas entity);
        Task<ResultOperation<int>> DeletePersonasAutorizadasAsync(PersonasAutorizadas entity);
        #endregion

        #region Cumplimentacion
        Task<ResultOperation> GetCumplimentacionByIdDisconnectedAsync(int id);

        Task<ResultOperation<int>> UpdateCumplimentacion(Autorizacion entityAutorizacion, Cumplimentacion entityCumplimentacion, UserInformationView userInformationView);

        Task<ResultOperation<int>> UpdateCumplimentacionImprocedencia(Cumplimentacion entityCumplimentacion, UserInformationView userInformationView, Documento entityDocumento, DataFile dataFile);

        Task<ResultOperation> GetResolucionCumplimentacionDisconnectedById(int id);

        Task<ResultOperation> GetResolucionCumplimentacion(int idCumplimentacion);
        #endregion

        #region Medios de Defensa
        Task<ResultOperation> GetMediosDefensa(int idAsunto, int idTipoAsunto, int? idResolucion);

        Task<ResultOperation> GetMediosDefensaDisconnected(List<string> noAsunto);
        #endregion
    }
}
