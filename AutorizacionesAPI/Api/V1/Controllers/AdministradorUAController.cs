using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.RequestFilters;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.LoggerConfiguration;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;

namespace AutorizacionesAPI.Api.V1.Controllers
{
    [Route("sicoj/autorizaciones/api/v1/administrador-ua/autorizaciones")]
    [ApiController]
    public class AdministradorUAController : ControllerBase
    {
        #region Variables / Constructor
        private readonly ILogger<AdministradorUAController> _logger;
        private readonly IAdministradorUnidadCentralService _service;
        private readonly IGenericService _genericService;
        private readonly IRedisClient _redisClient;

        public AdministradorUAController(ILogger<AdministradorUAController> logger, IRedisClient redisClient, IAdministradorUnidadCentralService service, IGenericService genericService) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
        }
        #endregion

        #region Histórico
        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesAdministradorUAHistorico>), 200)]
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

                RequestAdministradorUAHistoricoFilters filters = new();
                Filters.MapFilters(request, filters);
                if (!Filters.MapSort<EnumOrderColumnAutorizacionesAdminUAByFiltros>(request, null!, false, out string OrderByColumn, out bool OrderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesAdministradorUAHistorico>>("La columna de ordenamiento no es válida."));
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
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
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
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        default:
                            return Ok(ResultOperation.FailureErrorResponse<string>("Tipo de asunto no existe."));
                    }
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    Cumplimentacion entityExists = await _genericService.GetCumplimentacionById(id, null!);
                    if (entityExists is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<bool>(
                                "La cumplimentación no existe."
                            )
                        );
                    }
                    enumModulo = EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION;
                }
                else
                    return Ok(ResultOperation.FailureErrorResponse<bool>("No existe el tipo de entrada."));

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
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                {
                    Autorizacion entityExists = await _genericService.GetAutorizacionById(id);
                    if (entityExists is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<bool>(
                                "La autorización no existe."
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
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                            enumModulo = EnumModulosRedis.AUTORIZACIONES;
                            break;
                        default:
                            return Ok(ResultOperation.FailureErrorResponse<string>("Tipo de asunto no existe."));
                    }
                }
                else if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    Cumplimentacion entityExists = await _genericService.GetCumplimentacionById(id, null!);
                    if (entityExists is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<bool>(
                                "La cumplimentación no existe."
                            )
                        );
                    }
                    enumModulo = EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION;
                }
                else
                    return Ok(ResultOperation.FailureErrorResponse<bool>("No existe el tipo de entrada."));

                var response = await _redisClient.Drop(enumModulo, id, sessionInformation.UserInformation.Rfc!);
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
    }
}
