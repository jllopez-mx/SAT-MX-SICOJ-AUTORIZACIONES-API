using AutorizacionesAPI.Model.DTO.ContractsValidations;
using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities.Events.OficialPartes;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.Files;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Enums;
using Mapster;
using Sicoj.Utils.ViewModels;
using AutorizacionesAPI.Model.Entities.Events.Genericos;
using AutorizacionesAPI.Model.DTO.RequestFilters;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.LoggerConfiguration;

namespace AutorizacionesAPI.Api.V1.Controllers
{
    [Route("sicoj/autorizaciones/api/v1/oficial-partes/autorizaciones")]
    [ApiController]
    public class OficialPartesController : ControllerBase
    {
        #region Variables / Contructor
        private readonly ILogger<OficialPartesController> _logger;
        private readonly IValidator<RequestComercioExteriorFisicoCreate> _validatorCreateComercioExteriorFisico;
        private readonly RequestUpdateAutorizacionesComercioExteriorOficialPartesLineaValidator _updateComercioExteriorLineaValidator;
        private readonly RequestUpdateAutorizacionesComercioExteriorOficialPartesFisicoValidator _updateComercioExteriorFisicoValidator;
        private readonly IValidator<RequestImpuestosInternosFisicoCreate> _validatorCreateImpuestosInternosFisico;
        private readonly RequestUpdateAutorizacionesImpuestosInternosOficialPartesFisicoValidator _updateImpuestosInternosFisicoValidator;
        private readonly RequestUpdateAutorizacionesImpuestosInternosOficialPartesLineaValidator _updateImpuestosInternosLineaValidator;
        private readonly IValidator<RequestDocumentoFolioCreate> _createArchivoValidator;
        private readonly IValidator<RequestDocumentoFolioListCreate> _createDocumentoFolioListValidator;
        private readonly IValidator<RequestDocumentoFolio> _documentoFolioValidator;
        private readonly IValidator<RequestDocumentoUpdate> _requestDocumentoUpdateValidator;
        private readonly IValidator<RequestCumplimentacionOficialPartesCreate> _requestCumplimentacionCreateValidator;
        private readonly IOficialPartesService _service;
        private readonly IRedisClient _redisClient;
        private readonly IFileSystemService _fileSystemService;
        private readonly IGenericService _genericService;

        public OficialPartesController(ILogger<OficialPartesController> logger, 
            IValidator<RequestComercioExteriorFisicoCreate> validatorCreateComercioExteriorFisico, 
            RequestUpdateAutorizacionesComercioExteriorOficialPartesLineaValidator updateComercioExteriorLineaValidator, 
            RequestUpdateAutorizacionesComercioExteriorOficialPartesFisicoValidator updateComercioExteriorFisicoValidator, 
            IValidator<RequestImpuestosInternosFisicoCreate> validatorCreateImpuestosInternosFisico, 
            RequestUpdateAutorizacionesImpuestosInternosOficialPartesFisicoValidator updateImpuestosInternosFisicoValidator, 
            RequestUpdateAutorizacionesImpuestosInternosOficialPartesLineaValidator updateImpuestosInternosLineaValidator, 
            IValidator<RequestDocumentoFolioCreate> createArchivoValidator, 
            IValidator<RequestDocumentoFolioListCreate> createDocumentoFolioListValidator, 
            IValidator<RequestDocumentoFolio> documentoFolioValidator, 
            IOficialPartesService service, 
            IValidator<RequestDocumentoUpdate> requestDocumentoUpdateValidator, 
            IValidator<RequestCumplimentacionOficialPartesCreate> requestCumplimentacionCreateValidator,
            IRedisClient redisClient, 
            IFileSystemService fileSystemService, 
            IGenericService genericService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validatorCreateComercioExteriorFisico = validatorCreateComercioExteriorFisico ?? throw new ArgumentNullException(nameof(validatorCreateComercioExteriorFisico));
            _updateComercioExteriorLineaValidator = updateComercioExteriorLineaValidator ?? throw new ArgumentNullException(nameof(updateComercioExteriorLineaValidator));
            _updateComercioExteriorFisicoValidator = updateComercioExteriorFisicoValidator ?? throw new ArgumentNullException(nameof(updateComercioExteriorFisicoValidator));
            _validatorCreateImpuestosInternosFisico = validatorCreateImpuestosInternosFisico ?? throw new ArgumentNullException(nameof(validatorCreateImpuestosInternosFisico));
            _updateImpuestosInternosFisicoValidator = updateImpuestosInternosFisicoValidator ?? throw new ArgumentNullException(nameof(updateImpuestosInternosFisicoValidator));
            _updateImpuestosInternosLineaValidator = updateImpuestosInternosLineaValidator ?? throw new ArgumentNullException(nameof(updateImpuestosInternosLineaValidator));
            _createArchivoValidator = createArchivoValidator ?? throw new ArgumentNullException(nameof(createArchivoValidator));
            _createDocumentoFolioListValidator = createDocumentoFolioListValidator ?? throw new ArgumentNullException(nameof(createDocumentoFolioListValidator));
            _documentoFolioValidator = documentoFolioValidator ?? throw new ArgumentNullException(nameof(documentoFolioValidator));
            _requestDocumentoUpdateValidator = requestDocumentoUpdateValidator ?? throw new ArgumentNullException(nameof(requestDocumentoUpdateValidator));
            _requestCumplimentacionCreateValidator = requestCumplimentacionCreateValidator ?? throw new ArgumentNullException(nameof(requestCumplimentacionCreateValidator));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
        }
        #endregion

