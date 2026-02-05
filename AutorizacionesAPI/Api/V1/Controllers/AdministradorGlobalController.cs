using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Microsoft.AspNetCore.Mvc;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using AutorizacionesAPI.Model.Entities.Events.AdministradorGlobal;
using Mapster;
using AutorizacionesAPI.Model.DTO.RequestFilters;
using Sicoj.Utils.ViewModels;
using Sicoj.Utils.Files;
using AutorizacionesAPI.Model.Entities.Events.Genericos;
using AutorizacionesAPI.Model.Entities.Events.AdminAbogado;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.LoggerConfiguration;
using AutorizacionesAPI.Model.DAO.ServicesDAO;
using AutorizacionesAPI.Model.Entities.Events.Administrador;
using System.IO;

namespace AutorizacionesAPI.Api.V1.Controllers
{
    [Route("sicoj/autorizaciones/api/v1/administrador-global/autorizaciones")]
    [ApiController]
    public class AdministradorGlobalController : ControllerBase
    {
        #region Variables / Constructor
        private readonly ILogger<AdministradorGlobalController> _logger;
        private readonly IRedisClient _redisClient;
        private readonly IAdministradorGlobalService _service;
        private readonly IAdminAbogadoService _adminAbogadoService;
        private readonly IGenericService _genericService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IGenericImplementation _genericImplementation;
        private EnumRolesSicoj _roleSicoj = EnumRolesSicoj.ADMINISTRADOR_GLOBAL;
        private readonly string _path1 = EnumPaths.AUTORIZACIONES.ToString();

        public AdministradorGlobalController(ILogger<AdministradorGlobalController> logger, IRedisClient redisClient, IAdministradorGlobalService service, IAdminAbogadoService adminAbogadoService, IGenericService genericService, IFileSystemService fileSystemService, IGenericImplementation genericImplementation)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _adminAbogadoService = adminAbogadoService ?? throw new ArgumentNullException(nameof(adminAbogadoService));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _genericImplementation = genericImplementation ?? throw new ArgumentNullException(nameof(genericImplementation));
        }
        #endregion

