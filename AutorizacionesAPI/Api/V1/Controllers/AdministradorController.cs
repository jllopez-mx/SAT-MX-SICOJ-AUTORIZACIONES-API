using AutorizacionesAPI.Model.DAO.ServicesDAO;
using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.ContractsValidations;
using AutorizacionesAPI.Model.DTO.RequestFilters;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.Entities.Events.AdminAbogado;
using AutorizacionesAPI.Model.Entities.Events.Administrador;
using AutorizacionesAPI.Model.Entities.Events.Genericos;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Files;
using Sicoj.Utils.LoggerConfiguration;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Api.V1.Controllers
{
    [Route("sicoj/autorizaciones/api/v1/administrador/autorizaciones")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        #region Variables / Contructor
        private readonly ILogger<AdministradorController> _logger;
        private readonly RequestUpdateAutorizacionesComercioExteriorAdministradorFisicoValidator _updateComercioExteriorFisicoValidator;
        private readonly RequestUpdateAutorizacionesComercioExteriorAdministradorLineaValidator _updateComercioExteriorLineaValidator;
        private readonly RequestUpdateAutorizacionesImpuestosInternosAdministradorFisicoValidator _updateImpuestosInternosFisicoValidator;
        private readonly RequestUpdateAutorizacionesImpuestosInternosAdministradorLineaValidator _updateImpuestosInternosLineaValidator;
        private readonly IAdministradorService _service;
        private readonly IRedisClient _redisClient;
        private readonly IFileSystemService _fileSystemService;
        private readonly IAdminAbogadoService _adminAbogadoService;
        private readonly IGenericService _genericService;
        private readonly IGenericImplementation _genericImplementation;
        private EnumRolesSicoj _roleSicoj = EnumRolesSicoj.ABOGADO;
        private readonly string _path1 = EnumPaths.AUTORIZACIONES.ToString();

        public AdministradorController(ILogger<AdministradorController> logger, RequestUpdateAutorizacionesComercioExteriorAdministradorFisicoValidator updateComercioExteriorFisicoValidator, RequestUpdateAutorizacionesComercioExteriorAdministradorLineaValidator updateComercioExteriorLineaValidator, RequestUpdateAutorizacionesImpuestosInternosAdministradorFisicoValidator updateImpuestosInternosFisicoValidator, RequestUpdateAutorizacionesImpuestosInternosAdministradorLineaValidator updateImpuestosInternosLineaValidator, IAdministradorService service, IRedisClient redisClient, IFileSystemService fileSystemService, IAdminAbogadoService adminAbogadoService, IGenericService genericService, IGenericImplementation genericImplementation)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateComercioExteriorFisicoValidator = updateComercioExteriorFisicoValidator ?? throw new ArgumentNullException(nameof(updateComercioExteriorFisicoValidator));
            _updateComercioExteriorLineaValidator = updateComercioExteriorLineaValidator ?? throw new ArgumentNullException(nameof(updateComercioExteriorLineaValidator));
            _updateImpuestosInternosFisicoValidator = updateImpuestosInternosFisicoValidator ?? throw new ArgumentNullException(nameof(updateImpuestosInternosFisicoValidator));
            _updateImpuestosInternosLineaValidator = updateImpuestosInternosLineaValidator ?? throw new ArgumentNullException(nameof(updateImpuestosInternosLineaValidator));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _adminAbogadoService = adminAbogadoService ?? throw new ArgumentNullException(nameof(adminAbogadoService));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
            _genericImplementation = genericImplementation ?? throw new ArgumentNullException(nameof(genericImplementation));
            _genericImplementation.SetVariables(EnumRolesSicoj.ADMINISTRADOR);
        }
        #endregion

        #region Bandeja / Histórico
        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesAdministradorBandeja>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetBandejaPendientesAsync([FromQuery] PagerQuery request)
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

                if (!Filters.MapSort<EnumOrderColumnAutorizacionesAdminByFiltros>(request, EnumOrderColumnAutorizacionesAdminByFiltros.ByFechaVencimientoAsc.ToString(), false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesAdministradorBandeja>>("La columna de ordenamiento no es válida."));
                }
                var result = await _service.GetBandejaPendientesAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    sessionInformation.UserInformation);
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

        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesAdministradorHistorico>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("historico")]
        public async Task<IActionResult> GetHistoricoAsync([FromQuery] PagerQueryFilters request)
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

                RequestAdministradorHistoricoFilters filters = new();
                Filters.MapFilters(request, filters);
                if (!Filters.MapSort<EnumOrderColumnAutorizacionesAbogadoByFiltros>(request, null!, false, out string OrderByColumn, out bool OrderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesAdministradorHistorico>>("La columna de ordenamiento no es válida."));
                }
                var result = await _service.GetHistoricoAsync(
                    request.fetch,
                    request.page,
                    OrderByColumn,
                    OrderDesc,
                    Filters.GetStringValue(filters!.ByNoAsunto.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionHasta.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByRfc.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByPromovente.FirstOrDefault()),
                    filters.ByIdTipoAsunto.Adapt<List<int>>(),
                    filters.ByIdEstadoTarea.Adapt<List<int>>(),
                    filters.ByIdTipoEntrada.Adapt<List<int>>(),
                    filters.ByAlerta.Adapt<List<int>>(),
                    filters.ByIdEstadoProcesal.Adapt<List<int>>(),
                    sessionInformation.UserInformation
                    );
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

        #region Tomar / Soltar
        [ProducesResponseType(typeof(ResultOperation<string>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("tomar/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> PatchTomar(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.PatchTomar(HttpContext, id, idTipoAsunto));
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
                return Ok(await _genericImplementation.PatchSoltar(HttpContext, id, idTipoAsunto));
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

        #region Edición Asunto
        [ProducesResponseType(typeof(ResultOperation<ResponseComercioExteriorAbogadoById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("comercio-exterior/{id}")]
        public async Task<IActionResult> GetComercioExteriorById(int id)
        {
            try
            {
                var result = await _service.GetComercioExteriorByIdDisconnected(id);
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
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("comercio-exterior")]
        public async Task<IActionResult> PatchAutorizacionesComercioExterior(RequestComercioExteriorAdministradorUpdate request)
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

                    try
                    {
                        if (request.Historico)
                        {
                            AutorizacionAdminAbogadoEvents.Update(ref entityExists,
                                ref avisoSinRespuesta,
                                ref avisoConRespuesta,
                                null!,
                                null!,
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
                        else
                        {
                            ComercioExteriorAdministradorEvents.UpdateModalidadFisico(ref entityExists,
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
                                request.IdSubadministracion,
                                sessionInformation.UserInformation.Rfc
                                );
                        }
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation);
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

                    try
                    {
                        if (request.Historico)
                        {
                            AutorizacionAdminAbogadoEvents.Update(ref entityExists,
                                ref avisoSinRespuesta,
                                ref avisoConRespuesta,
                                null!,
                                null!,
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
                        else
                        {
                            ComercioExteriorAdministradorEvents.UpdateModalidadLinea(
                                ref entityExists,
                                request.IdTipoAutorizacion,
                                request.PromoventeNoContribuyente,
                                request.RfcContribuyente,
                                request.Contribuyente,
                                request.IdAutoridadDirigida,
                                request.OtroAutoridadDirigida,
                                request.DomicilioNotificaciones,
                                DateTime.Parse(request.FechaPresentacion),
                                request.IdFundamentoSolicitud!,
                                request.OtroFundamentoSolicitud,
                                request.IdTema!,
                                request.OtroTema,
                                DateTime.Parse(request.FechaRecepcion),
                                request.IdSubadministracion,
                                sessionInformation.UserInformation.Rfc
                                );
                        }
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation);
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

        [ProducesResponseType(typeof(ResultOperation<ResponseComercioExteriorAbogadoById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("impuestos-internos/{id}")]
        public async Task<IActionResult> GetImpuestosInternosById(int id)
        {
            try
            {
                var result = await _service.GetImpuestosInternosByIdDisconnected(id);
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
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("impuestos-internos")]
        public async Task<IActionResult> PatchAutorizacionesImpuestosInternos(RequestImpuestosInternosAdministradorUpdate request)
        {
            try
            {
                _logger.LogInformationSicoj("Request: ", request);
                if (request.Historico)
                {
                    return Ok(await _genericImplementation.PatchImpuestosInternosAbogado(HttpContext, request.Adapt<RequestImpuestosInternosAbogadoUpdate>()));
                }
                else
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
                                    return Ok(ResultOperation.FailureErrorResponse<string>(
                                        "El número de asunto a relacionar fue eliminado."
                                    ));
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
                            _logger.LogInformationSicoj("Autorizacion Pre: ", entityExists);
                            ImpuestosInternosAdministradorEvents.UpdateModalidadFisico(ref entityExists,
                                ref avisoSinRespuesta,
                                ref avisoConRespuesta,
                               entityRelacionado,
                                noAsuntoExterno,
                                request.Rfc,
                                request.Promovente,
                                request.PromoventeNoContribuyente,
                                request.RfcContribuyente,
                                request.Contribuyente,
                                request.DespachosAutorizados,
                                request.DomicilioPromovente,
                                request.DomicilioNotificaciones,
                                DateTime.Parse(request.FechaPresentacion),
                                DateTime.Parse(request.FechaRecepcion),
                                request.IdTema,
                                request.OtroTema,
                                request.NoIndicaMonto,
                                request.Monto,
                                request.IdSubadministracion,
                                sessionInformation.UserInformation.Rfc
                                );
                            _logger.LogInformationSicoj("Autorizacion Post: ", entityExists);
                        }
                        catch (Exception _ex)
                        {
                            return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                        }
                        var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation);
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
                                    return Ok(ResultOperation.FailureErrorResponse<string>(
                                        "El número de asunto a relacionar fue eliminado."
                                    ));
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
                            _logger.LogInformationSicoj("Autorizacion Pre: ", entityExists);
                            ImpuestosInternosAdministradorEvents.UpdateModalidadLinea(ref entityExists,
                                ref avisoSinRespuesta,
                                ref avisoConRespuesta,
                                entityRelacionado,
                                entityRelacionado is null ? noAsuntoExterno : entityRelacionado.no_asunto,
                                request.PromoventeNoContribuyente,
                                request.RfcContribuyente,
                                request.Contribuyente,
                                request.DespachosAutorizados,
                                request.DomicilioNotificaciones,
                                DateTime.Parse(request.FechaPresentacion),
                                DateTime.Parse(request.FechaRecepcion),
                                request.IdTema,
                                request.OtroTema,
                                request.NoIndicaMonto,
                                request.Monto,
                                request.IdSubadministracion,
                                sessionInformation.UserInformation.Rfc
                                );
                            _logger.LogInformationSicoj("Autorizacion Post: ", entityExists);
                        }
                        catch (Exception _ex)
                        {
                            return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                        }
                        var result = await _genericService.UpdateAutorizacionAsync(entityExists, avisoSinRespuesta, avisoConRespuesta, sessionInformation.UserInformation);
                        return Ok(result);
                    }
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

        #region Asignar
        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("asignar")]
        public async Task<IActionResult> PatchAsignar(RequestAsignar request)
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
                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                        key = $"{EnumModulosRedis.AUTORIZACIONES.ToStringValue()}{request.idAsunto}";
                        break;
                    case EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS:
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
                    _logger.LogInformationSicoj("El asunto no ha sido tomado: ", key);
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
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

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    Abogado entityAbogado = null!;
                    try
                    {
                        ComercioExteriorAdministradorEvents.UpdateAsignar(ref entityExists,
                            sessionInformation.TokenInfomation.workforceID,
                            sessionInformation.UserInformation.Rfc
                        );

                        entityAbogado = ComercioExteriorAsignarAbogadoEvents.Create(request.idAbogado, sessionInformation.UserInformation.Rfc);
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.AsignarAsync(entityExists, null!, entityAbogado);
                    return Ok(result);
                }
                else if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var entityExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La cumplimentación no existe o fue eliminada."
                            )
                        );
                    }

                    Abogado entityAbogado = null!;
                    try
                    {
                        CumplimentaciónAdministradorEvents.UpdateAsignarCumplimentacion(ref entityExists,
                            sessionInformation.TokenInfomation.workforceID,
                            sessionInformation.UserInformation.Rfc
                        );

                        entityAbogado = ComercioExteriorAsignarAbogadoEvents.Create(request.idAbogado, sessionInformation.UserInformation.Rfc);
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.AsignarAsync(null!, entityExists, entityAbogado);
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

        #region Reasignar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("reasignar")]
        public async Task<IActionResult> PatchReasignar(RequestReasignar request)
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
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    var result = await _genericService.ReasignarAsync(request.idAsuntoLista.ToArray(), sessionInformation.UserInformation, request.idAbogado);
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
        #endregion

        #region Remisión
        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("remitir")]
        public async Task<IActionResult> PatchRemitir([FromForm] RequestRemitir request)
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

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    DataFile dataFile = null!;
                    if (request.documento is not null)
                    {
                        if (!_fileSystemService.FileTryOut(
                                request.documento,
                                Path.Combine("AUTORIZACIONES", "COMERCIO EXTERIOR", request.idAsunto.ToString()),
                                out dataFile, out string message, requiredExtentions))
                        {
                            return Ok(
                                    ResultOperation.FailureErrorResponse<int>(
                                        message
                                    )
                                );
                        }
                    }

                    Documento entityDocumento = null!;
                    Remision entityRemision = null!;
                    try
                    {
                        entityRemision = ComercioExteriorRemisionAdministradorEvents.Create(entityExists.id, request.idTipoAutoridad, sessionInformation.UserInformation.IdAdministracionCentral, request.idUnidadAdministrativaRecibe, request.idUnidadAdministrativaExterna, DateTime.Parse(request.fechaOficio), request.numeroOficio, sessionInformation.UserInformation.Rfc);

                        ComercioExteriorAdministradorEvents.UpdateRemitir(ref entityExists,
                            entityRemision,
                            sessionInformation.UserInformation.Rfc
                        );

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
                                dataFile.File.ContentType,
                                _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                                sessionInformation.UserInformation.Rfc,
                                sessionInformation.UserInformation.Rfc,
                                true,
                                EnumRolesSicoj.ADMINISTRADOR.GetHashCode(),
                                null!
                            );
                        }
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.RemitirAsync(entityExists, entityRemision, entityDocumento, dataFile!);
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


        [ProducesResponseType(typeof(ResultOperation<ResponseRemision>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("remision/historico")]
        public async Task<IActionResult> GetRemisionAsync([FromQuery] PagerQuery request, int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                if (!Filters.MapSort<EnumOrderColumnRemisionByFiltros>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseRemision>>("La columna de ordenamiento no es válida."));
                }
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    var result = await _service.GetRemisionAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    idAutorizacion);
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
        #endregion

        #region Archivos
        [ProducesResponseType(typeof(ResultOperation<ResponseDocumentoHistorico>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("archivo/historico")]
        public async Task<IActionResult> GetDocumentosHistoricoAsync([FromQuery] RequestPagerQueryDocumentosHistoricoFilters request)
        {
            try
            {
                return Ok(await _genericImplementation.GetDocumentosHistoricoAsync(request));
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
                return Ok(await _genericImplementation.GetDocumentosHistoricoSeccionAsync(idAsunto, idTipoAsunto, idSeccion, idRenglonSeccion));
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
                return Ok(await _genericImplementation.GetDocumentosHistoricoSeccionPaginadoAsync(request));
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
                var (success, data, content, documento, message) = await _genericImplementation.GetDescargarArchivo(id, idTipoAsunto);
                if (!success)
                {
                    return BadRequest(message);
                }

                Response.Headers.Add("Content-Disposition", content.ToString());
                return File(data, documento.content_type);
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
                return Ok(await _genericImplementation.PostDocumento(HttpContext, request));
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
                return Ok(await _genericImplementation.PatchDocumento(HttpContext, request));
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
                return Ok(await _genericImplementation.DeleteDocumento(HttpContext, request));
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

        #region Solicitud de Opinión e información
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("solicitud-opinion-informacion")]
        public async Task<IActionResult> PostSolicitudInformacion([FromForm] RequestCreateSolicitudOpinionInformacion request)
        {
            try
            {
                return Ok(await _genericImplementation.PostSolicitudInformacion(HttpContext, request));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseSolicitudOpinionInformacionAbogadoById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("solicitud-opinion-informacion/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetSolicitudOpinionInformacionById(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetSolicitudOpinionInformacionById(id, idTipoAsunto));
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

        [ProducesResponseType(typeof(ResultOperation<List<ResponseSolicitudOpinionInformacionAbogadoById>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("solicitud-opinion-informacion/historico/{idAutorizacion}/{idTipoAsunto}")]
        public async Task<IActionResult> GetSolicitudOpinionInformacionByIDS(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetSolicitudOpinionInformacionByIDS(idAutorizacion, idTipoAsunto));
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
        [HttpPatch("solicitud-opinion-informacion")]
        public async Task<IActionResult> PatchSolicitudOpinionInformacion([FromForm] RequestSolicitudOpinionInformacionAbogadoUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchSolicitudOpinionInformacion(HttpContext, request));
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
        [HttpPatch("no-solicitud-opinion")]
        public async Task<IActionResult> PatchNoSolicitudOpinionInformacion([FromBody] RequestNoSolicitudOpinionInformacion request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchNoSolicitudOpinionInformacion(HttpContext, request));
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

        #region Personas Autorizadas
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("personas-autorizadas")]
        public async Task<IActionResult> PostPersonasAutorizadas(RequestPersonasAutorizadasCreate request)
        {
            try
            {
                return Ok(await _genericImplementation.PostPersonasAutorizadas(HttpContext, request));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseComercioExteriorPersonasAutorizadasAbogadoById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("personas-autorizadas/{id}")]
        public async Task<IActionResult> GetPersonasAutorizadasById(int id)
        {
            try
            {
                return Ok(await _genericImplementation.GetPersonasAutorizadasById(id));
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

        [ProducesResponseType(typeof(ResultOperation<List<ResponseComercioExteriorPersonasAutorizadasAbogadoById>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("personas-autorizadas/historico/{idAutorizacion}/{idTipoAsunto}")]
        public async Task<IActionResult> GetByID(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetByID(idTipoAsunto, idTipoAsunto));
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
        [HttpPatch("personas-autorizadas")]
        public async Task<IActionResult> PatchPersonasAutorizadas(RequestPersonasAutorizadasAbogadoUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchPersonasAutorizadas(HttpContext, request));
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
        [HttpDelete("personas-autorizadas/{idAsunto}/{id}")]
        public async Task<IActionResult> Delete(int idAsunto, int id)
        {
            try
            {
                return Ok(await _genericImplementation.Delete(HttpContext, idAsunto, id));
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

        #region Aviso Sin Respuesta
        [ProducesResponseType(typeof(ResultOperation<List<ResponseNumeroAsunto>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("impuestos-internos/numero-asunto/{noAsunto}/{idTipoAsunto}")]
        public async Task<IActionResult> GetNumeroAsunto(string noAsunto, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetNumeroAsunto(HttpContext, noAsunto, idTipoAsunto));
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
        [HttpPatch("aviso-sin-respuesta")]
        public async Task<IActionResult> PatchAvisoSinRespuesta([FromForm] RequestAvisoSinRespuesta request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchAvisoSinRespuesta(HttpContext, request));
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

        [ProducesResponseType(typeof(ResultOperation<List<ResponseAvisoSinRespuestaRelacionado>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("aviso-sin-respuesta/historico/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetAvisoSinRespuestaRelacionadoAsync(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetAvisoSinRespuestaRelacionadoAsync(id, idTipoAsunto));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseAvisoSinRespuestaRelacionado>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("aviso-sin-respuesta/{id}")]
        public async Task<IActionResult> GetAsuntoSinRespuestaById(int id)
        {
            try
            {
                return Ok(await _genericImplementation.GetAsuntoSinRespuestaById(id));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseAvisoSinRespuestaRelacionado>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("pendientes/aviso-sin-respuesta/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetAsuntoSinRespuestaByIdAutorizacion(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetAsuntoSinRespuestaByIdAutorizacion(id, idTipoAsunto));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseAvisoSinRespuestaRelacionado>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("aviso-sin-respuesta/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetAsuntoSinRespuestaByIdAsunto(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetAsuntoSinRespuestaByIdAsunto(id, idTipoAsunto));
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
        [HttpPatch("aviso-sin-respuesta/rechazar")]
        public async Task<IActionResult> PatchRechazarAvisoSinRespuesta(RequestRechazarAvisoSinRespuesta request)
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

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                {
                    var entityAvisoExists = await _genericService.GetAvisoSinRespuestaByIdAutorizacion(request.idAsunto);
                    if (entityAvisoExists is null || !entityAvisoExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "El aviso sin respuesta no existe o fue eliminado."
                            )
                        );
                    }

                    var entityExists = await _genericService.GetAutorizacionById(entityAvisoExists.id_autorizacion);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }
                    var descarteList = await _genericService.GetDescartarByIdAsunto(entityExists.id, null!);
                    Descartar entityDescarte = null!;
                    if (descarteList is not null && descarteList.Any(c => c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES) && c.activo))
                    {
                        entityDescarte = descarteList.FirstOrDefault(c => c.id_seccion.Equals(EnumSeccionesCons.DATOS_GENERALES) && c.activo)!;
                    }

                    List<int> secciones = new()
                    {
                        EnumSecciones.AVISO_SIN_RESPUESTA.GetHashCode(),
                    };

                    try
                    {
                        ImpuestosInternosAvisoSinRespuestaEvents.Rechazar(ref entityExists,
                            entityDescarte,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.RechazarAvisoSinRespuestaAsync(entityExists, secciones);
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

        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("aviso-sin-respuesta/aprobar")]
        public async Task<IActionResult> PatchAprobarAvisoSinRespuesta(RequestRechazarAvisoSinRespuesta request)
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

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                {
                    var entityAvisoExists = await _genericService.GetAvisoSinRespuestaByIdAutorizacion(request.idAsunto);
                    if (entityAvisoExists is null || !entityAvisoExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<string>(
                                "El aviso sin respuesta no existe o fue eliminado."
                            )
                        );
                    }

                    var entityExists = await _genericService.GetAutorizacionById(entityAvisoExists.id_autorizacion);
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
                        ImpuestosInternosAvisoSinRespuestaEvents.Aprobar(ref entityExists,
                            ref entityAvisoExists,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _service.AprobarAvisoSinRespuestaAsync(entityExists, entityAvisoExists);
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
        #endregion

        #region Requerimiento PRODECON
        [ProducesResponseType(typeof(ResultOperation<List<ResponseRequerimientoProdecon>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("requerimiento-prodecon/historico/{idAutorizacion}/{idTipoAsunto}")]
        public async Task<IActionResult> GetRequerimientoProdeconByAutorizacionAsync(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetRequerimientoProdeconByAutorizacionAsync(idAutorizacion, idTipoAsunto));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseRequerimientoProdecon>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("requerimiento-prodecon/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetRequerimientoProdeconById(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetRequerimientoProdeconById(id, idTipoAsunto));
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
        [HttpPost("requerimiento-prodecon")]
        public async Task<IActionResult> PostRequerimientoProdecon([FromForm] RequestRequerimientoProdeconCreate request)
        {
            try
            {
                return Ok(await _genericImplementation.PostRequerimientoProdecon(HttpContext, request));
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
        [HttpPatch("requerimiento-prodecon")]
        public async Task<IActionResult> PatchRequerimientoProdecon([FromBody] RequestRequerimientoProdeconUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchRequerimientoProdecon(HttpContext, request));
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

        #region Requerimiento
        [ProducesResponseType(typeof(ResultOperation<List<ResponseRequerimiento>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("requerimiento/historico/{idAutorizacion}/{idTipoAsunto}")]
        public async Task<IActionResult> GetRequerimientoByIdAutorizacion(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetRequerimientoByIdAutorizacion(idAutorizacion, idTipoAsunto));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseRequerimiento>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("requerimiento/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetRequerimientoById(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetRequerimientoById(id, idTipoAsunto));
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
        [ProducesResponseType(typeof(ResultOperation<bool>), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("no-requerimiento")]
        public async Task<IActionResult> PatchNoRequerimiento([FromBody] RequestNoRequerimiento request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchNoRequerimiento(HttpContext, request));
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
        [HttpPost("requerimiento")]
        public async Task<IActionResult> PostRequerimiento([FromForm] RequestRequerimientoCreate request)
        {
            try
            {
                return Ok(await _genericImplementation.PostRequerimiento(HttpContext, request));
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
        [HttpPatch("requerimiento")]
        public async Task<IActionResult> PatchRequerimiento([FromForm] RequestRequerimientoUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchRequerimiento(HttpContext, request));
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

        #region Aviso y Comunicados
        [ProducesResponseType(typeof(ResultOperation<List<ResponseRequerimiento>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("avisos-comunicados/historico/{idAutorizacion}/{idTipoAsunto}")]
        public async Task<IActionResult> GetAvisosComunicadosByIdAutorizacion(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetAvisosComunicadosByIdAutorizacion(idAutorizacion, idTipoAsunto));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseRequerimiento>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("avisos-comunicados/{id}/{idTipoAsunto}")]
        public async Task<IActionResult> GetAvisosComunicadosById(int id, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetAvisosComunicadosById(id, idTipoAsunto));
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
        [HttpPost("avisos-comunicados")]
        public async Task<IActionResult> PostAvisosComunicados([FromForm] RequestAvisosComunicadosCreate request)
        {
            try
            {
                return Ok(await _genericImplementation.PostAvisosComunicados(HttpContext, request));
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
        [HttpPatch("avisos-comunicados")]
        public async Task<IActionResult> PatchAvisosComunicados([FromBody] RequestAvisosComunicadosUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchAvisosComunicados(HttpContext, request));
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

        #region Resolución / Concluir / Aviso con Respuesta
        [ProducesResponseType(typeof(ResultOperation<ResponseResolucion>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("resolucion/{idAsunto}/{idTipoAsunto}/{idResolucion}")]
        public async Task<IActionResult> GetResolucionById(int idAsunto, int idTipoAsunto, int? idResolucion)
        {
            try
            {

                return Ok(await _genericImplementation.GetResolucionById(idAsunto, idTipoAsunto, idResolucion));
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
        [HttpPost("resolucion")]
        public async Task<IActionResult> PostResolucion([FromForm] RequestResolucionCreate request)
        {
            try
            {
                return Ok(await _genericImplementation.PostResolucion(HttpContext, request));
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
        [HttpPatch("resolucion")]
        public async Task<IActionResult> PatchResolucion([FromForm] RequestResolucionUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchResolucion(HttpContext, request));
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
        [HttpPatch("concluir")]
        public async Task<IActionResult> PatchConcluir([FromBody] RequestConcluir request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchConcluir(HttpContext, request));
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

        [ProducesResponseType(typeof(ResultOperation<List<ResponseAvisoConRespuesta>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("aviso-con-respuesta/historico/{idAutorizacion}/{idTipoAsunto}")]
        public async Task<IActionResult> GetAvisoConRespuestaRelacionadoAsync(int idAutorizacion, int idTipoAsunto)
        {
            try
            {
                return Ok(await _genericImplementation.GetAvisoConRespuestaRelacionadoAsync(idAutorizacion, idTipoAsunto));
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
        [ProducesResponseType(typeof(ResultOperation<ResponseCumplimentacionById>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("cumplimentacion/{id}")]
        public async Task<IActionResult> GetCumplimentacionById(int id)
        {
            try
            {
                return Ok(await _genericImplementation.GetCumplimentacionById(id));
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
        [HttpPatch("cumplimentacion")]
        public async Task<IActionResult> PatchCumplimentacion([FromBody] RequestCumplimentacionAdministradorUpdate request)
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
                try
                {
                    CumplimentaciónAdministradorEvents.UpdateCumplimentacion(
                        ref autorizacionExists,
                        ref entityExists,
                        request.idTipoAsuntoCumplimentar,
                        request.rfc,
                        request.promovente,
                        request.promoventeNoContribuyente,
                        request.rfcContribuyente,
                        request.contribuyenteAutorizacion,
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
                        request.idSubadministracion,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _adminAbogadoService.UpdateCumplimentacion(autorizacionExists, entityExists, sessionInformation.UserInformation);
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
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("cumplimentacion/improcedencia")]
        public async Task<IActionResult> PatchCumplimentacionImprocedencia([FromForm] RequestCumplimentacionImprocedencia request)
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
                string path2 = EnumPathSub.CUMPLIMENTACION.ToString();

                if (request.documento is null)
                {
                    return
                            Ok(ResultOperation.FailureErrorResponse<int>(
                                "El documento es obligatorio."

                        ));
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                if (!_fileSystemService.FileTryOut(
                    request.documento,
                    Path.Combine(_path1, path2, request.id.ToString()),
                    out var dataFileOficio, out string message, requiredExtentions))
                {
                    return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                message

                        ));
                }

                Documento entityDocumentoOficio = null!;

                try
                {
                    CumplimentaciónAdministradorEvents.UpdateImprocedencia(
                        ref entityExists,
                        sessionInformation.UserInformation.Rfc!
                    );

                    entityDocumentoOficio = ArchivoEvents.CreateFolio(
                        request.numeroFolio!,
                        null!,
                        EnumTipoDocumentoCons.OFICIO_ACUERDO_DE_CONCLUSIÓN_POR_IMPROCEDENCIA,
                        EnumSeccionesCons.DATOS_GENERALES,
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
                        request.id
                        );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _adminAbogadoService.UpdateCumplimentacionImprocedencia(entityExists, sessionInformation.UserInformation, entityDocumentoOficio, dataFileOficio);
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

        #region Medios de Defensa
        [ProducesResponseType(typeof(ResultOperation<ResponseMediosDefensa>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("medios-defensa/{idAsunto}/{idTipoAsunto}/{idResolucion}")]
        public async Task<IActionResult> GetMediosDefensa(int idAsunto, int idTipoAsunto, int? idResolucion)
        {
            try
            {
                var result = await _adminAbogadoService.GetMediosDefensa(idAsunto, idTipoAsunto, idResolucion);
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

        [ProducesResponseType(typeof(ResultOperation<ResponseMediosDefensa>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("medios-defensa")]
        public async Task<IActionResult> PostMediosDefensa([FromBody] RequestMediosDefensa request)
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

                var result = await _adminAbogadoService.GetMediosDefensaDisconnected(request.dato);
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


