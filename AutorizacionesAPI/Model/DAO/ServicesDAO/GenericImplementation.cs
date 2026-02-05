using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.ContractsValidations;
using AutorizacionesAPI.Model.DTO.RequestFilters;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.Entities.Events.AdminAbogado;
using AutorizacionesAPI.Model.Entities.Events.Genericos;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Files;
using Sicoj.Utils.LoggerConfiguration;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DAO.ServicesDAO
{
    public interface IGenericImplementation
    {
        void SetVariables(EnumRolesSicoj roleSicoj);
        Task<ResultOperation> PatchTomar(HttpContext context, int id, int idTipoAsunto);
        Task<ResultOperation> PatchSoltar(HttpContext context, int id, int idTipoAsunto);
        Task<ResultOperation> PatchComercioExteriorAbogado(HttpContext context, RequestComercioExteriorAbogadoUpdate request);
        Task<ResultOperation> PatchImpuestosInternosAbogado(HttpContext context, RequestImpuestosInternosAbogadoUpdate request);
        Task<ResultOperation> GetDocumentosHistoricoAsync(RequestPagerQueryDocumentosHistoricoFilters request);
        Task<ResultOperation> GetDocumentosHistoricoSeccionAsync(int idAsunto, int idTipoAsunto, int idSeccion, int? idRenglonSeccion);
        Task<ResultOperation> GetDocumentosHistoricoSeccionPaginadoAsync(RequestPagerQueryDocumentosHistoricoSeccionFilters request);
        Task<(bool success, byte[] data, System.Net.Mime.ContentDisposition content, Documento documento, string message)> GetDescargarArchivo(int id, int idTipoAsunto);
        Task<ResultOperation> PostDocumento(HttpContext context, RequestDocumentoFolioCreate request);
        Task<ResultOperation> PatchDocumento(HttpContext context, RequestDocumentoUpdate request);
        Task<ResultOperation> DeleteDocumento(HttpContext context, RequestDocumentoDelete request);
        Task<ResultOperation> PostSolicitudInformacion(HttpContext context, RequestCreateSolicitudOpinionInformacion request);
        Task<ResultOperation> GetSolicitudOpinionInformacionById(int id, int idTipoAsunto);
        Task<ResultOperation> GetSolicitudOpinionInformacionByIDS(int idAutorizacion, int idTipoAsunto);
        Task<ResultOperation> PatchSolicitudOpinionInformacion(HttpContext context, RequestSolicitudOpinionInformacionAbogadoUpdate request);
        Task<ResultOperation> PatchNoSolicitudOpinionInformacion(HttpContext context, RequestNoSolicitudOpinionInformacion request);
        Task<ResultOperation> PostPersonasAutorizadas(HttpContext context, RequestPersonasAutorizadasCreate request);
        Task<ResultOperation> GetPersonasAutorizadasById(int id);
        Task<ResultOperation> GetByID(int idAutorizacion, int idTipoAsunto);
        Task<ResultOperation> PatchPersonasAutorizadas(HttpContext context, RequestPersonasAutorizadasAbogadoUpdate request);
        Task<ResultOperation> Delete(HttpContext context, int idAsunto, int id);
        Task<ResultOperation> GetNumeroAsunto(HttpContext context, string noAsunto, int idTipoAsunto);
        Task<ResultOperation> PatchAvisoSinRespuesta(HttpContext context, RequestAvisoSinRespuesta request);
        Task<ResultOperation> GetAvisoSinRespuestaRelacionadoAsync(int id, int idTipoAsunto);
        Task<ResultOperation> GetAsuntoSinRespuestaById(int id);
        Task<ResultOperation> GetAsuntoSinRespuestaByIdAutorizacion(int id, int idTipoAsunto);
        Task<ResultOperation> GetAsuntoSinRespuestaByIdAsunto(int id, int idTipoAsunto);
        Task<ResultOperation> GetRequerimientoProdeconByAutorizacionAsync(int idAutorizacion, int idTipoAsunto);
        Task<ResultOperation> GetRequerimientoProdeconById(int id, int idTipoAsunto);
        Task<ResultOperation> PostRequerimientoProdecon(HttpContext context, RequestRequerimientoProdeconCreate request);
        Task<ResultOperation> PatchRequerimientoProdecon(HttpContext context, RequestRequerimientoProdeconUpdate request);
        Task<ResultOperation> GetRequerimientoByIdAutorizacion(int idAutorizacion, int idTipoAsunto);
        Task<ResultOperation> GetRequerimientoById(int id, int idTipoAsunto);
        Task<ResultOperation> PatchNoRequerimiento(HttpContext context, RequestNoRequerimiento request);
        Task<ResultOperation> PostRequerimiento(HttpContext context, RequestRequerimientoCreate request);
        Task<ResultOperation> PatchRequerimiento(HttpContext context, RequestRequerimientoUpdate request);
        Task<ResultOperation> GetAvisosComunicadosByIdAutorizacion(int idAutorizacion, int idTipoAsunto);
        Task<ResultOperation> GetAvisosComunicadosById(int id, int idTipoAsunto);
        Task<ResultOperation> PostAvisosComunicados(HttpContext context, RequestAvisosComunicadosCreate request);
        Task<ResultOperation> PatchAvisosComunicados(HttpContext context, RequestAvisosComunicadosUpdate request);
        Task<ResultOperation> GetResolucionById(int idAsunto, int idTipoAsunto, int? idResolucion);
        Task<ResultOperation> PostResolucion(HttpContext context, RequestResolucionCreate request);
        Task<ResultOperation> PatchResolucion(HttpContext context, RequestResolucionUpdate request);
        Task<ResultOperation> PatchConcluir(HttpContext context, RequestConcluir request);
        Task<ResultOperation> GetAvisoConRespuestaRelacionadoAsync(int idAutorizacion, int idTipoAsunto);
        Task<ResultOperation> GetCumplimentacionById(int id);
        Task<ResultOperation> PatchCumplimentacion(HttpContext context, RequestCumplimentacionAbogadoUpdate request);
    }

    public class GenericImplementation : IGenericImplementation
    {
        private readonly ILogger<GenericImplementation> _logger;
        private readonly IValidator<RequestPersonasAutorizadasCreate> _requestCreateAutorizacionesPersonasAutorizadas;
        private readonly IValidator<RequestPersonasAutorizadasAbogadoUpdate> _requestUpdateAutorizacionesPersonasAutorizadasAbogado;
        private readonly IValidator<RequestAvisoSinRespuesta> _requestAvisoSinRespuestaValidator;
        private readonly IValidator<RequestRequerimientoProdeconCreate> _requestCreateRequerimientoProdeconValidator;
        private readonly IValidator<RequestRequerimientoProdeconUpdate> _requestUpdateRequerimientoProdeconValidator;
        private readonly RequestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator _requestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator;
        private readonly RequestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator _requestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator;
        private readonly RequestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator _requestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator;
        private readonly RequestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator _requestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator;
        private readonly IValidator<RequestRequerimientoCreate> _requestRequerimientoCreateValidator;
        private readonly RequestRequerimientoUpdateValidator _requestRequerimientoUpdateValidator;
        private readonly RequestRequerimientoUpdateOnlyFileValidator _requestRequerimientoUpdateOnlyFileValidator;
        private readonly RequestResolucionCreateValidator _requestResolucionCreateValidator;
        private readonly RequestResolucionUpdateValidator _requestResolucionUpdateValidator;
        private readonly IValidator<RequestCreateSolicitudOpinionInformacion> _validatorCreateSolicitudOpinionInformacion;
        private readonly RequestUpdateSolicitudOpinionInformacionAbogadoValidator _requestUpdateSolicitudOpinionInformacionAbogadoValidator;
        private readonly RequestSolicitudUpdateOnlyFileValidatorAbogado _requestSolicitudUpdateOnlyFileValidatorAbogado;
        private readonly RequestConcluirValidator _requestConcluirValidator;
        private readonly IValidator<RequestDocumentoFolioCreate> _createArchivoValidator;
        private readonly IValidator<RequestDocumentoUpdate> _requestDocumentoUpdateValidator;
        private readonly IAbogadoService _service;
        private readonly IAdminAbogadoService _adminAbogadoService;
        private readonly IRedisClient _redisClient;
        private readonly IFileSystemService _fileSystemService;
        private readonly RequestAvisosComunicadosCreateValidator _requestAvisosComunicadosCreateValidator;
        private readonly RequestAvisosComunicadosUpdateValidator _requestAvisosComunicadosUpdateValidator;
        private readonly IGenericService _genericService;
        private EnumRolesSicoj _roleSicoj = default;
        private readonly string _path1 = EnumPaths.AUTORIZACIONES.ToString();

        public GenericImplementation(ILogger<GenericImplementation> logger, IValidator<RequestPersonasAutorizadasCreate> requestCreateAutorizacionesPersonasAutorizadas, IValidator<RequestPersonasAutorizadasAbogadoUpdate> requestUpdateAutorizacionesPersonasAutorizadasAbogado, IValidator<RequestAvisoSinRespuesta> requestAvisoSinRespuestaValidator, IValidator<RequestRequerimientoProdeconCreate> requestCreateRequerimientoProdeconValidator, IValidator<RequestRequerimientoProdeconUpdate> requestUpdateRequerimientoProdeconValidator, RequestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator requestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator, RequestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator requestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator, RequestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator requestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator, RequestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator requestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator, IValidator<RequestRequerimientoCreate> requestRequerimientoCreateValidator, RequestRequerimientoUpdateValidator requestRequerimientoUpdateValidator, RequestRequerimientoUpdateOnlyFileValidator requestRequerimientoUpdateOnlyFileValidator, RequestResolucionCreateValidator requestResolucionCreateValidator, RequestResolucionUpdateValidator requestResolucionUpdateValidator, IValidator<RequestCreateSolicitudOpinionInformacion> validatorCreateSolicitudOpinionInformacion, RequestUpdateSolicitudOpinionInformacionAbogadoValidator requestUpdateSolicitudOpinionInformacionAbogadoValidator, RequestSolicitudUpdateOnlyFileValidatorAbogado requestSolicitudUpdateOnlyFileValidatorAbogado, RequestConcluirValidator requestConcluirValidator, IValidator<RequestDocumentoFolioCreate> createArchivoValidator, IValidator<RequestDocumentoUpdate> requestDocumentoUpdateValidator, IAbogadoService service, IAdminAbogadoService adminAbogadoService, IRedisClient redisClient, IFileSystemService fileSystemService, RequestAvisosComunicadosCreateValidator requestAvisosComunicadosCreateValidator, RequestAvisosComunicadosUpdateValidator requestAvisosComunicadosUpdateValidator, IGenericService genericService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestCreateAutorizacionesPersonasAutorizadas = requestCreateAutorizacionesPersonasAutorizadas ?? throw new ArgumentNullException(nameof(requestCreateAutorizacionesPersonasAutorizadas));
            _requestUpdateAutorizacionesPersonasAutorizadasAbogado = requestUpdateAutorizacionesPersonasAutorizadasAbogado ?? throw new ArgumentNullException(nameof(requestUpdateAutorizacionesPersonasAutorizadasAbogado));
            _requestAvisoSinRespuestaValidator = requestAvisoSinRespuestaValidator ?? throw new ArgumentNullException(nameof(requestAvisoSinRespuestaValidator));
            _requestCreateRequerimientoProdeconValidator = requestCreateRequerimientoProdeconValidator ?? throw new ArgumentNullException(nameof(requestCreateRequerimientoProdeconValidator));
            _requestUpdateRequerimientoProdeconValidator = requestUpdateRequerimientoProdeconValidator ?? throw new ArgumentNullException(nameof(requestUpdateRequerimientoProdeconValidator));
            _requestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator = requestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator ?? throw new ArgumentNullException(nameof(requestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator));
            _requestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator = requestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator ?? throw new ArgumentNullException(nameof(requestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator));
            _requestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator = requestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator ?? throw new ArgumentNullException(nameof(requestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator));
            _requestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator = requestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator ?? throw new ArgumentNullException(nameof(requestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator));
            _requestRequerimientoCreateValidator = requestRequerimientoCreateValidator ?? throw new ArgumentNullException(nameof(requestRequerimientoCreateValidator));
            _requestRequerimientoUpdateValidator = requestRequerimientoUpdateValidator ?? throw new ArgumentNullException(nameof(requestRequerimientoUpdateValidator));
            _requestRequerimientoUpdateOnlyFileValidator = requestRequerimientoUpdateOnlyFileValidator ?? throw new ArgumentNullException(nameof(requestRequerimientoUpdateOnlyFileValidator));
            _requestResolucionCreateValidator = requestResolucionCreateValidator ?? throw new ArgumentNullException(nameof(requestResolucionCreateValidator));
            _requestResolucionUpdateValidator = requestResolucionUpdateValidator ?? throw new ArgumentNullException(nameof(requestResolucionUpdateValidator));
            _validatorCreateSolicitudOpinionInformacion = validatorCreateSolicitudOpinionInformacion ?? throw new ArgumentNullException(nameof(validatorCreateSolicitudOpinionInformacion));
            _requestUpdateSolicitudOpinionInformacionAbogadoValidator = requestUpdateSolicitudOpinionInformacionAbogadoValidator ?? throw new ArgumentNullException(nameof(requestUpdateSolicitudOpinionInformacionAbogadoValidator));
            _requestSolicitudUpdateOnlyFileValidatorAbogado = requestSolicitudUpdateOnlyFileValidatorAbogado ?? throw new ArgumentNullException(nameof(requestSolicitudUpdateOnlyFileValidatorAbogado));
            _requestConcluirValidator = requestConcluirValidator ?? throw new ArgumentNullException(nameof(requestConcluirValidator));
            _createArchivoValidator = createArchivoValidator ?? throw new ArgumentNullException(nameof(createArchivoValidator));
            _requestDocumentoUpdateValidator = requestDocumentoUpdateValidator ?? throw new ArgumentNullException(nameof(requestDocumentoUpdateValidator));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _adminAbogadoService = adminAbogadoService ?? throw new ArgumentNullException(nameof(adminAbogadoService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _requestAvisosComunicadosCreateValidator = requestAvisosComunicadosCreateValidator ?? throw new ArgumentNullException(nameof(requestAvisosComunicadosCreateValidator));
            _requestAvisosComunicadosUpdateValidator = requestAvisosComunicadosUpdateValidator ?? throw new ArgumentNullException(nameof(requestAvisosComunicadosUpdateValidator));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
        }

        public void SetVariables(EnumRolesSicoj roleSicoj)
        {
            _roleSicoj = roleSicoj;
        }

        public async Task<ResultOperation> PatchComercioExteriorAbogado(HttpContext context, RequestComercioExteriorAbogadoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."

                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.Id}");
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."

                    );
                }

                if (request.IdTipoModalidad == EnumTipoModalidad.FÍSICO.GetHashCode())
                {
                    var validationResult = await _requestUpdateAutorizacionesComercioExteriorAbogadoFisicoValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return ResultOperation.FailureWarningResponse(validationResult.ToString(" - "));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || entityExists.id_tipo_modalidad != EnumTipoModalidad.FÍSICO.GetHashCode())
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "La autorización no existe."

                        );
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.Id, null!);
                    Modificacion modificacion = null!;
                    if (modificacionList is not null && modificacionList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        modificacion = modificacionList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    var descartarList = await _genericService.GetDescartarByIdAsunto(request.Id, null!);
                    Descartar descartar = null!;
                    if (descartarList is not null && descartarList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    AvisoSinRespuesta avisoSinRespuesta = null!;
                    AvisoConRespuesta avisoConRespuesta = null!;
                    Autorizacion entityRelacionada = null!;
                    if (request.idAsuntoRelacionado.GetValueOrDefault(0) > 0)
                    {
                        entityRelacionada = await _genericService.GetAutorizacionById(request.idAsuntoRelacionado.GetValueOrDefault());
                        if (entityRelacionada is null || !entityRelacionada.activo)
                        {
                            return
                                ResultOperation.FailureErrorResponse<string>(
                                    "La autorización a relacionar no existe o fue eliminada."

                            );
                        }
                    }
                    try
                    {
                        AutorizacionAdminAbogadoEvents.Update(ref entityExists,
                                ref avisoSinRespuesta,
                                ref avisoConRespuesta,
                                entityRelacionada,
                                request.noAsuntoExterno,
                                ref modificacion,
                                ref descartar,
                                request.IdTipoAutorizacion,
                                request.Rfc,
                                request.Promovente!,
                                request.PromoventeNoContribuyente,
                                request.RfcContribuyente,
                                request.Contribuyente,
                                request.DespachosAutorizados!,
                                request.IdAutoridadDirigida,
                                request.OtroAutoridadDirigida,
                                request.DomicilioNotificaciones,
                                DateTime.Parse(request.FechaPresentacion),
                                request.IdFundamentoSolicitud!,
                                request.OtroFundamentoSolicitud,
                                request.IdTema!,
                                request.OtroTema,
                                request.NoIndicaMonto,
                                request.Monto,
                                DateTime.Parse(request.FechaRecepcion),
                                sessionInformation.UserInformation.Rfc
                            );
                    }
                    catch (Exception _ex)
                    {
                        return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                    }
                    var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation, EnumSeccionesCons.DATOS_GENERALES);
                    return result;
                }
                else if (request.IdTipoModalidad == EnumTipoModalidad.LÍNEA.GetHashCode())
                {
                    var validationResult = await _requestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return ResultOperation.FailureWarningResponse(validationResult.ToString(" - "));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || entityExists.id_tipo_modalidad != EnumTipoModalidad.LÍNEA.GetHashCode())
                    {
                        return
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe."

                        );
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.Id, null!);
                    Modificacion modificacion = null!;
                    if (modificacionList is not null && modificacionList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        modificacion = modificacionList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    var descartarList = await _genericService.GetDescartarByIdAsunto(request.Id, null!);
                    Descartar descartar = null!;
                    if (descartarList is not null && descartarList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    AvisoSinRespuesta avisoSinRespuesta = null!;
                    AvisoConRespuesta avisoConRespuesta = null!;
                    Autorizacion entityRelacionada = null!;
                    if (request.idAsuntoRelacionado.GetValueOrDefault(0) > 0)
                    {
                        entityRelacionada = await _genericService.GetAutorizacionById(request.idAsuntoRelacionado.GetValueOrDefault());
                        if (entityRelacionada is null || !entityRelacionada.activo)
                        {
                            return
                                ResultOperation.FailureErrorResponse<string>(
                                    "La autorización a relacionar no existe o fue eliminada."

                            );
                        }
                    }

                    try
                    {
                        AutorizacionAdminAbogadoEvents.Update(
                                ref entityExists,
                                ref avisoSinRespuesta,
                                ref avisoConRespuesta,
                                entityRelacionada,
                                request.noAsuntoExterno,
                                ref modificacion,
                                ref descartar,
                                request.IdTipoAutorizacion,
                                entityExists.rfc,
                                entityExists.promovente!,
                                request.PromoventeNoContribuyente,
                                request.RfcContribuyente,
                                request.Contribuyente,
                                entityExists.despachos_autorizados!,
                                request.IdAutoridadDirigida,
                                request.OtroAutoridadDirigida,
                                request.DomicilioNotificaciones,
                                DateTime.Parse(request.FechaPresentacion),
                                request.IdFundamentoSolicitud!,
                                request.OtroFundamentoSolicitud,
                                request.IdTema!,
                                request.OtroTema,
                                entityExists.no_indica_monto,
                                entityExists.monto,
                                DateTime.Parse(request.FechaRecepcion),
                                sessionInformation.UserInformation.Rfc
                            );
                    }
                    catch (Exception _ex)
                    {
                        return ResultOperation.FailureWarningResponse(_ex.Message);
                    }
                    var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation, EnumSeccionesCons.DATOS_GENERALES);
                    return result;
                }

                return ResultOperation.FailureWarningResponse("El tipo de modalidad no es válida.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchImpuestosInternosAbogado(HttpContext context, RequestImpuestosInternosAbogadoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."

                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.Id}");
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."

                    );
                }

                if (request.IdTipoModalidad == EnumTipoModalidad.FÍSICO.GetHashCode())
                {
                    var validationResult = await _requestUpdateAutorizacionesImpuestosInternosAbogadoFisicoValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return ResultOperation.FailureWarningResponse(validationResult.ToString(" - "));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || !entityExists.activo || entityExists.id_tipo_modalidad != EnumTipoModalidad.FÍSICO.GetHashCode())
                    {
                        return
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe o fue eliminada."

                        );
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.Id, null!);
                    Modificacion modificacion = null!;
                    if (modificacionList is not null && modificacionList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        modificacion = modificacionList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    var descartarList = await _genericService.GetDescartarByIdAsunto(request.Id, null!);
                    Descartar descartar = null!;
                    if (descartarList is not null && descartarList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    AvisoSinRespuesta avisoSinRespuesta = await _genericService.GetAvisoSinRespuestaByIdAutorizacion(request.Id);
                    AvisoConRespuesta avisoConRespuesta = await _genericService.GetAvisoConRespuestaByIdAutorizacion(request.Id);
                    Autorizacion entityRelacionado = null!;
                    string? noAsuntoExterno = null;

                    if (!string.IsNullOrWhiteSpace(request.NoAsuntoAviso))
                    {
                        var autorizacionRelacionadaExists = await _adminAbogadoService.GetAvisoNumeroAsunto(request.NoAsuntoAviso);
                        if (autorizacionRelacionadaExists is not null)
                        {
                            if (!autorizacionRelacionadaExists.activo)
                            {
                                return ResultOperation.FailureErrorResponse<string>(
                                    "El número de asunto a relacionar fue eliminado."
                                );
                            }

                            entityRelacionado = autorizacionRelacionadaExists;
                        }
                        else
                        {
                            noAsuntoExterno = request.NoAsuntoAviso;
                        }
                    }

                    try
                    {
                        AutorizacionAdminAbogadoEvents.Update(ref entityExists,
                            ref avisoSinRespuesta,
                            ref avisoConRespuesta,
                            entityRelacionado,
                            noAsuntoExterno,
                            ref modificacion,
                            ref descartar,
                            entityExists.id_tipo_autorizacion,
                            request.Rfc,
                            request.Promovente,
                            request.PromoventeNoContribuyente,
                            request.RfcContribuyente,
                            request.Contribuyente,
                            request.DespachosAutorizados,
                            entityExists.id_autoridad_dirigida!,
                            entityExists.otra_autoridad_dirigida,
                            request.DomicilioNotificaciones,
                            DateTime.Parse(request.FechaPresentacion),
                            entityExists.id_fundamento_solicitud,
                            entityExists.otro_fundamento_solicitud,
                            request.IdTema,
                            request.OtroTema,
                            request.NoIndicaMonto,
                            request.Monto,
                            DateTime.Parse(request.FechaRecepcion),
                            sessionInformation.UserInformation.Rfc
                            );
                    }
                    catch (Exception _ex)
                    {
                        return ResultOperation.FailureWarningResponse(_ex.Message);
                    }
                    var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation, EnumSeccionesCons.DATOS_GENERALES);
                    return result;
                }
                else if (request.IdTipoModalidad == EnumTipoModalidad.LÍNEA.GetHashCode())
                {
                    var validationResult = await _requestUpdateAutorizacionesImpuestosInternosAbogadoLineaValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return ResultOperation.FailureWarningResponse(validationResult.ToString(" - "));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || !entityExists.activo || entityExists.id_tipo_modalidad != EnumTipoModalidad.LÍNEA.GetHashCode())
                    {
                        return
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe o fue eliminada."

                        );
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.Id, null!);
                    Modificacion modificacion = null!;
                    if (modificacionList is not null && modificacionList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        modificacion = modificacionList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    var descartarList = await _genericService.GetDescartarByIdAsunto(request.Id, null!);
                    Descartar descartar = null!;
                    if (descartarList is not null && descartarList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES)))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES))!;
                    }

                    AvisoSinRespuesta avisoSinRespuesta = await _genericService.GetAvisoSinRespuestaByIdAutorizacion(request.Id);
                    AvisoConRespuesta avisoConRespuesta = await _genericService.GetAvisoConRespuestaByIdAutorizacion(request.Id);
                    Autorizacion entityRelacionado = null!;
                    Autorizacion NoAsuntoAviso = null!;
                    string? noAsuntoExterno = null;

                    if (!string.IsNullOrWhiteSpace(request.NoAsuntoAviso))
                    {
                        NoAsuntoAviso = await _adminAbogadoService.GetAvisoNumeroAsunto(request.NoAsuntoAviso);

                        if (NoAsuntoAviso is not null)
                        {
                            if (!NoAsuntoAviso.activo)
                            {
                                return ResultOperation.FailureErrorResponse<string>(
                                    "El número de asunto a relacionar fue eliminado."
                                );
                            }

                            entityRelacionado = NoAsuntoAviso;
                        }
                        else
                        {
                            noAsuntoExterno = request.NoAsuntoAviso;
                        }
                    }

                    try
                    {
                        AutorizacionAdminAbogadoEvents.Update(
                            ref entityExists,
                            ref avisoSinRespuesta,
                            ref avisoConRespuesta,
                            entityRelacionado,
                            entityRelacionado is null ? noAsuntoExterno : entityRelacionado.no_asunto,
                            ref modificacion,
                            ref descartar,
                            entityExists.id_tipo_autorizacion,
                            entityExists.rfc,
                            entityExists.promovente,
                            request.PromoventeNoContribuyente,
                            request.RfcContribuyente,
                            request.Contribuyente,
                            request.DespachosAutorizados,
                            entityExists.id_autoridad_dirigida,
                            entityExists.otra_autoridad_dirigida,
                            request.DomicilioNotificaciones,
                            DateTime.Parse(request.FechaPresentacion),
                            entityExists.id_fundamento_solicitud,
                            entityExists.otro_fundamento_solicitud,
                            request.IdTema,
                            request.OtroTema,
                            request.NoIndicaMonto,
                            request.Monto,
                            DateTime.Parse(request.FechaRecepcion),
                            sessionInformation.UserInformation.Rfc
                            );
                    }
                    catch (Exception _ex)
                    {
                        return ResultOperation.FailureWarningResponse(_ex.Message);
                    }
                    var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation, EnumSeccionesCons.DATOS_GENERALES);
                    return result;
                }

                return ResultOperation.FailureWarningResponse("El tipo de modalidad no es válida.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchTomar(HttpContext context, int id, int idTipoAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                    );
                }
                EnumModulosRedis enumModulo = default!;
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(id);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                        );
                    }
                    switch (idTipoAsunto)
                    {
                        case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.DONATARIAS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        default:
                            return ResultOperation.FailureErrorResponse<string>("Tipo de asunto no existe.");
                    }
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    Cumplimentacion entityExists = await _genericService.GetCumplimentacionById(id, null!);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<string>(
                                "La cumplimentación no existe o fue eliminada."
                        );
                    }
                    enumModulo = EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION;
                }
                else
                    return ResultOperation.FailureErrorResponse<string>("No existe el tipo de asunto.");

                var response = await _redisClient.Take(enumModulo, id, sessionInformation.UserInformation.Rfc!, sessionInformation.UserInformation.Nombre!);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchSoltar(HttpContext context, int id, int idTipoAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."

                    );
                }

                EnumModulosRedis enumModulo = default!;
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(id);
                    if (entityExists is null)
                    {
                        return
                            ResultOperation.FailureErrorResponse<bool>(
                                "La autorización no existe."

                        );
                    }
                    switch (idTipoAsunto)
                    {
                        case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.DONATARIAS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        default:
                            return ResultOperation.FailureErrorResponse<string>("Tipo de asunto no existe.");
                    }
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    Cumplimentacion entityExists = await _genericService.GetCumplimentacionById(id, null!);
                    if (entityExists is null)
                    {
                        return
                            ResultOperation.FailureErrorResponse<bool>(
                                "La cumplimentación no existe."

                        );
                    }
                    enumModulo = EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION;
                }
                else
                    return ResultOperation.FailureErrorResponse<bool>("No existe el tipo de entrada.");

                var response = await _redisClient.Drop(enumModulo, id, sessionInformation.UserInformation.Rfc!);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetDocumentosHistoricoAsync(RequestPagerQueryDocumentosHistoricoFilters request)
        {
            try
            {
                if (!Filters.MapSort<EnumOrderColumnDocumentosByFiltros>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return ResultOperation.FailureWarningResponse<List<ResponseDocumentoHistorico>>("La columna de ordenamiento no es válida.");
                }
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var result = await _genericService.GetDocumentosAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : request.idAsunto,
                    request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? request.idAsunto : null);
                    return result;
                }

                return ResultOperation.FailureWarningResponse<List<ResponseDocumentoHistorico>>("El tipo de asunto no es válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetDocumentosHistoricoSeccionAsync(int idAsunto, int idTipoAsunto, int idSeccion, int? idRenglonSeccion)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var result = await _genericService.GetDocumentosSeccionAsync(
                    idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : idAsunto,
                    idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? idAsunto : null,
                    idSeccion,
                    idRenglonSeccion.GetValueOrDefault(0) == 0 ? null! : idRenglonSeccion);
                    return result;
                }
                return ResultOperation<DataTableView<ResponseDocumentoHistorico>>.FailureWarningResponse("El tipo de asunto no es válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetDocumentosHistoricoSeccionPaginadoAsync(RequestPagerQueryDocumentosHistoricoSeccionFilters request)
        {
            try
            {
                if (!Filters.MapSort<EnumOrderColumnDocumentosByFiltros>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return ResultOperation.FailureWarningResponse<DataTableView<ResponseDocumentoHistorico>>("La columna de ordenamiento no es válida.");
                }
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var result = await _genericService.GetDocumentosSeccionPaginadoAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : request.idAsunto,
                    request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? request.idAsunto : null,
                    request.idSeccion,
                    request.idRenglonSeccion);
                    return result;
                }
                return ResultOperation.FailureWarningResponse<DataTableView<ResponseDocumentoHistorico>>("El tipo de asunto no es válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool success, byte[] data, System.Net.Mime.ContentDisposition content, Documento documento, string message)> GetDescargarArchivo(int id, int idTipoAsunto)
        {
            try
            {
                var entityDocumentoList = await _genericService.GetArchivosAutorizacionesByIds(new int[] { id });
                if (entityDocumentoList is null || !entityDocumentoList.Any(c => c.activo))
                {
                    return (false,
                            null!,
                            null!,
                            null!,
                            "No existe el documento o fue eliminado."
                            );
                }

                var entityDocumento = entityDocumentoList.FirstOrDefault(c => c.activo);
                var response = await _fileSystemService.GetFileAsync(entityDocumento!.file_path);
                if (response is null)
                {
                    return (false,
                            null!,
                            null!,
                            null!,
                            "No existe el documento."
                            );
                }

                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    Inline = true,
                    FileName = Path.GetFileName(entityDocumento.file_path)
                };
                return (true,
                            null!,
                            contentDisposition,
                            entityDocumento,
                            null!
                            );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PostDocumento(HttpContext context, RequestDocumentoFolioCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _createArchivoValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return
                            ResultOperation.FailureErrorResponse<int>("Tipo Asunto no válido"
                        );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha apartado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                bool permanente = false;
                switch (request.idTipoArchivo)
                {
                    case EnumTipoDocumentoCons.SOLICITUD_AUTORIZACIÓN:
                        permanente = true;
                        break;
                }

                string path2 = null!;
                if (request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION)
                {
                    path2 = EnumPathSub.CUMPLIMENTACION.ToString();
                    var entityCumplimentacion = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (entityCumplimentacion is null)
                    {
                        return
                            ResultOperation.FailureErrorResponse(
                                "La cumplimentación no existe."
                        );
                    }
                }
                else
                {
                    path2 = EnumPathSub.AUTORIZACION.ToString();
                    var entityExists = await _genericService.GetAutorizacion(request.idAsunto);
                    if (entityExists is null)
                    {
                        return
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe."
                        );
                    }
                }

                if (permanente)
                {
                    List<Documento> listDocumentos = await _genericService.GetDocumentosByRenglonTipoAsync(
                        request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : request.idAsunto,
                        request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? request.idAsunto : null,
                        request.idSeccion, request.idRenglonSeccion, request.idTipoArchivo);
                    if (listDocumentos is not null && listDocumentos.Any(c => c.activo))
                    {
                        permanente = false;
                    }
                }
                if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine(_path1, path2, request.idAsunto.ToString()!),
                        out var dataFile, out string message, requiredExtentions))
                {
                    return
                            ResultOperation.FailureErrorResponse<int>(
                                message
                        );
                }

                bool idCumplimentacion = request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION;
                Documento entity = null!;
                try
                {
                    entity = ArchivoEvents.CreateFolio(
                        request.numeroFolio,
                        idCumplimentacion ? null : request.idAsunto,
                        request.idTipoArchivo,
                        request.idSeccion,
                        request.idRenglonSeccion,
                        sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                        dataFile.File.FileName,
                        dataFile.FilePath,
                        dataFile.File.ContentType,
                        _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                        sessionInformation.UserInformation.Rfc,
                        sessionInformation.UserInformation.Rfc,
                        permanente,
                        _roleSicoj.GetHashCode(),
                        idCumplimentacion ? request.idAsunto : null
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse(_ex.Message);
                }
                var result = await _genericService.AddArchivo(entity, dataFile);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchDocumento(HttpContext context, RequestDocumentoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestDocumentoUpdateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return
                            ResultOperation.FailureErrorResponse<int>("Tipo Asunto no válido");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización.");
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."
                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = null!;
                if (request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION)
                {
                    path2 = EnumPathSub.CUMPLIMENTACION.ToString();
                    Cumplimentacion entityCumplimentacion = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (entityCumplimentacion is null || !entityCumplimentacion.activo)
                    {
                        return ResultOperation.FailureErrorResponse(
                                "La cumplimentación no existe o fue eliminada."
                        );
                    }
                }
                else
                {
                    path2 = EnumPathSub.AUTORIZACION.ToString();
                    Autorizacion entityAutorizacionExists = await _genericService.GetAutorizacion(request.idAsunto);
                    if (entityAutorizacionExists is null || !entityAutorizacionExists.activo)
                    {
                        return ResultOperation.FailureErrorResponse(
                                "La autorización no existe o fue eliminada."
                        );
                    }
                }

                var entityDocumentoList = await _genericService.GetArchivosAutorizacionesByIds(new int[] { request.id });
                if (entityDocumentoList is null || !entityDocumentoList.Any(c => c.activo))
                {
                    return ResultOperation.FailureErrorResponse<int>(
                            "No existe el documento o fue eliminado."
                    );
                }

                var entityDocumento = entityDocumentoList.FirstOrDefault(c => c.activo);
                if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine(_path1, path2, request.idAsunto.ToString()!),
                        out var dataFile, out string message, requiredExtentions, entityDocumento!.file_path))
                {
                    return ResultOperation.FailureErrorResponse<int>(
                                message
                        );
                }

                try
                {
                    if (dataFile is not null)
                    {
                        ArchivoEvents.UpdateWithFile(
                            ref entityDocumento,
                            request.numeroFolio!,
                            request.idTipoArchivo,
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File!.ContentType,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    else
                    {
                        ArchivoEvents.Update(
                            ref entityDocumento,
                            request.numeroFolio!,
                            request.idTipoArchivo,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _genericService.UpdateArchivo(entityDocumento!, dataFile!);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> DeleteDocumento(HttpContext context, RequestDocumentoDelete request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                    );
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización.");
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."
                    );
                }

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var entityListExists = await _genericService.GetArchivosAutorizacionesByIds(request.Ids.ToArray());
                    if (entityListExists is null || !entityListExists.Any())
                    {
                        return ResultOperation.FailureErrorResponse(
                                "Los documentos no existen."
                        );
                    }

                    if (!request.Ids.All(value => entityListExists.Select(x => x.id).Contains(value)))
                    {
                        return ResultOperation.FailureErrorResponse(
                                "Algunos documentos no existen."
                        );
                    }

                    try
                    {
                        for (int i = 0; i < entityListExists.Count; i++)
                        {
                            var entity = entityListExists[i];
                            ArchivoEvents.Delete(ref entity, sessionInformation.UserInformation.Rfc);
                        }
                    }
                    catch (Exception _ex)
                    {
                        return ResultOperation.FailureWarningResponse(_ex.Message);
                    }

                    var result = await _genericService.DeleteArchivo(request.Ids.ToArray(), sessionInformation.UserInformation.Rfc);
                    return result;
                }
                return ResultOperation.FailureWarningResponse("El tipo de modalidad no es válida.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PostSolicitudInformacion(HttpContext context, RequestCreateSolicitudOpinionInformacion request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return ResultOperation.FailureErrorResponse("No se pudo obtener la información del usuario.");
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _validatorCreateSolicitudOpinionInformacion.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                if (key is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no lo ha tomado.");
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return ResultOperation.FailureInformationResponse("El registro ya se encuentra en uso por otro usuario.");
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                Autorizacion entityExist = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExist is null || !entityExist.activo)
                {
                    return ResultOperation.FailureErrorResponse<int>("La autorización no existe o fue eliminada.");
                }

                string path2 = EnumPathSub.AUTORIZACION.ToString();
                DataFile dataFile = null!;
                if (request.documento != null ||
                    !string.IsNullOrEmpty(request.numeroFolio) ||
                    request.idTipoArchivo.GetValueOrDefault() > 0 ||
                    request.idSeccion.GetValueOrDefault() > 0)
                {
                    var validationResultFile = await _validatorCreateSolicitudOpinionInformacion.ValidateAsync(request);
                    if (!validationResultFile.IsValid)
                    {
                        return ResultOperation.FailureWarningResponse(validationResultFile.ToString(" -- "));
                    }

                    if (request.documento != null)
                    {
                        _fileSystemService.FileTryOut(
                            request.documento!,
                            Path.Combine(_path1, path2, request.idAsunto.ToString()),
                            out dataFile
                        );
                    }
                }

                var entity = ComercioExteriorSolicitudOpinionInformacionEvents.CreateComercioExteriorSolicitudOpinionInformacion(
                    entityExist,
                    request.idAsunto,
                    request.idUnidadAdministrativa,
                    request.noOficioSolicitud,
                    DateTime.Parse(request.fechaOficioSolicitud!),
                    request.unidadInterna,
                    request.unidadAdministrativaExterna,
                    sessionInformation.UserInformation.Rfc!
                );

                if (dataFile is not null)
                {
                    var entityDocumentoUpdated = ArchivoEvents.CreateFolio(
                        request.numeroFolio!,
                        request.idAsunto,
                        request.idTipoArchivo.GetValueOrDefault(),
                        request.idSeccion.GetValueOrDefault(),
                        null!,
                        sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                        dataFile.File.FileName,
                        dataFile.FilePath,
                        dataFile.File!.ContentType,
                        _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                        sessionInformation.UserInformation.Rfc!,
                        sessionInformation.UserInformation.Rfc!,
                        true,
                        _roleSicoj.GetHashCode(),
                        null!
                    );
                    var resultWithDocument = await _adminAbogadoService.AddComercioExteriorSolicitudOpinionInformacion(entity, entityDocumentoUpdated, dataFile, entityExist);
                    return resultWithDocument;
                }
                else
                {
                    var resultWithoutDocument = await _adminAbogadoService.AddComercioExteriorSolicitudOpinionInformacion(entity, null!, null!, entityExist);
                    return resultWithoutDocument;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetSolicitudOpinionInformacionById(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetComercioExteriorSolicitudOpinionInformacionByIdDisconnected(id);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<ResponseRequerimiento>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetSolicitudOpinionInformacionByIDS(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GeSolicitudOpinionInformacionComercioExteriorByIdAutorizacion(idAutorizacion);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<ResponseRequerimiento>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchSolicitudOpinionInformacion(HttpContext context, RequestSolicitudOpinionInformacionAbogadoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return ResultOperation.FailureErrorResponse("No se pudo obtener la información del usuario.");
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestUpdateSolicitudOpinionInformacionAbogadoValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "));
                }

                string key;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no lo ha tomado.");
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return ResultOperation.FailureInformationResponse("El registro ya se encuentra en uso por otro usuario.");
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };

                var entityComercio = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityComercio is null || !entityComercio.activo)
                {
                    return ResultOperation.FailureWarningResponse("La autorización no existe o fue eliminada.");
                }

                var entityExists = await _adminAbogadoService.GetComercioExteriorSolicitudOpinionInformacionById(request.id);
                if (entityExists == null)
                {
                    return ResultOperation.FailureWarningResponse("La solicitud no existe o fue eliminada.");
                }

                DataFile dataFile = null!;
                Documento entityDocumento = null!;
                string path2 = EnumPathSub.AUTORIZACION.ToString();
                if (request.documento != null || !string.IsNullOrEmpty(request.numeroFolio) ||
                    request.idTipoArchivo.GetValueOrDefault() > 0 || request.idSeccion.GetValueOrDefault() > 0)
                {
                    var validationResultFile = await _requestSolicitudUpdateOnlyFileValidatorAbogado.ValidateAsync(request);
                    if (!validationResultFile.IsValid)
                    {
                        return ResultOperation.FailureWarningResponse(validationResultFile.ToString(" - "));
                    }

                    List<Documento> listDocumentos = await _genericService.GetDocumentosByRenglonTipoAsync(
                        request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : request.idAsunto,
                        null,
                        request.idSeccion.GetValueOrDefault(), request.id, null!);
                    if (listDocumentos is not null && listDocumentos.Any(c => c.activo))
                    {
                        entityDocumento = listDocumentos.FirstOrDefault(c => c.activo)!;
                    }

                    if (request.documento is not null)
                    {
                        if (!_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine(_path1, path2, request.idAsunto.ToString()),
                            out dataFile, out string message, requiredExtentions, entityDocumento is null ? null! : entityDocumento.file_path))
                        {
                            return ResultOperation.FailureErrorResponse<int>(message);
                        }
                    }
                }

                try
                {
                    ComercioExteriorSolicitudOpinionInformacionEvents.UpdateComercioExteriorSolicitudOpinonInformacion(
                        ref entityExists,
                        entityComercio,
                        request.idAsunto,
                        request.idUnidadAdministrativa,
                        request.noOficioSolicitud,
                        DateTime.Parse(request.fechaOficioSolicitud!),
                        request.atendioSolicitud,
                        string.IsNullOrWhiteSpace(request.noOficioRespuesta) || request.noOficioRespuesta == "undefined" ? null : request.noOficioRespuesta,
                        string.IsNullOrWhiteSpace(request.fechaOficioRespuesta) || request.fechaOficioRespuesta == "undefined" ? (DateTime?)null : DateTime.Parse(request.fechaOficioRespuesta),
                        string.IsNullOrWhiteSpace(request.fechaRecepcion) || request.fechaRecepcion == "undefined" ? (DateTime?)null : DateTime.Parse(request.fechaRecepcion),
                        request.unidadInterna,
                        request.unidadAdministrativaExterna,
                        sessionInformation.UserInformation.Rfc!
                    );

                    if (entityDocumento is null)
                    {
                        if (dataFile is not null)
                        {
                            entityDocumento = ArchivoEvents.CreateFolio(
                                request.numeroFolio!,
                                request.idAsunto,
                                request.idTipoArchivo.GetValueOrDefault(),
                                request.idSeccion.GetValueOrDefault(),
                                null!,
                                sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                                dataFile.File.FileName,
                                dataFile.FilePath,
                                dataFile.File!.ContentType,
                                _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                                sessionInformation.UserInformation.Rfc!,
                                sessionInformation.UserInformation.Rfc!,
                                true,
                                _roleSicoj.GetHashCode(),
                                null!
                            );
                        }
                    }
                    else
                    {
                        if (dataFile is null)
                        {
                            ArchivoEvents.Update(
                                ref entityDocumento,
                                request.numeroFolio!,
                                request.idTipoArchivo.GetValueOrDefault(),
                                sessionInformation.UserInformation.Rfc!,
                                sessionInformation.UserInformation.Rfc!
                            );
                        }
                        else
                        {
                            ArchivoEvents.UpdateWithFile(
                               ref entityDocumento,
                               request.numeroFolio,
                               request.idTipoArchivo.GetValueOrDefault(),
                               dataFile.File.FileName,
                               dataFile.FilePath,
                               dataFile.File.ContentType,
                               _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                               sessionInformation.UserInformation.Rfc,
                               sessionInformation.UserInformation.Rfc
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ResultOperation.FailureWarningResponse(ex.Message);
                }

                var result = await _adminAbogadoService.UpdateComercioExteriorSolicitudOpinionInformacion(entityExists, entityDocumento!, dataFile!);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchNoSolicitudOpinionInformacion(HttpContext context, RequestNoSolicitudOpinionInformacion request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."
                    );
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                    );
                }

                Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "La autorización no existe o fue eliminada."
                    );
                }

                try
                {
                    ComercioExteriorSolicitudOpinionInformacionEvents.NoSolicitudOpinionInformacion(
                        ref entityExists,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.UpdateComercioExteriorNoSolicitudOpinionInformacion(entityExists);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PostPersonasAutorizadas(HttpContext context, RequestPersonasAutorizadasCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                    );
                }

                var validatationResult = await _requestCreateAutorizacionesPersonasAutorizadas.ValidateAsync(request);
                if (!validatationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse(validatationResult.ToString(" - "));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}");
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede agregar ya que el usuario no lo ha apartado."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                    );
                }

                var entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return ResultOperation.FailureWarningResponse("La autorización no existe o fue eliminada");
                }

                PersonasAutorizadas entity = null!;
                try
                {
                    entity = ComercioExteriorPersonasAutorizadasEvents.CreatePersonasAutorizadas(entityExists,
                        request.Nombre,
                        request.Rfc,
                        request.Telefono,
                        request.Email,
                        request.idAsunto
                     );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse(_ex.Message);
                }

                ResultOperation result = await _adminAbogadoService.AddPersonasAutorizadas(entity);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetPersonasAutorizadasById(int id)
        {
            try
            {
                var result = await _adminAbogadoService.GetPersonasAutorizadasByIdDisconnected(id);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetByID(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto == EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR)
                {
                    var result = await _adminAbogadoService.GetPersonasAutorizadasByIdAutorizacion(idAutorizacion);
                    return result;
                }
                return ResultOperation.SuccessResponseNoMessage<List<ResponseComercioExteriorPersonasAutorizadasAbogadoById>>(new());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchPersonasAutorizadas(HttpContext context, RequestPersonasAutorizadasAbogadoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return ResultOperation.FailureErrorResponse("No se pudo obtener la información del usuario.");
                }

                var validationResult = await _requestUpdateAutorizacionesPersonasAutorizadasAbogado.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse(validationResult.ToString(" - "));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}");
                if (keyExists is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado.");
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return ResultOperation.FailureInformationResponse("La autorización ya se encuentra en uso por otro usuario.");
                }

                var entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return ResultOperation.FailureWarningResponse("La autorización no existe o fue eliminada");
                }

                var entityPersonasAutorizadas = await _adminAbogadoService.GetPersonasAutorizadasById(request.id);
                if (entityPersonasAutorizadas == null)
                {
                    return ResultOperation.FailureErrorResponse("La persona autorizada no existe.");
                }

                try
                {
                    ComercioExteriorPersonasAutorizadasEvents.UpdatePersonasAutorizadas(
                        ref entityPersonasAutorizadas,
                        entityExists,
                        request.nombre,
                        request.rfc,
                        request.telefono,
                        request.email,
                        request.idAsunto
                    );
                }
                catch (Exception ex)
                {
                    return ResultOperation.FailureWarningResponse(ex.Message);
                }

                var result = await _adminAbogadoService.UpdatePersonasAutorizadas(entityPersonasAutorizadas);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> Delete(HttpContext context, int idAsunto, int id)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{idAsunto}");
                if (keyExists is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El registro no se puede eliminar ya que el usuario no lo ha apartado.");
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return ResultOperation.FailureInformationResponse("La autorización ya se encuentra en uso por otro usuario.");
                }

                var entityExists = await _genericService.GetAutorizacionById(idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return ResultOperation.FailureWarningResponse("La autorización no existe o fue eliminada");
                }

                var entityPersonasAutorizadas = await _adminAbogadoService.GetPersonasAutorizadasById(id);
                if (entityPersonasAutorizadas is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<string>("La persona autorizada no existe o fue eliminada.");
                }

                try
                {
                    ComercioExteriorPersonasAutorizadasEvents.UpdateDelete(
                        ref entityPersonasAutorizadas,
                        entityExists,
                        id
                        );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse(_ex.Message);
                }
                var result = await _adminAbogadoService.DeletePersonasAutorizadasAsync(entityPersonasAutorizadas);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetNumeroAsunto(HttpContext context, string noAsunto, int idTipoAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                    );
                }

                switch (idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                        idTipoAsunto = EnumTipoAsuntoConst.DONATARIAS;
                        break;
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        idTipoAsunto = EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS;
                        break;
                }

                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    var result = await _adminAbogadoService.GetNumeroAsuntoImpuestosInternosListAsync(noAsunto, idTipoAsunto);
                    return result;
                }

                return ResultOperation.FailureErrorResponse<string>("No existe el tipo de asunto.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchAvisoSinRespuesta(HttpContext context, RequestAvisoSinRespuesta request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestAvisoSinRespuestaValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse(validationResult.ToString(" - "));
                }

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                {
                    var entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                        );
                    }

                    var entityAviso = await _genericService.GetAvisoSinRespuestaByIdAutorizacion(request.idAsunto);
                    if (entityAviso is null || !entityAviso.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<string>(
                                "El aviso sin respuesta no existe o fue eliminado."
                        );
                    }

                    if (request.documento is null)
                    {
                        return
                                ResultOperation.FailureErrorResponse<string>(
                                    "El documento es obligatorio."
                            );
                    }

                    EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                    string path2 = EnumPathSub.AUTORIZACION.ToString();
                    if (!_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine(_path1, path2, request.idAsunto.ToString()),
                            out var dataFile, out string message, requiredExtentions))
                    {
                        return ResultOperation.FailureErrorResponse<int>(message);
                    }

                    var descarteList = await _genericService.GetDescartarByIdAsunto(entityExists.id, null!);
                    Descartar entityDescarte = null!;
                    if (descarteList is not null && descarteList.Any(c => c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES) && c.activo))
                    {
                        entityDescarte = descarteList.FirstOrDefault(c => c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES) && c.activo)!;
                    }

                    Documento entityDocumento = null!;
                    List<string> folios = new();

                    try
                    {
                        ImpuestosInternosAvisoSinRespuestaEvents.Update(
                            ref entityExists,
                            ref entityAviso,
                            ref folios,
                            entityDescarte!,
                            request.observaciones,
                            sessionInformation.UserInformation.Rfc!
                        );

                        entityDocumento = ArchivoEvents.CreateFolio(
                            request.numeroFolio!,
                            request.idAsunto,
                            request.idTipoArchivo.GetValueOrDefault(),
                            EnumSeccionesCons.AVISO_SIN_RESPUESTA,
                            null!,
                            sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File!.ContentType,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            true,
                            _roleSicoj.GetHashCode(),
                            null!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return ResultOperation.FailureWarningResponse(_ex.Message);
                    }

                    ResultOperation<int> result = await _adminAbogadoService.UpdateAvisoSinRespuesta(entityExists, entityAviso, request.folios, entityDocumento, dataFile);
                    return result;
                }

                return ResultOperation.FailureErrorResponse<string>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAvisoSinRespuestaRelacionadoAsync(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    var result = await _genericService.GetAvisoSinRespuestaRelacionadoAsync(id);
                    return result;
                }

                return ResultOperation.SuccessResponseNoMessage<List<ResponseAvisoSinRespuestaRelacionado>>(new());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAsuntoSinRespuestaById(int id)
        {
            try
            {
                var result = await _genericService.GetAvisoSinRespuestaByIdDisconnected(id);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAsuntoSinRespuestaByIdAutorizacion(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                {
                    var result = await _genericService.GetAvisoSinRespuestaPendienteByIdAuntoDisconnected(id);
                    return result;
                }

                return ResultOperation.FailureErrorResponse("El tipo de asunto no es válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAsuntoSinRespuestaByIdAsunto(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                {
                    var result = await _genericService.GetAvisoSinRespuestaByIdAsuntoDisconnected(id);
                    return result;
                }

                return ResultOperation.FailureErrorResponse("El tipo de asunto no es válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetRequerimientoProdeconByAutorizacionAsync(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetComercioExteriorRequerimientoProdeconAsync(idAutorizacion);
                    return result;
                }

                return ResultOperation.FailureErrorResponse<List<ResponseAvisoSinRespuestaRelacionado>>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetRequerimientoProdeconById(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetComercioExteriorRequerimientoProdeconByIdDisconnected(id);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<ResponseAvisoSinRespuestaRelacionado>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PostRequerimientoProdecon(HttpContext context, RequestRequerimientoProdeconCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestCreateRequerimientoProdeconValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."
                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = EnumPathSub.AUTORIZACION.ToString();
                Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "La autorización no existe o fue eliminada."

                    );
                }

                if (request.documento is null)
                {
                    return
                            ResultOperation.FailureErrorResponse<int>(
                                "El documento es obligatorio."
                        );
                }

                if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine(_path1, path2, request.idAsunto.ToString()),
                        out var dataFile, out string message, requiredExtentions))
                {
                    return
                            ResultOperation.FailureErrorResponse<int>(
                                message

                        );
                }

                Documento entityDocumento = null!;
                RequerimientoProdecon entity = null!;
                try
                {
                    entity = ComercioExteriorRequerimientoProdeconEvents.Create(
                        entityExists,
                        request.numeroOficio,
                        request.numeroExpediente,
                        DateTime.Parse(request.fechaOficio),
                        DateTime.Parse(request.fechaIngresoSat),
                        request.requiereAccion,
                        request.atencion,
                        sessionInformation.UserInformation.Rfc!
                    );

                    entityDocumento = ArchivoEvents.CreateFolio(
                        request.numeroFolio!,
                        request.idAsunto,
                        request.idTipoArchivo.GetValueOrDefault(),
                        request.idSeccion.GetValueOrDefault(),
                        null!,
                        sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                        dataFile.File.FileName,
                        dataFile.FilePath,
                        dataFile.File!.ContentType,
                        _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                        sessionInformation.UserInformation.Rfc!,
                        sessionInformation.UserInformation.Rfc!,
                        true,
                        _roleSicoj.GetHashCode(),
                        null!
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.AddComercioExteriorRequerimientoProdecon(entity, entityDocumento, dataFile);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchRequerimientoProdecon(HttpContext context, RequestRequerimientoProdeconUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                var validationResult = await _requestUpdateRequerimientoProdeconValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "La autorización no existe o fue eliminada."

                    );
                }

                RequerimientoProdecon entity = await _adminAbogadoService.GetComercioExteriorRequerimientoProdeconById(request.id)!;
                if (entity is null || !entity.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "El requerimiento PRODECON no existe o fue eliminado."

                    );
                }

                try
                {
                    ComercioExteriorRequerimientoProdeconEvents.Update(
                        entity,
                        entityExists,
                        request.numeroOficio,
                        request.numeroExpediente,
                        DateTime.Parse(request.fechaOficio),
                        DateTime.Parse(request.fechaIngresoSat),
                        request.requiereAccion,
                        request.atencion,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.UpdateComercioExteriorRequerimientoProdecon(entity);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetRequerimientoByIdAutorizacion(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetRequerimientoByIdAutorizacionDisconnectedAsync(idAutorizacion);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<List<ResponseRequerimiento>>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetRequerimientoById(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetComercioExteriorRequerimientoByIdDisconnected(id);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<ResponseRequerimiento>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchNoRequerimiento(HttpContext context, RequestNoRequerimiento request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<bool>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<bool>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<bool>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse<bool>(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<bool>(
                            "La autorización no existe o fue eliminada."

                    );
                }

                try
                {
                    ComercioExteriorRequerimientoEvents.NoRequerimiento(
                        ref entityExists,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<bool>(_ex.Message);
                }

                ResultOperation result = await _adminAbogadoService.UpdateImpuestosInternosNoRequerimiento(entityExists);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PostRequerimiento(HttpContext context, RequestRequerimientoCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                Filters.ValidateContractValues(request);

                var validationResult = await _requestRequerimientoCreateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                if (request.documento is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El documento es obligatorio.");
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = EnumPathSub.AUTORIZACION.ToString();
                Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "La autorización no existe o fue eliminada."

                    );
                }

                if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine(_path1, path2, request.idAsunto.ToString()),
                        out var dataFile, out string message, requiredExtentions))
                {
                    return
                            ResultOperation.FailureErrorResponse<int>(
                                message

                        );
                }

                Requerimiento entity = null!;
                Documento entityDocumento = null!;
                try
                {
                    entity = ComercioExteriorRequerimientoEvents.Create(
                        ref entityExists,
                        request.oficioRequerimiento,
                        DateTime.Parse(request.fechaOficio),
                        sessionInformation.UserInformation.Rfc!
                    );


                    entityDocumento = ArchivoEvents.CreateFolio(
                            request.numeroFolio!,
                            request.idAsunto,
                            EnumTipoDocumentoCons.OFICIO_DE_REQUERIMIENTO,
                            EnumSeccionesCons.REQUERIMIENTO,
                            null!,
                            sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File!.ContentType,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            true,
                            _roleSicoj.GetHashCode(),
                            null!
                        );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.AddComercioExteriorRequerimiento(entityExists, entity, entityDocumento, dataFile);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchRequerimiento(HttpContext context, RequestRequerimientoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestRequerimientoUpdateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = EnumPathSub.AUTORIZACION.ToString();
                Autorizacion entityAutorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityAutorizacionExists is null || !entityAutorizacionExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "La autorización no existe o fue eliminada."

                    );
                }

                Requerimiento entityRequerimientoExists = await _adminAbogadoService.GetRequerimientoById(request.id)!;
                if (entityRequerimientoExists is null || !entityRequerimientoExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "El requerimiento no existe o fue eliminado."

                    );
                }

                var modificacionList = await _genericService.GetModificacionByIdAsunto(request.id, null!);
                Modificacion modificacion = null!;
                if (modificacionList is not null && modificacionList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.REQUERIMIENTO)))
                {
                    modificacion = modificacionList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.REQUERIMIENTO))!;
                    var requerimientosList = await _adminAbogadoService.GetRequerimientoByIdAutorizacion(request.idAsunto);
                    if (requerimientosList is null || !requerimientosList.Any())
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "No se encontraron los requerimientos del asunto."

                        );
                    }
                    var ultimoRequerimeinto = requerimientosList.OrderBy(c => c.id).LastOrDefault();
                    if (ultimoRequerimeinto is null || ultimoRequerimeinto.id != entityRequerimientoExists.id)
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "El requerimiento no se puede editar ya que no es el último registrado."

                        );
                    }
                }

                DataFile dataFile = null!;
                Documento entityDocumento = null!;
                Documento entityDocumentoAcuse = null!;
                Documento entityDocumentoEscrito = null!;
                List<Documento> listDocumentos = await _genericService.GetDocumentosByRenglonTipoAsync(
                        request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : request.idAsunto,
                        null,
                        EnumSecciones.REQUERIMIENTO.GetHashCode(), request.id, null!);
                if (listDocumentos is not null && listDocumentos.Any())
                {
                    entityDocumentoAcuse = listDocumentos.FirstOrDefault(c => c.activo && c.id_tipo_documento == EnumTipoDocumento.OFICIO_DE_NOTIFICACIÓN.GetHashCode())!;
                    entityDocumentoEscrito = listDocumentos.FirstOrDefault(c => c.activo && c.id_tipo_documento == EnumTipoDocumento.ESCRITO_DE_RESPUESTA.GetHashCode())!;
                }

                if (request.documento is not null ||
                    !string.IsNullOrEmpty(request.numeroFolio) ||
                    request.idTipoArchivo.GetValueOrDefault() > 0)
                {

                    switch (request.idTipoArchivo.GetValueOrDefault())
                    {
                        case EnumTipoDocumentoCons.OFICIO_DE_NOTIFICACIÓN:
                            if (entityDocumentoAcuse is not null)
                            {
                                return ResultOperation.FailureWarningResponse<int>("Los parametrós de documento no son requeridos por que ya existe un documento de ACUSE DE NOTIFICACIÓN.");
                            }
                            break;
                        case EnumTipoDocumentoCons.ESCRITO_DE_RESPUESTA:
                            if (entityDocumentoEscrito is not null)
                            {
                                return ResultOperation.FailureWarningResponse<int>("Los parametrós de documento no son requeridos por que ya existe un documento de ESCRITO DE RESPUESTA.");
                            }
                            break;
                    }

                    if (request.documento is null)
                    {
                        return ResultOperation.FailureErrorResponse<int>("El documento es obligatorio.");
                    }

                    var validationResultFile = await _requestRequerimientoUpdateOnlyFileValidator.ValidateAsync(request);
                    if (!validationResultFile.IsValid)
                    {
                        return ResultOperation.FailureWarningResponse<int>(validationResultFile.ToString(" - "));
                    }

                    if (request.documento is not null)
                    {
                        if (!_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine(_path1, path2, request.idAsunto.ToString()),
                            out dataFile, out string message, requiredExtentions, null!))
                        {
                            return
                                    ResultOperation.FailureErrorResponse<int>(
                                        message

                                );
                        }
                    }
                }

                try
                {
                    switch (request.idTipoAsunto)
                    {
                        case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                            ImpuestosInternosRequerimientoEvents.UpdateImpuestosInternos(
                                ref entityAutorizacionExists,
                                ref entityRequerimientoExists,
                                modificacion,
                                request.oficioRequerimiento,
                                DateTime.Parse(request.fechaOficio),
                                DateTime.Parse(request.fechaNotificacion),
                                request.atendioRequerimiento,
                                string.IsNullOrEmpty(request.fechaAtencion) ? null! : DateTime.Parse(request.fechaAtencion),
                                sessionInformation.UserInformation.Rfc!
                            );
                            break;
                        case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                        case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                            ImpuestosInternosRequerimientoEvents.UpdateImpuestosInternos(
                                ref entityAutorizacionExists,
                                ref entityRequerimientoExists,
                                modificacion,
                                request.oficioRequerimiento,
                                DateTime.Parse(request.fechaOficio),
                                DateTime.Parse(request.fechaNotificacion),
                                request.atendioRequerimiento,
                                string.IsNullOrEmpty(request.fechaAtencion) ? null! : DateTime.Parse(request.fechaAtencion),
                                sessionInformation.UserInformation.Rfc!
                            );
                            break;
                        case EnumTipoAsuntoConst.DONATARIAS:
                            ImpuestosInternosRequerimientoEvents.UpdateDonatarias(
                                ref entityAutorizacionExists,
                                ref entityRequerimientoExists,
                                modificacion,
                                request.oficioRequerimiento,
                                DateTime.Parse(request.fechaOficio),
                                DateTime.Parse(request.fechaNotificacion),
                                request.primeraProrroga,
                                request.segundaProrroga,
                                request.atendioRequerimiento,
                                string.IsNullOrEmpty(request.fechaAtencion) ? null! : DateTime.Parse(request.fechaAtencion),
                                sessionInformation.UserInformation.Rfc!
                            );
                            break;
                        default:
                            break;
                    }

                    if (dataFile is not null)
                    {
                        switch (request.idTipoArchivo.GetValueOrDefault())
                        {
                            case EnumTipoDocumentoCons.OFICIO_DE_NOTIFICACIÓN:
                                entityDocumento = ArchivoEvents.CreateFolio(
                                    request.numeroFolio!,
                                    request.idAsunto,
                                    request.idTipoArchivo.GetValueOrDefault(),
                                    EnumSecciones.REQUERIMIENTO.GetHashCode(),
                                    entityRequerimientoExists.id,
                                    sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                                    dataFile.File.FileName,
                                    dataFile.FilePath,
                                    dataFile.File!.ContentType,
                                    _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                                    sessionInformation.UserInformation.Rfc!,
                                    sessionInformation.UserInformation.Rfc!,
                                    true,
                                    _roleSicoj.GetHashCode(),
                                    null!
                                );
                                break;
                            case EnumTipoDocumentoCons.ESCRITO_DE_RESPUESTA:
                                entityDocumento = ArchivoEvents.CreateFolio(
                                    request.numeroFolio!,
                                    request.idAsunto,
                                    request.idTipoArchivo.GetValueOrDefault(),
                                    EnumSecciones.REQUERIMIENTO.GetHashCode(),
                                    entityRequerimientoExists.id,
                                    sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                                    dataFile.File.FileName,
                                    dataFile.FilePath,
                                    dataFile.File!.ContentType,
                                    _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                                    sessionInformation.UserInformation.Rfc!,
                                    sessionInformation.UserInformation.Rfc!,
                                    true,
                                    _roleSicoj.GetHashCode(),
                                    null!
                                );
                                break;
                        }
                    }
                    else
                    {
                        if (entityDocumentoAcuse is null && request.idTipoArchivo.GetValueOrDefault() == EnumTipoDocumentoCons.OFICIO_DE_NOTIFICACIÓN)
                        {
                            if (entityRequerimientoExists.fecha_notificacion is not null)
                            {
                                throw new Exception("El documento de ESCRITO DE RESPUESTA es requerido.");
                            }
                        }
                        if (entityDocumentoEscrito is null && request.idTipoArchivo.GetValueOrDefault() == EnumTipoDocumentoCons.ESCRITO_DE_RESPUESTA)
                        {
                            if (entityRequerimientoExists.atendio_requerimiento.GetValueOrDefault())
                            {
                                throw new Exception("El documento de ESCRITO DE RESPUESTA es requerido.");
                            }
                        }
                    }
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.UpdateComercioExteriorRequerimiento(entityAutorizacionExists, entityRequerimientoExists, entityDocumento!, dataFile!, EnumSeccionesCons.REQUERIMIENTO);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAvisosComunicadosByIdAutorizacion(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetComercioExteriorAvisosComunicadosAsync(idAutorizacion);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<List<ResponseRequerimiento>>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAvisosComunicadosById(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetComercioExteriorAvisosComunicadosByIdDisconnected(id);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<ResponseRequerimiento>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PostAvisosComunicados(HttpContext context, RequestAvisosComunicadosCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                Filters.ValidateContractValues(request);

                var validationResult = await _requestAvisosComunicadosCreateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                if (request.documento is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El documento es obligatorio.");
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = EnumPathSub.AUTORIZACION.ToString();
                Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityExists is null || !entityExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "La autorización no existe o fue eliminada."

                    );
                }

                if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine(_path1, path2, request.idAsunto.ToString()),
                        out var dataFile, out string message, requiredExtentions))
                {
                    return
                            ResultOperation.FailureErrorResponse<int>(
                                message

                        );
                }

                AvisosComunicados entity = null!;
                Documento entityDocumento = null!;
                try
                {
                    entity = ComercioExteriorAvisosComunicadosEvents.Create(
                        ref entityExists,
                        request.idTipoAviso,
                        request.numeroOficio,
                        request.sinNumeroOficio,
                        DateTime.Parse(request.fechaIngreso),
                        request.observaciones,
                        sessionInformation.UserInformation.Rfc!
                    );


                    entityDocumento = ArchivoEvents.CreateFolio(
                            request.numeroFolio!,
                            request.idAsunto,
                            EnumTipoDocumentoCons.AVISO_COMUNICADO,
                            EnumSeccionesCons.AVISOS_Y_COMUNICADOS,
                            null!,
                            sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File!.ContentType,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            true,
                            _roleSicoj.GetHashCode(),
                            null!
                        );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.AddComercioExteriorAvisosComunicados(entity, entityDocumento, dataFile);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchAvisosComunicados(HttpContext context, RequestAvisosComunicadosUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestAvisosComunicadosUpdateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = EnumPathSub.AUTORIZACION.ToString();
                Autorizacion entityAutorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                if (entityAutorizacionExists is null || !entityAutorizacionExists.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "La autorización no existe o fue eliminada."

                    );
                }

                AvisosComunicados entity = await _adminAbogadoService.GetComercioExteriorAvisosComunicadosById(request.id)!;
                if (entity is null || !entity.activo)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "El requerimiento no existe o fue eliminado."

                    );
                }


                try
                {
                    ComercioExteriorAvisosComunicadosEvents.Update(
                        ref entityAutorizacionExists,
                        ref entity,
                        request.requiereAccion,
                        request.atencion,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.UpdateComercioExteriorAvisosComunicados(entity);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetResolucionById(int idAsunto, int idTipoAsunto, int? idResolucion)
        {
            try
            {

                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetResolucionByIdAsuntoDisconnectedAsync(idAsunto, null!, idResolucion);
                    return result;
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var result = await _adminAbogadoService.GetResolucionByIdAsuntoDisconnectedAsync(null!, idAsunto, idResolucion);
                    return result;
                }
                return ResultOperation.FailureErrorResponse<ResponseResolucion>("Tipo de asunto no válido.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PostResolucion(HttpContext context, RequestResolucionCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestResolucionCreateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado el asunto."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "El asunto ya se encuentra en uso por otro usuario."

                    );
                }

                if (request.documento is null)
                {
                    return ResultOperation.FailureErrorResponse<int>("El documento es obligatorio.");
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = null!;
                int enumSeccionesCons = 0;
                Autorizacion autorizacionExists = null!;
                Cumplimentacion cumplimentacionExists = null!;
                Resolucion entityResolucionExists = null!;
                Descartar descartar = null!;
                EnumTipoDocumento tipoDocumento = default;
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    path2 = EnumPathSub.AUTORIZACION.ToString();
                    enumSeccionesCons = EnumSeccionesCons.EMISION_RESOLUCION;
                    tipoDocumento = EnumTipoDocumento.OFICIO_DE_LA_EMISIÓN_RESOLUCIÓN;
                    autorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (autorizacionExists is null || !autorizacionExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "El asunto no existe o fue eliminado."

                        );
                    }

                    var entityResolucionExistsList = await _adminAbogadoService.GetResolucionByIdAsuntoAsync(request.idAsunto, null!);
                    if (entityResolucionExistsList is not null && entityResolucionExistsList.Any(c => c.activo))
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "La autorización ya cuenta con una resolución."

                        );
                    }
                    if (entityResolucionExistsList is not null && entityResolucionExistsList.Any())
                    {
                        entityResolucionExists = entityResolucionExistsList!.LastOrDefault()!;
                    }
                        
                    var descartarList = await _genericService.GetDescartarByIdAsunto(request.idAsunto, null!);
                    if (descartarList is not null && descartarList.Any(c => c.activo))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo)!;
                    }
                }
                else if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    path2 = EnumPathSub.CUMPLIMENTACION.ToString();
                    enumSeccionesCons = EnumSeccionesCons.CUMPLIMENTACION;
                    tipoDocumento = EnumTipoDocumento.OFICIO_ACUERDO_DE_CONCLUSIÓN;
                    cumplimentacionExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (cumplimentacionExists is null || !cumplimentacionExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "El asunto no existe o fue eliminado."

                        );
                    }

                    var descartarList = await _genericService.GetDescartarByIdAsunto(null!, request.idAsunto);
                    if (descartarList is not null && descartarList.Any(c => c.activo))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo)!;
                    }
                }
                else
                {
                    return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                if (request.documento is null)
                {
                    return
                            ResultOperation.FailureErrorResponse<int>(
                                "El documento es obligatorio."

                        );
                }

                if (!_fileSystemService.FileTryOut(
                    request.documento,
                    Path.Combine(_path1, path2, request.idAsunto.ToString()),
                    out var dataFileOficio, out string message, requiredExtentions))
                {
                    return
                            ResultOperation.FailureErrorResponse<int>(
                                message

                        );
                }

                Resolucion entity = null!;
                Documento entityDocumentoOficio = null!;
                try
                {
                    entity = ResolucionEvents.Create(
                        ref autorizacionExists,
                        ref cumplimentacionExists!,
                        descartar,
                        entityResolucionExists!,
                        request.oficioResolucion,
                        DateTime.Parse(request.fechaOficio),
                        request.idSentido,
                        string.IsNullOrEmpty(request.fechaVencimiento) ? null : DateTime.Parse(request.fechaVencimiento),
                        sessionInformation.UserInformation.Rfc!
                        );

                    entityDocumentoOficio = ArchivoEvents.CreateFolio(
                        request.numeroFolio!,
                        entity.id_autorizacion,
                        tipoDocumento.GetHashCode(),
                        enumSeccionesCons,
                        null!,
                        sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                        dataFileOficio.File.FileName,
                        dataFileOficio.FilePath,
                        dataFileOficio.File!.ContentType,
                        _fileSystemService.ConvertBytesToMegaBytesString(dataFileOficio.File.Length),
                        sessionInformation.UserInformation.Rfc!,
                        sessionInformation.UserInformation.Rfc!,
                        true,
                        _roleSicoj.GetHashCode(),
                        entity.id_cumplimentacion
                        );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.AddImpuestosInternosResolucionAsync(autorizacionExists, cumplimentacionExists, entity, entityDocumentoOficio, dataFileOficio);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchResolucion(HttpContext context, RequestResolucionUpdate request)
        {
            try
            {
                _logger.LogInformationSicoj("Request: ", request);
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                var validationResult = await _requestResolucionUpdateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.DONATARIAS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                string path2 = null!;
                int enumSeccionesCons = 0;
                Autorizacion autorizacionExists = null!;
                Cumplimentacion cumplimentacionExists = null!;
                Resolucion entityResolucionExists = null!;
                Descartar descartar = null!;
                Modificacion modificacion = null!;
                EnumTipoDocumento enumTipoDocumento = default;
                bool primeraResolucion = true;
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    path2 = EnumPathSub.AUTORIZACION.ToString();
                    enumSeccionesCons = EnumSeccionesCons.EMISION_RESOLUCION;
                    enumTipoDocumento = EnumTipoDocumento.ACUSE_DE_NOTIFICACIÓN;
                    autorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (autorizacionExists is null || !autorizacionExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "El asunto no existe o fue eliminado."

                        );
                    }

                    var entityResolucionExistsList = await _adminAbogadoService.GetResolucionByIdAsuntoAsync(request.idAsunto, null!);
                    if (entityResolucionExistsList is null || !entityResolucionExistsList.Any(c => c.activo))
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "La resolución no existe o fue eliminada"

                        );
                    }

                    entityResolucionExists = entityResolucionExistsList!.FirstOrDefault(c => c.activo)!;

                    var descartarList = await _genericService.GetDescartarByIdAsunto(request.idAsunto, null!);
                    if (descartarList is not null && descartarList.Any(c => c.activo))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo)!;
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.idAsunto, null!);
                    if (modificacionList is not null && modificacionList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.EMISION_RESOLUCION)))
                    {
                        modificacion = modificacionList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.EMISION_RESOLUCION))!;
                    }               
                }
                else if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    if (request.idTipoArchivo is not null)
                    {
                        if (!request.idTipoArchivo.Equals(EnumTipoDocumentoCons.ACUSE_DE_NOTIFICACIÓN_CUMPLIMENTACIÓN) && 
                            !request.idTipoArchivo.Equals(EnumTipoDocumentoCons.OFICIO_ACUERDO_DE_CONCLUSIÓN_CUMPLIMENTACIÓN))
                        {
                            return
                            ResultOperation.FailureErrorResponse<int>(
                                "Tipo documento no válido para cumplimentación"

                            );
                        }

                        enumTipoDocumento = request.idTipoArchivo.Equals(EnumTipoDocumentoCons.ACUSE_DE_NOTIFICACIÓN_CUMPLIMENTACIÓN) ?
                            EnumTipoDocumento.ACUSE_DE_NOTIFICACIÓN_CUMPLIMENTACIÓN :
                            EnumTipoDocumento.OFICIO_ACUERDO_DE_CONCLUSIÓN_CUMPLIMENTACIÓN;
                    }
                    path2 = EnumPathSub.CUMPLIMENTACION.ToString();
                    enumSeccionesCons = EnumSeccionesCons.CUMPLIMENTACION;
                    cumplimentacionExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (cumplimentacionExists is null || !cumplimentacionExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "El asunto no existe o fue eliminado."

                        );
                    }

                    var entityResolucionExistsList = await _adminAbogadoService.GetResolucionByIdAsuntoAsync(null!, request.idAsunto);
                    if (entityResolucionExistsList is null || !entityResolucionExistsList.Any(c => c.activo && c.id.Equals(request.id)))
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "La resolución no existe o fue eliminada"

                        );
                    }
                    entityResolucionExists = entityResolucionExistsList!.FirstOrDefault(c => c.activo && c.id.Equals(request.id.GetValueOrDefault(0)))!;
                    var primerResolucionExits = entityResolucionExistsList.Where(c => c.activo).OrderBy(c => c.id).FirstOrDefault();
                    primeraResolucion = primerResolucionExits!.id == request.id;
                    var descartarList = await _genericService.GetDescartarByIdAsunto(null!, request.idAsunto);
                    if (descartarList is not null && descartarList.Any(c => c.activo))
                    {
                        descartar = descartarList.FirstOrDefault(c => c.activo)!;
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(null!, request.idAsunto);
                    if (modificacionList is not null && modificacionList.Any(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.EMISION_RESOLUCION)))
                    {
                        modificacion = modificacionList.FirstOrDefault(c => c.activo && c.id_seccion.Equals(EnumSeccionesCons.EMISION_RESOLUCION))!;
                    }
                }
                else
                {
                    return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                Documento entityDocumento = null!;
                
                if (request.documento is not null ||
                    !string.IsNullOrEmpty(request.numeroFolio))
                {
                    var listDocumentos = await _genericService.GetDocumentosByRenglonTipoAsync(
                        entityResolucionExists!.id_autorizacion,
                        entityResolucionExists.id_cumplimentacion,
                        enumSeccionesCons, entityResolucionExists.id, enumTipoDocumento.GetHashCode());
                    if (listDocumentos is not null && listDocumentos.Any(c => c.activo))
                    {
                        return
                                ResultOperation.FailureErrorResponse<int>(
                                    "Ya existe registradp un documento con el mismo tipo"

                            );
                    }
                }

                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine(_path1, path2, request.idAsunto.ToString()),
                        out dataFile, out string message, requiredExtentions, null!))
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                message

                            );
                    }
                }

                try
                {
                    ResolucionEvents.Update(
                        ref autorizacionExists,
                        ref cumplimentacionExists,
                        ref entityResolucionExists,
                        modificacion,
                        request.oficioResolucion,
                        DateTime.Parse(request.fechaOficio),
                        request.idSentido,
                        DateTime.Parse(request.fechaNotificacion),
                        sessionInformation.UserInformation.Rfc!,
                        primeraResolucion,
                        string.IsNullOrEmpty(request.fechaVencimiento) ? null! : DateTime.Parse(request.fechaVencimiento)
                        );

                    if (dataFile is not null)
                    {
                        if (entityResolucionExists.fecha_notificacion is null)
                        {
                            throw new Exception("Para poder cargar el documento es necesario registrar la fecha notificación.");
                        }

                        entityDocumento = ArchivoEvents.CreateFolio(
                            request.numeroFolio!,
                            entityResolucionExists.id_autorizacion,
                            enumTipoDocumento.GetHashCode(),
                            enumSeccionesCons,
                            entityResolucionExists.id,
                            sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File!.ContentType,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            true,
                            _roleSicoj.GetHashCode(),
                            entityResolucionExists.id_cumplimentacion!
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.UpdateImpuestosInternosResolucionAsync(autorizacionExists, cumplimentacionExists, entityResolucionExists, entityDocumento!, dataFile!, EnumSeccionesCons.EMISION_RESOLUCION);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchConcluir(HttpContext context, RequestConcluir request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."

                    );
                }

                var validationResult = await _requestConcluirValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - "));
                }

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.DONATARIAS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."

                    );
                }

                Autorizacion autorizacionExists = null!;
                Cumplimentacion cumplimentacionExists = null!;
                Resolucion entityResolucionExits = null!;
                AvisoConRespuesta avisoConRespuestaExists = null!;

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    autorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (autorizacionExists is null || !autorizacionExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "El asunto no existe o fue eliminado."

                        );
                    }
                    var entityResolucionExitsList = await _adminAbogadoService.GetResolucionByIdAsuntoAsync(request.idAsunto, null!);
                    if (entityResolucionExitsList is null || !entityResolucionExitsList.Any(c => c.activo))
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "La resolución no existe o fue eliminada."

                        );
                    }

                    entityResolucionExits = entityResolucionExitsList!.FirstOrDefault(c => c.activo)!;


                    if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                    {
                        avisoConRespuestaExists = await _genericService.GetAvisoConRespuestaByIdAutorizacion(request.idAsunto);
                        if (avisoConRespuestaExists is null || !avisoConRespuestaExists.activo)
                        {
                            return
                                ResultOperation.FailureErrorResponse<int>(
                                    "El aviso con respuesta no existe o fue eliminado."

                            );
                        }
                    }
                }
                else if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    cumplimentacionExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (cumplimentacionExists is null || !cumplimentacionExists.activo)
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "El asunto no existe o fue eliminado."

                        );
                    }
                    var entityResolucionExitsList = await _adminAbogadoService.GetResolucionByIdAsuntoAsync(null!, request.idAsunto);
                    if (entityResolucionExitsList is null || !entityResolucionExitsList.Any(c => c.activo && c.id.Equals(request.id.GetValueOrDefault(0))))
                    {
                        return
                            ResultOperation.FailureErrorResponse<int>(
                                "La resolución no existe o fue eliminada."

                        );
                    }

                    entityResolucionExits = entityResolucionExitsList.FirstOrDefault(c => c.activo && c.id.Equals(request.id.GetValueOrDefault(0)))!;
                }
                else
                {
                    return ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido.");
                }

                try
                {
                    ResolucionEvents.Concluir(
                        ref autorizacionExists,
                        ref cumplimentacionExists,
                        avisoConRespuestaExists,
                        entityResolucionExits,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse<int>(_ex.Message);
                }

                ResultOperation<int> result = await _adminAbogadoService.UpdateConcluirAsync(autorizacionExists, cumplimentacionExists);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAvisoConRespuestaRelacionadoAsync(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _adminAbogadoService.GetAvisoConRespuestaRelacionadoAsync(idAutorizacion);
                    return result;
                }

                return ResultOperation.SuccessResponseNoMessage<List<ResponseAvisoConRespuesta>>(new());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetCumplimentacionById(int id)
        {
            try
            {
                var result = await _adminAbogadoService.GetCumplimentacionByIdDisconnectedAsync(id);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PatchCumplimentacion(HttpContext context, RequestCumplimentacionAbogadoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(context);
                if (sessionInformation is null)
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."

                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.id}");
                if (keyExists is null)
                {
                    return
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado."
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."

                    );
                }

                var entityExists = await _genericService.GetCumplimentacionById(request.id, null!);
                if (entityExists is null || !entityExists.activo || entityExists.id_tipo_modalidad != EnumTipoModalidad.FÍSICO.GetHashCode())
                {
                    return
                        ResultOperation.FailureErrorResponse(
                            "La cumplimentación no existe o fue eliminada."

                    );
                }


                var autorizacionExists = await _genericService.GetAutorizacionById(entityExists.id_autorizacion);

                try
                {
                    CumplimentaciónEvents.UpdateCumplimentacion(
                        ref autorizacionExists,
                        ref entityExists,
                        request.idTipoAsuntoCumplimentar,
                        request.rfc,
                        request.promovente,
                        request.promoventeNoContribuyente,
                        request.rfcContribuyente,
                        request.contribuyente,
                        request.noJuicioRecuroAmparo,
                        string.IsNullOrWhiteSpace(request.fechaRecepcionSolicitud) ? null : DateTime.Parse(request.fechaRecepcionSolicitud),
                        string.IsNullOrWhiteSpace(request.fechaFirmeza) ? null : DateTime.Parse(request.fechaFirmeza),
                        request.idPlazoCumplimiento,
                        string.IsNullOrWhiteSpace(request.fechaVencimiento) ? null : DateTime.Parse(request.fechaVencimiento),
                        request.idOrganoJurisdiccional,
                        request.organoJurisdiccional,
                        request.idUnidadAdministrativaCumplimiento,
                        request.unidadAdministrativaCumplimiento,
                        request.oficioResolucionImpugnada,
                        string.IsNullOrWhiteSpace(request.fechaOficioResolucionImpugnada) ? null : DateTime.Parse(request.fechaOficioResolucionImpugnada),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return ResultOperation.FailureWarningResponse(_ex.Message);
                }
                var result = await _adminAbogadoService.UpdateCumplimentacion(autorizacionExists, entityExists, sessionInformation.UserInformation);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