        #region Histórico
        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesAdministradorGlobalHistorico>), 200)]
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

                RequestAdministradorGlobalHistoricoFilters filters = new();
                Filters.MapFilters(request, filters);
                if (!Filters.MapSort<EnumOrderColumnAutorizacionesAdminGlobalByFiltros>(request, null!, false, out string OrderByColumn, out bool OrderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesAdministradorGlobalHistorico>>("La columna de ordenamiento no es válida."));
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

        #region Get By Id
        [ProducesResponseType(typeof(ResultOperation<ResponseComercioExteriorAdministradorGlobalById>), 200)]
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

        [ProducesResponseType(typeof(ResultOperation<ResponseComercioExteriorAdministradorGlobalById>), 200)]
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
        #endregion

        #region Modificar / Descartar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("modificar")]
        public async Task<IActionResult> PatchModificarAsunto([FromBody] RequestModificarAsunto request)
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
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.idAsunto, null!);
                    if (modificacionList is not null && modificacionList.Any(c => c.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "Ya existe una modificacion en progreso."
                            )
                        );
                    }

                    Modificacion entity = null!;
                    try
                    {
                        entity = ComercioExteriorModificacionEvents.Create(
                            ref entityExists,
                            EnumSeccionesCons.DATOS_GENERALES,
                            null!,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                    }

                    ResultOperation result = await _genericService.AddModificacionAsync(entityExists, entity);
                    return Ok(result);
                }

                return Ok(ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido."));
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
        [HttpPatch("descartar")]
        public async Task<IActionResult> PatchDescartar([FromBody] RequestDescartarDatos request)
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

                string key = null!;
                switch (request.idTipoAsunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.DONATARIAS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                    case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
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
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                Autorizacion autorizacionExists = null!;
                Cumplimentacion cumplimentacionExists = null!;
                List<int> listaSecciones = null!;
                Descartar entity = null!;
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                {
                    autorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (autorizacionExists is null || !autorizacionExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    listaSecciones = new()
                    {
                        EnumSecciones.REMISION.GetHashCode(),
                        EnumSecciones.REQUERIMIENTO.GetHashCode(),
                        EnumSecciones.SOLICITUD_DE_OPINION.GetHashCode(),
                        EnumSecciones.EMISION_RESOLUCION.GetHashCode(),
                        EnumSecciones.AVISOS_Y_COMUNICADOS.GetHashCode(),
                    };
                }
                else if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    cumplimentacionExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (cumplimentacionExists is null || !cumplimentacionExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "La cumplimentación no existe o fue eliminada."
                            )
                        );
                    }
                    listaSecciones = new()
                    {
                        EnumSecciones.EMISION_RESOLUCION.GetHashCode(),
                    };
                }

                try
                {
                    entity = ComercioExteriorDescartarEvents.Create(
                              autorizacionExists,
                              cumplimentacionExists,
                              EnumSeccionesCons.DATOS_GENERALES,
                              sessionInformation.UserInformation.Rfc!
                    );

                    ComercioExteriorAdministradorGlobalEvents.Descartar(
                              ref autorizacionExists,
                              ref cumplimentacionExists,
                              sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.DescartarAsync(autorizacionExists, cumplimentacionExists, entity, listaSecciones);
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

        #region Reactivar
        [ProducesResponseType(typeof(ResultOperation<ResponseReactivar>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("reactivar")]
        public async Task<IActionResult> PatchReactivar([FromBody] RequestReactivar request)
        {
            try
            {
                _logger.LogInformationSicoj("Request:", request);
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseReactivar>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
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
                        return Ok(ResultOperation.FailureErrorResponse<ResponseReactivar>("Tipo de asunto no válido."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    _logger.LogInformationSicoj("Asunto no tomado", key);
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseReactivar>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    _logger.LogInformationSicoj("Asunto tomado por otro usuario", keyExists);
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseReactivar>(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                Autorizacion autorizacionExists = null!;
                Cumplimentacion cumplimentacionExists = null!;
                Abogado entityAbogado = null!;
                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                {
                    autorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (autorizacionExists is null || !autorizacionExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseReactivar>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    entityAbogado = await _service.GetAbogadoByIdAutorizacionAsync(request.idAsunto, null!);
                    if (entityAbogado is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseReactivar>(
                                "La autorización no tiene asignado un abogado."
                            )
                        );
                    }
                }
                else if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    cumplimentacionExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                    if (cumplimentacionExists is null || !cumplimentacionExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseReactivar>(
                                "La cumplimentación no existe o fue eliminada."
                            )
                        );
                    }

                    entityAbogado = await _service.GetAbogadoByIdAutorizacionAsync(null!, request.idAsunto);
                    if (entityAbogado is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseReactivar>(
                                "La cumplimentación no tiene asignado un abogado."
                            )
                        );
                    }
                }

                Reactivar entity = null!;
                try
                {
                    entity = ComercioExteriorReactivarEvents.Create(autorizacionExists, cumplimentacionExists, sessionInformation.UserInformation.Rfc);
                    ComercioExteriorAdministradorGlobalEvents.Reactivar(ref autorizacionExists, ref cumplimentacionExists, sessionInformation.UserInformation.Rfc);
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<ResponseReactivar>(_ex.Message));
                }
                var result = await _service.ReactivarAsync(autorizacionExists, cumplimentacionExists, entityAbogado, entity);
                _logger.LogInformationSicoj("Response: ", result);
                return Ok(result);
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse<ResponseReactivar>(
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

        #region Solicitud de Opinión e Información
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
        #endregion

        #region Personas Autorizadas
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

        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation<int>), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("requerimiento/modificar")]
        public async Task<IActionResult> PatchRequerimientoModificar([FromBody] RequestRequerimientoModificacion request)
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
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.idAsunto, null!);
                    if (modificacionList is not null && modificacionList.Any(c => c.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "Ya existe una modificacion en progreso."
                            )
                        );
                    }

                    Modificacion entity = null!;
                    try
                    {
                        entity = ComercioExteriorModificacionEvents.Create(
                            ref entityExists,
                            EnumSeccionesCons.REQUERIMIENTO,
                            null!,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                    }

                    ResultOperation result = await _genericService.AddModificacionAsync(entityExists, entity);
                    return Ok(result);
                }
                return Ok(ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse<int>(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<ResponseRequerimientoDescartar>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("requerimiento/descartar")]
        public async Task<IActionResult> PatchRequerimientoDescartar(
            [FromBody] RequestRequerimientoDescartar request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
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
                    default:
                        return Ok(ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>("Tipo de asunto no válido."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseRequerimientoDescartar>(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityAutorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityAutorizacionExists is null || !entityAutorizacionExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    if (entityAutorizacionExists.requerimiento is null)
                    {
                        return Ok(
                                ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
                                    "El asunto no contiene sección de requerimiento."
                                )
                            );
                    }

                    if (entityAutorizacionExists.requerimiento.GetValueOrDefault())
                    {
                        var requerimientoList = await _adminAbogadoService.GetRequerimientoByIdAutorizacion(request.idAsunto)!;
                        if (requerimientoList is null || !requerimientoList.Any(c => c.activo))
                        {
                            return Ok(
                                ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
                                    "El asunto no contiene registros de requerimiento."
                                )
                            );
                        }
                    }

                    Descartar entity = null!;
                    try
                    {
                        entity = ComercioExteriorDescartarEvents.Create(
                                  entityAutorizacionExists,
                                  null!,
                                  EnumSeccionesCons.REQUERIMIENTO,
                                  sessionInformation.UserInformation.Rfc!
                        );

                        ComercioExteriorRequerimientoEvents.DescartarSeccion(
                            ref entityAutorizacionExists,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse<ResponseRequerimientoDescartar>(_ex.Message));
                    }

                    List<int> listaSecciones = new()
                    {
                        EnumSecciones.REQUERIMIENTO.GetHashCode(),
                        EnumSecciones.EMISION_RESOLUCION.GetHashCode(),
                    };

                    var result = await _service.RequerimientoDescartarAsync(entityAutorizacionExists, entity, listaSecciones);
                    return Ok(result);
                }

                return Ok(ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>("Tipo de asunto no válido."));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseRequerimientoDescartar>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("requerimiento/descartar-ultimo")]
        public async Task<IActionResult> PatchRequerimientoDescartarUltimo(
            [FromBody] RequestRequerimientoDescartar request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
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
                    default:
                        return Ok(ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>("Tipo de asunto no válido."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseRequerimientoDescartar>(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                if (request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityAutorizacionExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityAutorizacionExists is null || !entityAutorizacionExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    var requerimientoList = await _adminAbogadoService.GetRequerimientoByIdAutorizacion(request.idAsunto)!;
                    if (requerimientoList is null || !requerimientoList.Any(c => c.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
                                "El asunto no contiene registros de requerimiento."
                            )
                        );
                    }

                    if (requerimientoList.Count(c => c.activo) <= 1)
                    {
                        return Ok(
                           ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>(
                               "Para realizar esta acción el asunto debe de contener más de un requerimiento."
                           )
                       );
                    }

                    var requerimiento = requerimientoList.LastOrDefault();
                    try
                    {
                        ComercioExteriorRequerimientoEvents.DescartarUltimo(
                            ref entityAutorizacionExists,
                            ref requerimiento!,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse<ResponseRequerimientoDescartar>(_ex.Message));
                    }

                    var result = await _service.RequerimientoDescartarUltimoAsync(entityAutorizacionExists, requerimiento, EnumSeccionesCons.REQUERIMIENTO);
                    return Ok(result);
                }

                return Ok(ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>("Tipo de asunto no válido."));
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

        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation<int>), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("resolucion/modificar")]
        public async Task<IActionResult> PatchResolucionModificar(
            [FromBody] RequestRequerimientoModificacion request)
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
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    var modificacionList = await _genericService.GetModificacionByIdAsunto(request.idAsunto, null!);
                    if (modificacionList is not null && modificacionList.Any(c => c.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "Ya existe una modificacion en progreso."
                            )
                        );
                    }

                    Modificacion entity = null!;
                    try
                    {
                        entity = ComercioExteriorModificacionEvents.Create(
                            ref entityExists,
                            EnumSeccionesCons.EMISION_RESOLUCION,
                            null!,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                    }

                    ResultOperation result = await _genericService.AddModificacionAsync(entityExists, entity);
                    return Ok(result);
                }
                return Ok(ResultOperation.FailureErrorResponse<int>("Tipo de asunto no válido."));
            }
            catch (Exception _e)
            {
                string error = _e.ManageException();
                _logger.LogErrorSicoj(_e, error, context: HttpContext);
                return BadRequest(
                    ResultOperation.FailureErrorResponse<int>(
                        error
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<ResponseResolucionDescartar>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("resolucion/descartar")]
        public async Task<IActionResult> PatchResolucionDescartar([FromBody] RequestResolucionDescartar request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>(
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
                    default:
                        return Ok(ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>("Tipo de asunto no válido."));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>("El registro no se puede usar ya que el usuario no ha tomado la autorización.")
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
                    request.idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(request.idAsunto);
                    if (entityExists is null || !entityExists.activo)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>(
                                "La autorización no existe o fue eliminada."
                            )
                        );
                    }

                    var entityResolucionExitsList = await _adminAbogadoService.GetResolucionByIdAsuntoAsync(request.idAsunto, null!);
                    if (entityResolucionExitsList is null || !entityResolucionExitsList.Any(c => c.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>(
                                "La resolución no existe o fue eliminada."
                            )
                        );
                    }

                    Resolucion entityResolucionExits = entityResolucionExitsList.FirstOrDefault(c => c.activo)!;
                    if (entityResolucionExits.id_autorizacion != entityExists.id)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>(
                                "La resolución no pertenece a la autorización."
                            )
                        );
                    }

                    Descartar entity = null!;
                    try
                    {
                        entity = ComercioExteriorDescartarEvents.Create(
                                  entityExists,
                                  null!,
                                  EnumSeccionesCons.EMISION_RESOLUCION,
                                  sessionInformation.UserInformation.Rfc!
                        );

                        ComercioExteriorResolucionEvents.Descartar(
                            ref entityExists,
                            ref entityResolucionExits,
                            sessionInformation.UserInformation.Rfc!
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse<ResponseResolucionDescartar>(_ex.Message));
                    }

                    List<int> listaSecciones = new()
                    {
                        EnumSecciones.EMISION_RESOLUCION.GetHashCode(),
                    };

                    var result = await _service.ResolucionDescartarAsync(entityExists, entity, listaSecciones);
                    return Ok(result);
                }

                return Ok(ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>("Tipo de asunto no válido."));
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
        public async Task<IActionResult> PatchReasignar([FromBody] RequestAdministradorGlobalReasignar request)
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
                    var result = await _genericService.ReasignarAsync(request.idAsuntoLista.ToArray(), sessionInformation.UserInformation, request.idAbogado!, request.rfcAdministrador!);
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

        #region Aviso Sin Respuesta
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
        #endregion

        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("cumplimentacion/descartar")]
        public async Task<IActionResult> PatchDescartarCumplimentacionPorImprocedencia([FromBody] RequestDescartarDatos request)
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

                string key = null!;
                switch (request.idTipoAsunto)
                {
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
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                Autorizacion autorizacionExists = null!;
                Cumplimentacion cumplimentacionExists = null!;
                List<int> listaSecciones = null!;
                Descartar entity = null!;

                cumplimentacionExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                if (cumplimentacionExists is null || !cumplimentacionExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "La cumplimentación no existe o fue eliminada."
                        )
                    );
                }

                try
                {
                    entity = ComercioExteriorDescartarEvents.Create(
                              autorizacionExists,
                              cumplimentacionExists,
                              EnumSeccionesCons.CUMPLIMENTACION_IMPROCEDENCIA,
                              sessionInformation.UserInformation.Rfc!
                    );

                    CumplimentacionAdminstradorGlobalEvents.DescartarPorImprocedencia(
                              ref cumplimentacionExists,
                              sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.DescartarCumplimentacionPorImprocedenciaAsync(cumplimentacionExists, entity);
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
        [HttpPatch("cumplimentacion/descartar/NoAsunto")]
        public async Task<IActionResult> PatchDescartarCumplimentacionNoAsunto([FromBody] RequestDescartarDatos request)
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

                string key = null!;
                switch (request.idTipoAsunto)
                {
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
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                Autorizacion autorizacionExists = null!;
                Cumplimentacion cumplimentacionExists = null!;
                List<int> listaSecciones = null!;
                Descartar entity = null!;

                cumplimentacionExists = await _genericService.GetCumplimentacionById(request.idAsunto, null!);
                if (cumplimentacionExists is null || !cumplimentacionExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "La cumplimentación no existe o fue eliminada."
                        )
                    );
                }

                try
                {
                    entity = ComercioExteriorDescartarEvents.Create(
                              autorizacionExists,
                              cumplimentacionExists,
                              EnumSeccionesCons.CUMPLIMENTACION_ASUNTO_A_CUMPLIMENTAR,
                              sessionInformation.UserInformation.Rfc!
                    );

                    CumplimentacionAdminstradorGlobalEvents.DescartarNoAsunto(
                              ref cumplimentacionExists,
                              sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.DescartarCumplimentacionNoAsuntoAsync(cumplimentacionExists, entity);
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
    }
}
