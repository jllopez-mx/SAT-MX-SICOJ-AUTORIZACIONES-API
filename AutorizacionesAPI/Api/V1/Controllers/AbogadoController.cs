using AutorizacionesAPI.Model.DAO.ServicesDAO;
using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.RequestFilters;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.LoggerConfiguration;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.ViewModels;


namespace AutorizacionesAPI.Api.V1.Controllers
{
    [Route("sicoj/autorizaciones/api/v1/abogado/autorizaciones")]
    [ApiController]
    public class AbogadoController : ControllerBase
    {
        #region Variables / Contructor
        private readonly ILogger<AbogadoController> _logger;
        private readonly IAbogadoService _service;
        private readonly IGenericImplementation _genericImplementation;
        

        public AbogadoController(ILogger<AbogadoController> logger, IAbogadoService service, IGenericImplementation genericImplementation)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _genericImplementation = genericImplementation ?? throw new ArgumentNullException(nameof(genericImplementation));
            _genericImplementation.SetVariables(EnumRolesSicoj.ABOGADO);
        }
        #endregion

        #region Bandeja / Histórico
        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesAbogadoBandeja>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetBandejaPendientesAsync([FromQuery] PagerQueryFilters request)
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

                RequestAbogadoPendientesFilters filters = new();
                Filters.MapFilters(request, filters);
                if (!Filters.MapSort<EnumOrderColumnAutorizacionesAbogadoByFiltros>(request, EnumOrderColumnAutorizacionesAbogadoByFiltros.ByFechaVencimientoAsc.ToString(), false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesAbogadoBandeja>>("La columna de ordenamiento no es válida."));
                }
                var result = await _service.GetBandejaPendientesAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    Filters.GetStringValue(filters!.ByNoAsunto.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaPresentacionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaPresentacionHasta.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoHasta.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByRfc.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByPromovente.FirstOrDefault()),
                    filters.ByIdTema.Adapt<List<int>>(),
                    filters.ByIdTipoAsunto.Adapt<List<int>>(),
                    filters.ByIdEstadoTarea.Adapt<List<int>>(),
                    filters.ByIdTipoEntrada.Adapt<List<int>>(),
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

        [ProducesResponseType(typeof(ResultOperation<ResponseAutorizacionesAbogadoHistorico>), 200)]
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

                RequestAbogadoHistoricoFilters filters = new();
                Filters.MapFilters(request, filters);
                if (!Filters.MapSort<EnumOrderColumnAutorizacionesAbogadoByFiltros>(request, null!, false, out string OrderByColumn, out bool OrderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesAbogadoHistorico>>("La columna de ordenamiento no es válida."));
                }
                var result = await _service.GetHistoricoAsync(
                    request.fetch,
                    request.page,
                    OrderByColumn,
                    OrderDesc,
                    Filters.GetStringValue(filters!.ByNumeroAsunto.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionHasta.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByRfc.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByPromovente.FirstOrDefault()),
                    filters.ByTipoAsunto.Adapt<List<int>>(),
                    filters.ByTarea.Adapt<List<int>>(),
                    filters.ByTipoModalidad.Adapt<List<int>>(),
                    filters.ByEstadoProcesal.Adapt<List<int>>(),
                    filters.ByAlerta.Adapt<List<int>>(),
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
        public async Task<IActionResult> PatchAutorizacionesComercioExterior(RequestComercioExteriorAbogadoUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchComercioExteriorAbogado(HttpContext, request));
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
        public async Task<IActionResult> PatchAutorizacionesImpuestosInternos(RequestImpuestosInternosAbogadoUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchImpuestosInternosAbogado(HttpContext, request));
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

        #region Archivo
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
                return Ok(await _genericImplementation.PatchSolicitudOpinionInformacion(HttpContext, request)) ;
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
        public async Task<IActionResult> PatchCumplimentacion([FromBody] RequestCumplimentacionAbogadoUpdate request)
        {
            try
            {
                return Ok(await _genericImplementation.PatchCumplimentacion(HttpContext, request));
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