        #region Generales
        /// <summary>
        /// Método para la bandeja de pendientes del oficial de partes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesOficialPartesBandeja>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetBandejaPendientesAsync(
            [FromQuery] PagerQuery request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                if (!Filters.MapSort<EnumOrderColumnAutorizacionesByFiltros>(request, EnumOrderColumnAutorizacionesByFiltros.ByFechaRecepcionAsc.ToString(), true, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesOficialPartesBandeja>>("La columna de ordenamiento no es válida.")
                        );
                }

                var result = await _service.GetBandejaPendientesAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    sessionInformation.UserInformation);

                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        /// <summary>
        /// Método para obtener el historico de autorizaciones para el oficial de partes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesOficialPartesHistorico>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("historico")]
        public async Task<IActionResult> GetHistorico([FromQuery] PagerQueryFilters request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                RequestOficialPartesHistoricoFilters filters = new();
                Filters.MapFilters(request, filters);

                if (!Filters.MapSort<EnumOrderColumnAutorizacionesByFiltros>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesOficialPartesHistorico>>("La columna de ordenamiento no es válida."));
                }

                var result = await _service.GetHistoricoAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    Filters.GetStringValue(filters!.ByNoAsunto.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionHasta.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByRfc.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByPromovente.FirstOrDefault()),
                    filters!.ByIdTipoAsunto.Adapt<List<int>>(),
                    filters.ByIdEstadoTarea.Adapt<List<int>>(),
                    filters.ByIdTipoModalidad.Adapt<List<int>>(),
                    filters.ByIdEstadoProcesal.Adapt<List<int>>(),
                    sessionInformation.UserInformation);

                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id, int idTipoAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                string key = null!;
                switch (idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{id}";
                        break;
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{id}";
                        break;
                    case EnumTipoAsuntoConst.DONATARIAS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{id}";
                        break;
                    default:
                        return Ok(ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var entityExists = await _genericService.GetAutorizacionById(id);
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "La autorización no existe o fue eliminada."
                        )
                    );
                }

                try
                {
                    AutorizacionOficialPartesEvents.UpdateDelete(ref entityExists,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _service.DeleteAutorizacionAsync(entityExists);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }
        #endregion

        #region Comercio Exterior
        /// <summary>
        ///Método para consultar Autorizaciones de Comercio Exterior por Id
        /// </summary>
        /// <param name="id">Id Autorización Comercio Exterior</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseComercioExteriorOficialPartesById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("comercio-exterior/{id}")]
        public async Task<IActionResult> GetComercioExteriorById(int id)
        {
            try
            {
                var result = await _service.GetAutorizacionByIdDisconnected(id);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        /// <summary>
        /// Método para agregar una autorización de comercio exterior de modalidad física
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("comercio-exterior")]
        public async Task<IActionResult> PostComercioExteriorValidaciones(RequestComercioExteriorFisicoCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var validationResult = await _validatorCreateComercioExteriorFisico.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" -- ")));
                }

                Autorizacion entity = null!;
                try
                {
                    entity = AutorizacionOficialPartesEvents.CreateModalidadFisico(
                        EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR,
                        request.IdTipoAutorizacion,
                        request.Rfc,
                        request.Promovente,
                        request.PromoventeNoContribuyente,
                        request.RfcContribuyente,
                        request.Contribuyente,
                        request.DespachosAutorizados,
                        request.IdAutoridadDirigida!,
                        request.OtroAutoridadDirigida,
                        request.DomicilioPromovente,
                        request.DomicilioNotificaciones,
                        DateTime.Parse(request.FechaPresentacion),
                        request.IdFundamentoSolicitud!,
                        request.OtroFundamentoSolicitud,
                        request.IdTema!,
                        request.OtroTema,
                        request.NoIndicaMonto,
                        request.Monto,
                        DateTime.Parse(request.FechaRecepcion),
                        sessionInformation.UserInformation.IdAdministracionCentral,
                        request.IdUnidadAdministrativa!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                ResultOperation<int> result = await _service.AddAutorizacionFisico(entity, sessionInformation.UserInformation);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        /// <summary>
        /// Método para actualizar autorizaciones de comercio exterior de modalida físicia y en linea
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("comercio-exterior")]
        public async Task<IActionResult> PatchAutorizacionesComercioExteriorFisico(RequestComercioExteriorOficialPartesUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.Id}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                if (request.IdTipoModalidad == EnumTipoModalidad.FÍSICO.GetHashCode())
                {
                    var validationResult = await _updateComercioExteriorFisicoValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || entityExists.id_tipo_modalidad != EnumTipoModalidad.FÍSICO.GetHashCode())
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe."
                            )
                        );
                    }

                    try
                    {
                        AutorizacionOficialPartesEvents.UpdateGuardarModalidadFisico(ref entityExists,
                            request.IdTipoAutorizacion,
                            request.Rfc,
                            request.Promovente,
                            request.PromoventeNoContribuyente,
                            request.RfcContribuyente,
                            request.Contribuyente,
                            request.DespachosAutorizados,
                            request.IdAutoridadDirigida!,
                            request.OtroAutoridadDirigida,
                            request.DomicilioPromovente,
                            request.DomicilioNotificaciones,
                            DateTime.Parse(request.FechaPresentacion),
                            request.IdFundamentoSolicitud!,
                            request.OtroFundamentoSolicitud,
                            request.IdTema!,
                            request.OtroTema,
                            request.NoIndicaMonto,
                            request.Monto,
                            DateTime.Parse(request.FechaRecepcion),
                            request.IdUnidadAdministrativa!,
                            sessionInformation.UserInformation.Rfc
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.UpdateAutorizacion(entityExists, sessionInformation.UserInformation);
                    return Ok(result);
                }
                else if (request.IdTipoModalidad == EnumTipoModalidad.LÍNEA.GetHashCode())
                {
                    var validationResult = await _updateComercioExteriorLineaValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || entityExists.id_tipo_modalidad != EnumTipoModalidad.LÍNEA.GetHashCode())
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe."
                            )
                        );
                    }

                    try
                    {
                        AutorizacionOficialPartesEvents.UpdateGuardarModalidadLinea(
                        ref entityExists,
                        request.PromoventeNoContribuyente,
                        request.RfcContribuyente,
                        request.Contribuyente,
                        request.DomicilioNotificaciones,
                        request.IdFundamentoSolicitud!,
                        request.OtroFundamentoSolicitud,
                        DateTime.Parse(request.FechaRecepcion),
                        request.IdUnidadAdministrativa!,
                        sessionInformation.UserInformation.Rfc
                    );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.UpdateAutorizacion(entityExists, sessionInformation.UserInformation);
                    return Ok(result);
                }

                return Ok(ResultOperation.FailureWarningResponse("El tipo de modalidad no es válida."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }
        #endregion

        #region Impuestos Internos
        /// <summary>
        ///Método para consultar Autorizaciones de Impuestos Internos por Id
        /// </summary>
        /// <param name="id">Id Autorización Impuestos Internos</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseImpuestosInternosById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("impuestos-internos/{id}")]
        public async Task<IActionResult> GetImpuestosInternosById(int id)
        {
            try
            {
                var result = await _service.GetAutorizacionByIdDisconnected(id);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        /// <summary>
        /// Método para crear Autorizaciones de Impuestos Internos de módadlida física
        /// </summary>
        /// <param name="request">Datos de Autorizaciones Impuestos Internos</param>
        /// <returns>Result Operation con Id de registro creado</returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("impuestos-internos")]
        public async Task<IActionResult> PostImpuestosInternos(RequestImpuestosInternosFisicoCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                _logger.LogInformationSicoj("Request: ", request);
                var validationResult = await _validatorCreateImpuestosInternosFisico.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" -- ")));
                }

                Autorizacion entity = null!;
                try
                {
                    entity = AutorizacionOficialPartesEvents.CreateModalidadFisico(
                        request.IdTipoAsunto,
                        null!,
                        request.Rfc,
                        request.Promovente,
                        request.PromoventeNoContribuyente,
                        request.RfcContribuyente,
                        request.Contribuyente,
                        null!,
                        null!,
                        null!,
                        request.DomicilioPromovente,
                        null!,
                        DateTime.Parse(request.FechaPresentacion),
                        null!,
                        null!,
                        null!,
                        null!,
                        null!,
                        null!,
                        DateTime.Parse(request.FechaRecepcion),
                        sessionInformation.UserInformation.IdAdministracionCentral,
                        request.IdUnidadAdministrativa!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                var result = await _service.AddAutorizacionFisico(entity, sessionInformation.UserInformation);
                _logger.LogInformationSicoj("Response: ", result);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        /// <summary>
        /// Metodo para actualizar autorizaciones de impuestos internos de modalida físicia y en linea
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("impuestos-internos")]
        public async Task<IActionResult> PatchAutorizacionesImpuestosInternos(RequestImpuestosInternosOficialPartesUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.Id}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                if (request.IdTipoModalidad == EnumTipoModalidad.FÍSICO.GetHashCode())
                {
                    var validationResult = await _updateImpuestosInternosFisicoValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || !entityExists.activo || entityExists.id_tipo_modalidad != EnumTipoModalidad.FÍSICO.GetHashCode())
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    try
                    {
                        AutorizacionOficialPartesEvents.UpdateGuardarModalidadFisico(
                            ref entityExists,
                            null!,
                            request.Rfc,
                            request.Promovente,
                            request.PromoventeNoContribuyente,
                            request.RfcContribuyente,
                            request.Contribuyente,
                            null!,
                            null!,
                            null!,
                            request.DomicilioPromovente,
                            null!,
                            DateTime.Parse(request.FechaPresentacion),
                            null!,
                            null!,
                            null!,
                            null!,
                            null!,
                            null!,
                            DateTime.Parse(request.FechaRecepcion),
                            request.IdUnidadAdministrativa!,
                            sessionInformation.UserInformation.Rfc
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.UpdateAutorizacion(entityExists, sessionInformation.UserInformation);
                    return Ok(result);
                }
                else if (request.IdTipoModalidad == EnumTipoModalidad.LÍNEA.GetHashCode())
                {
                    var validationResult = await _updateImpuestosInternosLineaValidator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                    }
                    var entityExists = await _genericService.GetAutorizacionById(request.Id);
                    if (entityExists is null || !entityExists.activo || entityExists.id_tipo_modalidad != EnumTipoModalidad.LÍNEA.GetHashCode())
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    try
                    {
                        AutorizacionOficialPartesEvents.UpdateGuardarModalidadLinea(
                            ref entityExists,
                            request.PromoventeNoContribuyente,
                            request.RfcContribuyente,
                            request.Contribuyente,
                            null,
                            null,
                            null,
                            DateTime.Parse(request.FechaRecepcion),
                            request.IdUnidadAdministrativa!,
                            sessionInformation.UserInformation.Rfc
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.UpdateAutorizacion(entityExists, sessionInformation.UserInformation);
                    return Ok(result);
                }

                return Ok(ResultOperation.FailureWarningResponse("El tipo de modalidad no es válida."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }
        #endregion

        [ProducesResponseType(typeof(ResultOperation<string>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("tomar/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> PatchTomar(int id, int idTipoAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                EnumModulosRedis enumModulo = default!;
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(id);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                            )
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
                        default:
                            return Ok(ResultOperation.FailureErrorResponse<string>("Tipo de asunto no existe."));
                    }
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    Cumplimentacion entityExists = await _genericService.GetCumplimentacionById(id, null!);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La cumplimentación no existe o fue eliminada."
                            )
                        );
                    }
                    enumModulo = EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION;
                }
                else
                    return Ok(ResultOperation.FailureErrorResponse<string>("No existe el tipo de asunto."));

                var response = await _redisClient.Take(enumModulo, id, sessionInformation.UserInformation.Rfc!, sessionInformation.UserInformation.Nombre!);
                return Ok(response);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("soltar/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> PatchSoltar(int id, int idTipoAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                EnumModulosRedis enumModulo = default!;
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(id);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                            )
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
                        default:
                            return Ok(ResultOperation.FailureErrorResponse<string>("Tipo de asunto no existe."));
                    }
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    Cumplimentacion entityExists = await _genericService.GetCumplimentacionById(id, null!);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La cumplimentación no existe o fue eliminada."
                            )
                        );
                    }
                    enumModulo = EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION;
                }
                else
                    return Ok(ResultOperation.FailureErrorResponse<string>("No existe el tipo de asunto."));

                var result = await _redisClient.Drop(enumModulo, id, sessionInformation.UserInformation.Rfc!);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("turnar/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> PatchTurnar(int id, int idTipoAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                string key = null!;
                if (idTipoAsunto == EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR)
                {
                    key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{id}";
                }
                else if (idTipoAsunto == EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS || idTipoAsunto == EnumTipoAsuntoConst.DONATARIAS)
                {
                    key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{id}";
                }
                else if (idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION)
                {
                    key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{id}";
                }
                else
                {
                    return Ok(ResultOperation.FailureErrorResponse<string>("No existe el tipo de asunto."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha apartado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    var entityExists = await _genericService.GetAutorizacionById(id);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    try
                    {
                        AutorizacionOficialPartesEvents.UpdateTurnar(ref entityExists,
                            sessionInformation.TokenInfomation.workforceID,
                            sessionInformation.UserInformation.Rfc
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.TurnarAsync(entityExists);
                    return Ok(result);
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var entityExists = await _genericService.GetCumplimentacionById(id, null!);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La cumplimentación no existe o fue eliminada."
                            )
                        );
                    }

                    try
                    {
                        CumplimentacionOficialPartesEvents.UpdateTurnarCumplimentacion(ref entityExists,
                            sessionInformation.TokenInfomation.workforceID,
                            sessionInformation.UserInformation.Rfc
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.TurnarCumplimentacionAsync(entityExists);
                    return Ok(result);
                }
                return Ok(ResultOperation.FailureWarningResponse("El tipo de asunto no es válido."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        #region Archivos
        [ProducesResponseType(typeof(ResultOperation<ResponseDocumentoHistorico>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("archivo/historico")]
        public async Task<IActionResult> GetDocumentosHistoricoAsync([FromQuery] RequestPagerQueryDocumentosHistoricoFilters request)
        {
            try
            {
                if (!Filters.MapSort<EnumOrderColumnDocumentosByFiltros>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseDocumentoHistorico>>("La columna de ordenamiento no es válida."));
                }
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var result = await _genericService.GetDocumentosAsync(
                        request.fetch,
                        request.page,
                        orderByColumn,
                        orderDesc,
                        request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : request.idAsunto,
                    request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? request.idAsunto : null);
                    return Ok(result);
                }

                return BadRequest("El tipo de asunto no es válido.");
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<DataTableView<ResponseDocumentoHistorico>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("archivo/historico/seccion/{idAsunto}/{idTipoAsunto}/{idSeccion}/{idRenglonSeccion}")]
        public async Task<IActionResult> GetDocumentosHistoricoSeccionAsync(int idAsunto, int idTipoAsunto, int idSeccion, int? idRenglonSeccion)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var result = await _genericService.GetDocumentosSeccionAsync(
                    idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? null : idAsunto,
                    idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION ? idAsunto : null,
                    idSeccion,
                    idRenglonSeccion.GetValueOrDefault(0) == 0 ? null! : idRenglonSeccion);
                    return Ok(result);
                }

                return BadRequest("El tipo de asunto no es válido.");
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                       error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<DataTableView<ResponseDocumentoHistorico>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("archivo/historico/seccion/paginado")]
        public async Task<IActionResult> GetDocumentosHistoricoSeccionPaginadoAsync([FromQuery] RequestPagerQueryDocumentosHistoricoSeccionFilters request)
        {
            try
            {
                if (!Filters.MapSort<EnumOrderColumnDocumentosByFiltros>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<DataTableView<ResponseDocumentoHistorico>>("La columna de ordenamiento no es válida."));
                }
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
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
                    return Ok(result);
                }

                return BadRequest("El tipo de asunto no es válido.");
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("archivo/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetDescargarArchivo(int id, int idTipoAsunto)
        {
            try
            {
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var entityDocumentoList = await _genericService.GetArchivosAutorizacionesByIds(new int[] { id });
                    if (entityDocumentoList is null || !entityDocumentoList.Any(c => c.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "No existe el documento o fue eliminado."
                            )
                        );
                    }

                    var entityDocumento = entityDocumentoList.FirstOrDefault(c => c.activo);
                    var response = await _fileSystemService.GetFileAsync(entityDocumento!.file_path);
                    if (response is null)
                    {
                        return BadRequest("No existe el documento.");
                    }

                    var contentDisposition = new System.Net.Mime.ContentDisposition
                    {
                        Inline = true,
                        FileName = Path.GetFileName(entityDocumento.file_path)
                    };

                    Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
                    return File(response, entityDocumento.content_type);
                }                
                return BadRequest("El tipo de asunto no es válido.");
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("archivo")]
        public async Task<IActionResult> PostDocumento([FromForm] RequestDocumentoFolioCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _createArchivoValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
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
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        break;
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha apartado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
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

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    if (request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION)
                    {
                        var entityCumplimentacion = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                        if (entityCumplimentacion is null)
                        {
                            return Ok(
                                ResultOperation.FailureErrorResponse(
                                    "La cumplimentación no existe."
                                )
                            );
                        }
                    }
                    else
                    {
                        var entityExists = await _genericService.GetAutorizacion(request.idAsunto);
                        if (entityExists is null)
                        {
                            return Ok(
                                ResultOperation.FailureErrorResponse(
                                    "La autorización no existe."
                                )
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
                            Path.Combine("AUTORIZACIONES", "COMERCIO EXTERIOR", request.idAsunto.ToString()!),
                            out var dataFile, out string message, requiredExtentions))
                    {
                        return Ok(
                                ResultOperation.FailureErrorResponse<int>(
                                    message
                                )
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
                            EnumRolesSicoj.OFICIAL_DE_PARTES.GetHashCode(),
                            idCumplimentacion ? request.idAsunto : null
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _genericService.AddArchivo(entity, dataFile);
                    return Ok(result);
                }                
                return Ok(ResultOperation.FailureWarningResponse("El tipo de modalidad no es válida."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("archivo")]
        public async Task<IActionResult> PatchDocumento([FromForm] RequestDocumentoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestDocumentoUpdateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - ")));
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
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return Ok(ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    if (request.idTipoAsunto == EnumTipoAsuntoConst.CUMPLIMENTACION)
                    {
                        Cumplimentacion entityCumplimentacion = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                        if (entityCumplimentacion is null || !entityCumplimentacion.activo)
                        {
                            return Ok(
                                ResultOperation.FailureErrorResponse(
                                    "La cumplimentación no existe o fue eliminada."
                                )
                            );
                        }
                    }
                    else
                    {
                        Autorizacion entityAutorizacionExists = await _genericService.GetAutorizacion(request.idAsunto);
                        if (entityAutorizacionExists is null || !entityAutorizacionExists.activo)
                        {
                            return Ok(
                                ResultOperation.FailureErrorResponse(
                                    "La autorización no existe o fue eliminada."
                                )
                            );
                        }
                    }

                    var entityDocumentoList = await _genericService.GetArchivosAutorizacionesByIds(new int[] { request.id });
                    if (entityDocumentoList is null || !entityDocumentoList.Any(c => c.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "No existe el documento o fue eliminado."
                            )
                        );
                    }

                    var entityDocumento = entityDocumentoList.FirstOrDefault(c => c.activo);
                    if (!_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine("AUTORIZACIONES", "COMERCIO EXTERIOR", request.idAsunto.ToString()!),
                            out var dataFile, out string message, requiredExtentions, entityDocumento!.file_path))
                    {
                        return Ok(
                                ResultOperation.FailureErrorResponse<int>(
                                    message
                                )
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
                        return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                    }

                    ResultOperation<int> result = await _genericService.UpdateArchivo(entityDocumento!, dataFile!);
                    return Ok(result);
                }                

                return Ok(ResultOperation.FailureErrorResponse<string>("Tipo de asunto no válido."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete("archivo")]
        public async Task<IActionResult> DeleteDocumento(RequestDocumentoDelete request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
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
                    case EnumTipoAsuntoConst.CUMPLIMENTACION:
                        key = $"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.idAsunto}";
                        break;
                    default:
                        return Ok(ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var entityListExists = await _genericService.GetArchivosAutorizacionesByIds(request.Ids.ToArray());
                    if (entityListExists is null || !entityListExists.Any())
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "Los documentos no existen."
                            )
                        );
                    }

                    if (!request.Ids.All(value => entityListExists.Select(x => x.id).Contains(value)))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "Algunos documentos no existen."
                            )
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
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }

                    var result = await _genericService.DeleteArchivo(request.Ids.ToArray(), sessionInformation.UserInformation.Rfc);
                    return Ok(result);
                }                
                return Ok(ResultOperation.FailureWarningResponse("El tipo de modalidad no es válida."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }
        #endregion

        #region Cumplimentación
        [ProducesResponseType(typeof(ResultOperation<List<ResponseCumplimentacionNumeroAsunto>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("buscar-cumplimentacion/numero-asunto/{noAsunto}")]
        public async Task<IActionResult> GetCumplimentacionNumeroAsunto(string noAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                var result = await _service.GetCumplimentacionBuscarNoAsuntoAsync(noAsunto, sessionInformation.UserInformation);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [ProducesResponseType(typeof(ResultOperation), 404)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("cumplimentacion")]
        public async Task<IActionResult> PostCumplimentacion([FromBody] RequestCumplimentacionOficialPartesCreate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(ResultOperation.FailureErrorResponse("No se pudo obtener la información del usuario."));
                }

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityExist = await _genericService.GetAutorizacionById(request.idAsunto.GetValueOrDefault());
                    if (entityExist is null || !entityExist.activo)
                    {
                        return Ok(ResultOperation.FailureErrorResponse<int>("La autorización no existe o fue eliminada."));
                    }

                    var entityExists = await _genericService.GetCumplimentacionById(null!, request.idAsunto);
                    Cumplimentacion entity = null!;
                    try
                    {
                        if (entityExists is not null)
                        {
                            throw new Exception($"Ya existe una cumplimentación para el número de asunto {entityExist.no_asunto}");
                        }

                        entity = CumplimentacionOficialPartesEvents.Create(
                            ref entityExist!,
                            request.noAsuntoExterno!,
                            sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ResultOperation.FailureWarningResponse(ex.Message));
                    }

                    var result = await _service.AddCumplimentacionAsync(entityExist!, entity, sessionInformation.UserInformation);
                    return Ok(result);
                }
                else if (!string.IsNullOrEmpty(request.noAsuntoExterno))
                {
                    var entityExists = await _service.GetAutorizacionByNoAsunto(request.noAsuntoExterno);
                    Cumplimentacion? entity = null!;
                    try
                    {
                        
                        if (entityExists is not null)
                        {
                            if (entityExists.externo_cumplimentacion)
                            {
                                throw new Exception($"Ya existe una cumplimentación para el número de asunto externo {request.noAsuntoExterno}");
                            }
                            throw new Exception($"Ya existe una autorización con el número de asunto {request.noAsuntoExterno}");
                        }

                        entity = CumplimentacionOficialPartesEvents.Create(
                            ref entityExists,
                            request.noAsuntoExterno!,
                            sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ResultOperation.FailureWarningResponse(ex.Message));
                    }
                    var result = await _service.AddCumplimentacionAsync(entityExists!, entity, sessionInformation.UserInformation);
                    return Ok(result);
                }
                return Ok(ResultOperation.FailureWarningResponse("Tipo de asunto no válido."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultOperation.FailureErrorResponse(ex.Message));
            }
        }

        
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("cumplimentacion")]
        public async Task<IActionResult> PatchCumplimentacion([FromBody] RequestCumplimentacionOficialPartesUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION.ToStringValue()}{request.id}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var entityExists = await _genericService.GetCumplimentacionById(request.id, null!);
                if (entityExists is null || !entityExists.activo || entityExists.id_tipo_modalidad != EnumTipoModalidad.FÍSICO.GetHashCode())
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La cumplimentación no existe o fue eliminada."
                        )
                    );
                }

                var autorizacionExists = await _genericService.GetAutorizacionById(entityExists.id_autorizacion);
                if (autorizacionExists is null || !autorizacionExists.activo )
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La autorización no existe o fue eliminada."
                        )
                    );
                }

                try
                {
                    CumplimentacionOficialPartesEvents.UpdateCumplimentacion(
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
                        string.IsNullOrEmpty(request.fechaVencimiento) ? null : DateTime.Parse(request.fechaVencimiento),
                        request.idOrganoJurisdiccional,
                        request.organoJurisdiccional,
                        request.idUnidadAdministrativaCumplimiento,
                        request.unidadAdministrativaCumplimiento,
                        request.oficioResolucionImpugnada,
                        string.IsNullOrWhiteSpace(request.fechaOficioResolucionImpugnada) ? null : DateTime.Parse(request.fechaOficioResolucionImpugnada),
                        request.idUnidadAdministrativa,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _service.UpdateCumplimentacion(autorizacionExists, entityExists, sessionInformation.UserInformation);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<ResponseCumplimentacionOficialPartesById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("cumplimentacion/{id}")]
        public async Task<IActionResult> GetCumplimentacionById(int id)
        {
            try
            {
                var result = await _service.GetCumplimentacionByIdDisconnectedAsync(id);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        error
                    )
                );
            }
        }
        #endregion
    }
}