using AutorizacionesAPI.Model.DAO.Repository;
using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.CatalogsContracts;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.LoggerConfiguration;
using Sicoj.Utils.Models;
using Sicoj.Utils.Models.Responses;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DAO.ServicesDAO
{
    public class AdminAbogadoService : IAdminAbogadoService
    {
        private readonly ILogger<AdminAbogadoService> _logger;
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        private readonly IGenericService _genericService;
        private readonly CatalogosEnpoints _catologosEnpoints;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IRequerimientoProdeconRepository _requerimientoProdeconRepository;
        private readonly IAvisosComunicadosRepository _avisosComunicadosRepository;
        private readonly IResolucionRepository _resolucionRepository;
        private readonly IAutorizacionRepository _autorizacionRepository;
        private readonly IRequerimientoRepository _requerimientoRepository;
        private readonly IPersonasAutorizadasRepository _personasAutorizadasRepository;
        private readonly ISolicitudOpinionInformacionRepository _solicitudOpinionInformacionRepository;
        private readonly IAvisoSinRespuestaRepository _avisoSinRespuestaRepository;
        private readonly ICumplimentacionRepository _cumplimentacionRepository;
        private readonly IMediosDefensaRepository _mediosDefensaRepository;
        private readonly IModificacionRepository _modificacionRepository;
        private readonly IDescartarRepository _descartarRepository;

        public AdminAbogadoService(
            ILogger<AdminAbogadoService> logger,
            IApiService apiService,
            IRedisClient redisClient,
            IGenericService genericService,
            IOptions<CatalogosEnpoints> catologosEnpoints,
            IOptions<ProxyEnpoints> proxyEnpoints,
            IRequerimientoProdeconRepository autorizacionesComercioExteriorRequerimientoProdeconRepository,
            IAvisosComunicadosRepository comercioExteriorAvisosComunicadosRepository,
            IResolucionRepository impuestosInternosResolucionRepository,
            IAutorizacionRepository repositoryComercioExterior,
            IRequerimientoRepository impuestosInternosRequerimientoRepository,
            IPersonasAutorizadasRepository personasAutorizadasComercioExteriorRepository,
            ISolicitudOpinionInformacionRepository solicitudOpinionInformacionComercioExteriorRepository,
            IAvisoSinRespuestaRepository avisoSinRespuestaRepository,
            ICumplimentacionRepository repositoryCumplimentacion,
            IMediosDefensaRepository repositoryMediosDefensa,
            IModificacionRepository comercioExteriorModificacionRepository,
            IDescartarRepository comercioExteriorDescartarRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
            _catologosEnpoints = catologosEnpoints.Value ?? throw new ArgumentNullException(nameof(catologosEnpoints));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _requerimientoProdeconRepository = autorizacionesComercioExteriorRequerimientoProdeconRepository ?? throw new ArgumentNullException(nameof(autorizacionesComercioExteriorRequerimientoProdeconRepository));
            _avisosComunicadosRepository = comercioExteriorAvisosComunicadosRepository ?? throw new ArgumentNullException(nameof(comercioExteriorAvisosComunicadosRepository));
            _resolucionRepository = impuestosInternosResolucionRepository ?? throw new ArgumentNullException(nameof(impuestosInternosResolucionRepository));
            _autorizacionRepository = repositoryComercioExterior ?? throw new ArgumentNullException(nameof(repositoryComercioExterior));
            _requerimientoRepository = impuestosInternosRequerimientoRepository ?? throw new ArgumentNullException(nameof(impuestosInternosRequerimientoRepository));
            _personasAutorizadasRepository = personasAutorizadasComercioExteriorRepository ?? throw new ArgumentNullException(nameof(personasAutorizadasComercioExteriorRepository));
            _solicitudOpinionInformacionRepository = solicitudOpinionInformacionComercioExteriorRepository ?? throw new ArgumentNullException(nameof(solicitudOpinionInformacionComercioExteriorRepository));
            _avisoSinRespuestaRepository = avisoSinRespuestaRepository ?? throw new ArgumentNullException(nameof(avisoSinRespuestaRepository));
            _cumplimentacionRepository = repositoryCumplimentacion ?? throw new ArgumentNullException(nameof(repositoryCumplimentacion));
            _mediosDefensaRepository = repositoryMediosDefensa ?? throw new ArgumentNullException(nameof(repositoryMediosDefensa));
            _modificacionRepository = comercioExteriorModificacionRepository ?? throw new ArgumentNullException(nameof(comercioExteriorModificacionRepository));
            _descartarRepository = comercioExteriorDescartarRepository ?? throw new ArgumentNullException(nameof(comercioExteriorDescartarRepository));
        }

        #region Requerimiento
        public async Task<ResultOperation<bool>> UpdateImpuestosInternosNoRequerimiento(Autorizacion entity)
        {
            var result = await _autorizacionRepository.UpdateNoRequerimientoAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<bool>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(true);
        }

        public async Task<ResultOperation<int>> AddComercioExteriorRequerimiento(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile)
        {
            var result = await _requerimientoRepository.AddAsync(entityAutorizaciones, entity, entityDocumento, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> UpdateComercioExteriorRequerimiento(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile, int idSeccion)
        {
            //if (entity.fecha_oficio.Date > dateTime.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            //}

            //if (entity.fecha_ingreso_sat.Date > dateTime.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            //}

            //if (entity.fecha_ingreso_sat.Date < entity.fecha_oficio.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha oficio no puede ser menor a la fecha de ingreso en el SAT.");
            //}

            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.AUTORIZACIONES.GetHashCode()}&baseDate={entity.fecha_notificacion:yyyy-MM-dd}&addDays={11}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entity.fecha_vencimiento = fechaVencimiento;

            var result = await _requerimientoRepository.UpdateAsync(entityAutorizaciones, entity, entityDocumento, dataFile, idSeccion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<Requerimiento> GetRequerimientoById(int id) =>
           await _requerimientoRepository.GetById(id);

        public async Task<ResultOperation> GetRequerimientoByIdDisconnected(int id)
        {
            var result = await _requerimientoRepository.GetByIdDisconnected(id);
            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseRequerimiento>("No se encontraron resultados.");
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            if (result.idTipoAsunto > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAsuntoAutorizaciones, result.idTipoAsunto.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.tipoAsunto = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
            }
            return resultOperation;
        }

        public async Task<ResultOperation> GetComercioExteriorRequerimientoByIdDisconnected(int id)
        {
            var result = await _requerimientoRepository.GetByIdDisconnected(id);
            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseRequerimiento>("No se encontraron resultados.");
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            if (result.idTipoAsunto > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAsuntoAutorizaciones, result.idTipoAsunto.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.tipoAsunto = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
            }
            return resultOperation;
        }

        public async Task<ResultOperation> GetRequerimientoByIdAutorizacionDisconnectedAsync(int idAutorizacion)
        {
            var result = await _requerimientoRepository.GetByIdAutorizacionDisconnected(idAutorizacion);

            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseRequerimiento>());
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<List<Requerimiento>> GetRequerimientoByIdAutorizacion(int idAutorizacion) =>
            await _requerimientoRepository.GetByIdAutorizacion(idAutorizacion);
        #endregion

        #region Requerimiento PRODECON
        public async Task<ResultOperation<int>> AddComercioExteriorRequerimientoProdecon(RequerimientoProdecon entity, Documento entityDocumento, DataFile dataFile)
        {
            DateTime dateTime = DateTime.Now;
            if (entity.fecha_oficio.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_ingreso_sat.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_ingreso_sat.Date < entity.fecha_oficio.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("Fecha oficio no puede ser menor a la fecha de ingreso en el SAT.");
            }

            var result = await _requerimientoProdeconRepository.AddAsync(entity, entityDocumento, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> UpdateComercioExteriorRequerimientoProdecon(RequerimientoProdecon entity)
        {
            DateTime dateTime = DateTime.Now;
            if (entity.fecha_oficio.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_ingreso_sat.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_ingreso_sat.Date < entity.fecha_oficio.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("Fecha oficio no puede ser menor a la fecha de ingreso en el SAT.");
            }

            var result = await _requerimientoProdeconRepository.UpdateAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GetComercioExteriorRequerimientoProdeconAsync(int idAutorizacion)
        {
            var result = await _requerimientoProdeconRepository.GetRequerimientoProdecon(idAutorizacion);

            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseRequerimientoProdecon>());
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation> GetComercioExteriorRequerimientoProdeconByIdDisconnected(int id)
        {
            var result = await _requerimientoProdeconRepository.GetRequerimientoProdeconByIdDisconnected(id);
            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseRequerimientoProdecon>("No se encontraron resultados.");
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            return resultOperation;
        }
        public async Task<RequerimientoProdecon> GetComercioExteriorRequerimientoProdeconById(int id) =>
            await _requerimientoProdeconRepository.GetRequerimientoProdeconById(id);
        #endregion

        #region Avisos Y Comunicados
        public async Task<ResultOperation<int>> AddComercioExteriorAvisosComunicados(AvisosComunicados entity, Documento entityDocumento, DataFile dataFile)
        {
            var result = await _avisosComunicadosRepository.AddAsync(entity, entityDocumento, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> UpdateComercioExteriorAvisosComunicados(AvisosComunicados entity)
        {
            var result = await _avisosComunicadosRepository.UpdateAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GetComercioExteriorAvisosComunicadosAsync(int idAutorizacion)
        {
            var result = await _avisosComunicadosRepository.GetDisconnectedByIdAutorizacion(idAutorizacion);

            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseAvisosComunicados>());
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation> GetComercioExteriorAvisosComunicadosByIdDisconnected(int id)
        {
            var result = await _avisosComunicadosRepository.GetDisconnectedById(id);
            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseAvisosComunicados>("No se encontraron resultados.");
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            if (result.idTipoAviso > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAvisoAutorizaciones, result.idTipoAviso.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.tipoAviso = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo aviso.");
            }

            return resultOperation;
        }

        public async Task<AvisosComunicados> GetComercioExteriorAvisosComunicadosById(int id) =>
            await _avisosComunicadosRepository.GetById(id);
        #endregion

        #region Aviso Sin Respuesta
        public async Task<ResultOperation<int>> AddImpuestosInternosAvisoSinRespuesta(Autorizacion entityAutorizaciones, AvisoSinRespuesta entity, Documento entityDocumento, DataFile dataFile)
        {
            var result = await _avisoSinRespuestaRepository.AddAsync(entityAutorizaciones, entity, entityDocumento, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> UpdateAvisoSinRespuesta(Autorizacion entityAutorizaciones, AvisoSinRespuesta entityAviso, List<string> folios, Documento entityDocumento, DataFile dataFile)
        {
            var result = await _avisoSinRespuestaRepository.UpdateAsync(entityAutorizaciones, entityAviso, folios, entityDocumento, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GetNumeroAsuntoImpuestosInternosListAsync(string noAsunto, int idTipoAsunto)
        {
            var result = await _autorizacionRepository.GetListByNoAsuntoAsync(noAsunto, idTipoAsunto);

            if (result is null || !result.Any())
                return ResultOperation.SuccessResponseNoMessage(new List<ResponseNumeroAsunto>());

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<Autorizacion> GetAvisoNumeroAsunto(string? noAsunto) =>
            await _autorizacionRepository.GetAvisoNoAsuntoExternoAsync(noAsunto);

        #endregion

        #region Resolución        
        public async Task<ResultOperation<int>> AddImpuestosInternosResolucionAsync(Autorizacion entityAutorizaciones, Cumplimentacion cumplimentacion, Resolucion entity, Documento entityDocumentoOficio, DataFile dataFile)
        {
            //if (entity.fecha_oficio.Date > dateTime.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            //}

            //if (entity.fecha_ingreso_sat.Date > dateTime.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            //}

            //if (entity.fecha_ingreso_sat.Date < entity.fecha_oficio.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha oficio no puede ser menor a la fecha de ingreso en el SAT.");
            //}

            var result = await _resolucionRepository.AddAsync(entityAutorizaciones, cumplimentacion, entity, entityDocumentoOficio, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> UpdateImpuestosInternosResolucionAsync(Autorizacion entityAutorizaciones, Cumplimentacion entityCumplimentacion, Resolucion entity, Documento entityDocumento, DataFile dataFile, int idSeccion)
        {
            //if (entity.fecha_oficio.Date > dateTime.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            //}

            //if (entity.fecha_ingreso_sat.Date > dateTime.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha de Ingreso en el SAT no puede ser mayor a la fecha actual.");
            //}

            //if (entity.fecha_ingreso_sat.Date < entity.fecha_oficio.Date)
            //{
            //    return ResultOperation.FailureWarningResponse<int>("Fecha oficio no puede ser menor a la fecha de ingreso en el SAT.");
            //}

            _logger.LogInformationSicoj("Autorizacion Entity", entityAutorizaciones);
            _logger.LogInformationSicoj("Cumplimentacion Entity", entityCumplimentacion);
            _logger.LogInformationSicoj("Resolucion Entity", entity);
            _logger.LogInformationSicoj("Documento Entity", entityDocumento);

            var result = await _resolucionRepository.UpdateAsync(entityAutorizaciones, entityCumplimentacion, entity, entityDocumento, dataFile, idSeccion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<List<Resolucion>> GetResolucionByIdAsuntoAsync(int? idAutorizacion, int? idCumplimentacion) =>
            await _resolucionRepository.GetByIdAusunto(idAutorizacion, idCumplimentacion);

        public async Task<ResultOperation> GetResolucionByIdAsuntoDisconnectedAsync(int? idAutorizacion, int? idCumplimentacion, int? idResolucion)
        {
            var listaResoluciones = await GetResolucionByIdAsuntoAsync(idAutorizacion, idCumplimentacion);
            Resolucion primeraResolucion = null!;
            if (listaResoluciones is not null && listaResoluciones.Any(c => c.activo))
            {
                primeraResolucion = listaResoluciones.Where(c => c.activo).OrderBy(c => c.id).FirstOrDefault()!;
            }

            var result = await _resolucionRepository.GetByIdAsuntoDisconnected(idAutorizacion, idCumplimentacion);
            if (idResolucion.GetValueOrDefault(0) > 0)
            {
                result = result.Where(c => c.id.Equals(idResolucion.GetValueOrDefault())).ToList();
            }

            if (result is null)
            {
                var descartarList = await _genericService.GetDescartarByIdAsunto(idAutorizacion, idCumplimentacion);
                if (descartarList is not null && descartarList.Any(c => c.activo && c.id_seccion != EnumSeccionesCons.EMISION_RESOLUCION))
                {
                    var resultResolucion = await this.GetResolucionByIdAsuntoAsync(idAutorizacion, idCumplimentacion);
                    if (resultResolucion is not null && !resultResolucion.Any(c => c.activo))
                    {
                        return ResultOperation.SuccessResponseNoMessage(new List<ResponseResolucion>());
                    }
                }
                return ResultOperation.SuccessResponseNoMessage(new List<ResponseResolucion>());
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            foreach (var item in resultOperation.Result)
            {
                if (primeraResolucion is not null && primeraResolucion.id.Equals(item.id))
                    item.primera = true;

                if (item.idSentido > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.SentidoResolucionAutorizaciones, item.idSentido.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        item.sentido = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de sentido.");
                }
            }

            return resultOperation;
        }

        public async Task<ResultOperation<int>> UpdateConcluirAsync(Autorizacion entityAutorizaciones, Cumplimentacion cumplimentacion)
        {
            var result = await _autorizacionRepository.ConcluirAsync(entityAutorizaciones, cumplimentacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GetAvisoConRespuestaRelacionadoAsync(int idAutorizacionRelacionada)
        {
            var result = await _resolucionRepository.GetAvisoConRespuestaDisconnected(idAutorizacionRelacionada);

            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseAvisoConRespuesta>());
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        #endregion

        #region SolicitudOpinionInformación
        public async Task<ResultOperation> AddComercioExteriorSolicitudOpinionInformacion(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile, Autorizacion entityComercio)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                var result = await _autorizacionRepository.GetByIdAsync(entity.idAutorizacion);

                if (entity.fechaOficioSolicitud.Date > dateTime.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de solicitud no puede ser mayor a la fecha actual.");
                }

                if (entity.fechaOficioSolicitud.Date < Convert.ToDateTime(result.fecha_presentacion))
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de solicitud debe ser mayor o igual a la fecha de presentación en el SAT.");
                }

                var countResult = await _solicitudOpinionInformacionRepository.AddComercioExteriorSolicitudOpinionInformacionAsync(entity, entityDocumento, dataFile, entityComercio);
                if (!countResult.Success)
                    return ResultOperation<int?>.FailureErrorResponse($"{countResult.MsgError!}{countResult.DetailError}");

                return ResultOperation.SuccessResponse(countResult.Result, "La solicitud se guardó correctamente");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> UpdateComercioExteriorSolicitudOpinionInformacion(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile)
        {
            DateTime dateTime = DateTime.Now;
            var results = await _autorizacionRepository.GetByIdAsync(entity.idAutorizacion);
            if (entity.fechaOficioSolicitud.Date > dateTime.Date)
            {
                return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de solicitud no puede ser mayor a la fecha actual.");
            }

            if (entity.fechaOficioSolicitud.Date < Convert.ToDateTime(results.fecha_presentacion))
            {
                return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de solicitud debe ser mayor o igual a la fecha de presentación en el SAT.");
            }

            if (entity.fechaOficioRespuesta.HasValue)
            {
                if (entity.fechaOficioRespuesta.Value < entity.fechaOficioSolicitud.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de respuesta debe ser mayor a la fecha de oficio de solicitud.");
                }

                if (entity.fechaOficioRespuesta.Value < Convert.ToDateTime(results.fecha_presentacion))
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de respuesta debe ser mayor o igual a la fecha de presentación en el SAT.");
                }
            }

            if (entity.fechaRecepcion.HasValue)
            {
                if (entity.fechaRecepcion.Value < Convert.ToDateTime(results.fecha_presentacion))
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de recepción debe ser mayor a la fecha de presentación en el SAT.");
                }

                if (entity.fechaRecepcion.Value < entity.fechaOficioRespuesta!.Value)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio de recepción debe ser mayor o igual a la fecha de respuesta en el SAT.");
                }
            }

            var result = await _solicitudOpinionInformacionRepository.UpdateComercioExteriorSolicitudOpinioninInformacionAsync(entity, entityDocumento, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GetComercioExteriorSolicitudOpinionInformacionByIdDisconnected(int id)
        {
            try
            {
                var result = await _solicitudOpinionInformacionRepository.GetComercioExteriorSolicitudOpinionInformacionByIdAsync(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }
                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SolicitudOpinionInformacion> GetComercioExteriorSolicitudOpinionInformacionById(int id) =>
            await _solicitudOpinionInformacionRepository.GetComercioExteriorSolicitudOpinionByIdAsync(id);

        public async Task<ResultOperation<int>> UpdateComercioExteriorNoSolicitudOpinionInformacion(Autorizacion entity)
        {
            var result = await _solicitudOpinionInformacionRepository.UpdateComercioExteriorNoSolicitudOpinionInformacionAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GeSolicitudOpinionInformacionComercioExteriorByIdAutorizacion(int idAutorizacion)
        {
            var result = await _solicitudOpinionInformacionRepository.GetComercioExteriorSolicitudOpinionInformacionByIdsAsync(idAutorizacion);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseSolicitudOpinionInformacionAbogadoById>());
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }
        #endregion

        #region Personas Autorizadas
        public async Task<ResultOperation<int>> AddPersonasAutorizadas(PersonasAutorizadas entity)
        {
            try
            {
                var result = await _personasAutorizadasRepository.AddPersonasAutorizadasAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetPersonasAutorizadasByIdDisconnected(int id)
        {
            try
            {
                var result = await _personasAutorizadasRepository.GetPersonasAutorizadasByIdAsync(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseComercioExteriorPersonasAutorizadasAbogadoById>("No se encontraron resultados.");
                }
                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);

                if (!string.IsNullOrEmpty(result.nombre))
                {
                    resultOperation.Result.nombre = result.nombre;
                }

                if (!string.IsNullOrEmpty(result.rfc))
                {
                    resultOperation.Result.rfc = result.rfc;
                }

                if (!string.IsNullOrEmpty(result.telefono))
                {
                    resultOperation.Result.telefono = result.telefono;
                }

                if (!string.IsNullOrEmpty(result.email))
                {
                    resultOperation.Result.email = result.email;
                }
                return resultOperation;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener los datos de las personas autorizadas", ex);
            }
        }

        public async Task<ResultOperation> GetPersonasAutorizadasByIdAutorizacion(int idAutorizacion)
        {
            var result = await _personasAutorizadasRepository.GetByIdAutorizacionAsync(idAutorizacion);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseComercioExteriorPersonasAutorizadasAbogadoById>());
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<PersonasAutorizadas> GetPersonasAutorizadasById(int id) =>
           await _personasAutorizadasRepository.GetByIdAsync(id);

        public async Task<ResultOperation<int>> UpdatePersonasAutorizadas(PersonasAutorizadas entity)
        {
            try
            {
                var result = await _personasAutorizadasRepository.UpdatePersonasAutorizadasAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation<int>> DeletePersonasAutorizadasAsync(PersonasAutorizadas entity)
        {
            try
            {
                var result = await _personasAutorizadasRepository.DeleteAsync(entity.Id);
                if (!result.Success)
                {
                    return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");
                }
                return ResultOperation.SuccessResponseNoMessage(entity.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Cumplimentacion
        public async Task<ResultOperation> GetCumplimentacionByIdDisconnectedAsync(int id)
        {
            try
            {
                var result = await _cumplimentacionRepository.GetCumplimentacionDisconnectedAsync(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }

                _redisClient.ValidateTake(result, EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION);

                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                var modificacionList = await _modificacionRepository.GetByIdAsuntoAsync(null, id);
                if (modificacionList is not null && modificacionList.Any(c => c.activo))
                {
                    resultOperation.Result.idSeccionModificar = modificacionList.FirstOrDefault(c => c.activo)!.id_seccion;
                }

                var descartarList = await _descartarRepository.GetByIdAsuntoAsync(null, id);
                if (descartarList is not null && descartarList.Any(c => c.activo))
                {
                    resultOperation.Result.idSeccionDescartar = descartarList.FirstOrDefault(c => c.activo)!.id_seccion;
                }

                if (result.idTipoAsunto > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAsuntoAutorizaciones, result.idTipoAsunto.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.tipoAsunto = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
                }

                if (result.idTipoAsuntoCumplimentar > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAsuntoAutorizaciones, result.idTipoAsuntoCumplimentar.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.tipoAsuntoCumplimentar = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
                }

                if (result.idTipoModalidad > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoEntradaAutorizaciones, result.idTipoModalidad.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.tipoModalidad = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de modalidad.");
                }

                if (result.idUnidadAdministrativa > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.Administracion, result.idUnidadAdministrativa.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.unidadAdministrativa = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitración.");
                }

                if (result.idSubadministracion > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.Subadministracion, result.idSubadministracion.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.subadministracion = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitración.");
                }

                if (!string.IsNullOrEmpty(result.idAbogado))
                {
                    var response = await _apiService.GetResultOperationAsync<UserInformationView>($"{_catologosEnpoints.RouteInfoUsuario}/{resultOperation.Result.idAbogado}");
                    if (response is not null && response.Success && response.Result is not null)
                    {
                        resultOperation.Result.abogado = response.Result.Nombre;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del abogado.");
                }

                if (result.idEstadoProcesal > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalAutorizaciones, result.idEstadoProcesal.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.estadoProcesal = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del estado procesal.");
                }

                if (result.idEstadoTarea > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TareaAutorizaciones, result.idEstadoTarea.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.estadoTarea = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la tarea.");
                }

                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation<int>> UpdateCumplimentacion(Autorizacion entityAutorizacion, Cumplimentacion entity, UserInformationView userInformationView)
        {
            var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdministracionResponse>($"{_catologosEnpoints.RouteAdministracion}/{entity.id_unidad_administrativa}");
            if (responseUnidadAdministrativa is null || !responseUnidadAdministrativa.Success || responseUnidadAdministrativa.Result is null)
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo validar la unidad administrativa");
            }

            if (userInformationView.EsCentral.GetValueOrDefault() && !responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones centrales.");
            else if (!userInformationView.EsCentral.GetValueOrDefault() && responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones desconcentradas.");

            if (!responseUnidadAdministrativa.Result.isCentral)
                entity.id_subadministracion = entity.id_unidad_administrativa;

            if (entity.id_plazo_cumplimiento == 1 || entity.id_plazo_cumplimiento == 2)
            {
                int dias = entity.id_plazo_cumplimiento == 1 ? 30 : 120;
                var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.AUTORIZACIONES.GetHashCode()}&baseDate={entity.fecha_firmeza!.Value:yyyy-MM-dd}&addDays={dias}&nextDay={true}");
                if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
                {
                    return ResultOperation.FailureWarningResponse<int>("No se pudo calcular la fecha de vencimiento");
                }

                if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
                {
                    return ResultOperation.FailureWarningResponse<int>("La fecha de vencimiento no tiene el formato correcto.");
                }
                entity.fecha_vencimiento = fechaVencimiento;
            }

            var result = await _cumplimentacionRepository.UpdateCumplimentacionAsync(entityAutorizacion, entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GetResolucionCumplimentacionDisconnectedById(int id)
        {
            var result = await _cumplimentacionRepository.GetResolucionCumplimentacionDisconnectedAsync(id);

            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            if (result.idSentidoCumplimentacion > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAsuntoAutorizaciones, result.idSentidoCumplimentacion.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.sentidoCumplimentacion = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de sentido.");
            }

            return resultOperation;
        }

        public async Task<ResultOperation> GetResolucionCumplimentacion(int idCumplimentacion)
        {
            var result = await _cumplimentacionRepository.GetResolucionCumplimentacions(idCumplimentacion);

            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseResolucionCumplimentacion>());
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            foreach (var item in result)
            {
                if (item.idSentidoCumplimentacion.HasValue && item.idSentidoCumplimentacion > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(
                        EnumCatalogos.TipoAsuntoAutorizaciones,
                        item.idSentidoCumplimentacion.Value.ToString()
                    );

                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        item.sentidoCumplimentacion = catalogValue;
                    }
                    else
                    {
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de sentido.");
                    }
                }
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }
        #endregion

        #region Medios de Defensa
        public async Task<ResultOperation> GetMediosDefensa(int idAsunto, int idTipoAsunto, int? idResolucion)
        {
            try
            {
                List<string> listaBusqueda = new();
                if (idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION))
                {
                    var asunto = await _genericService.GetCumplimentacionById(null!, idAsunto);
                    if (asunto is null)
                    {
                        return ResultOperation.SuccessResponseNoMessage(new DTO.MediosDefensaViewResponse());
                    }
                    listaBusqueda.Add(asunto.no_asunto_cumplimentacion!);
                }
                else
                {
                    var asunto = await _genericService.GetAutorizacionById(idAsunto);
                    if (asunto is null)
                    {
                        return ResultOperation.SuccessResponseNoMessage(new DTO.MediosDefensaViewResponse());
                    }
                    listaBusqueda.Add(asunto.no_asunto);
                }

                var resolucionList = await GetResolucionByIdAsuntoAsync(
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION) ? null! : idAsunto,
                    idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION) ? idAsunto! : null!);

                if (idResolucion.GetValueOrDefault(0) > 0)
                {
                    var resolucion = resolucionList.FirstOrDefault(c => c.activo && c.id.Equals(idResolucion));
                    if (resolucion is not null)
                    {
                        listaBusqueda.Add(resolucion.oficio_resolucion);
                    }
                }
                else
                {
                    listaBusqueda.AddRange(resolucionList.Where(c => c.activo).Select(c => c.oficio_resolucion).ToList());
                }

                var request = new
                {
                    idModulo = EnumModulosSicoj.AUTORIZACIONES.GetHashCode(),
                    dato = listaBusqueda
                };

                var result = await _apiService.PostAsync<ResultOperationResponse<DTO.MediosDefensaViewResponse>>(_proxyEnpoints.RouteMediosDefensa, request);
                if (result is null)
                {
                    return ResultOperation.FailureErrorResponse("La respuesta del servicio de medios de defensa no fue el esperado");
                }
                if (!result.Success)
                {
                    var resultOperation = ResultOperation.FailureResponseNoMessage(new DTO.MediosDefensaViewResponse());
                    resultOperation.AddMessages(result.Messages);
                    return resultOperation;
                }

                if (result.Messages.Any())
                {
                    var resultOperation = ResultOperation.FailureResponseNoMessage(result.Result);
                    resultOperation.AddMessages(result.Messages);
                    return resultOperation;
                }

                return ResultOperation.SuccessResponseNoMessage(result.Result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetMediosDefensaDisconnected(List<string> noAsunto)
        {
            try
            {
                var result = await _mediosDefensaRepository.GetMediosDefensaDisconnectedAsync(noAsunto);
                if (result is null || !result.Any())
                {
                    return ResultOperation.SuccessResponseNoMessage(new List<ResponseMediosDefensa>());
                }

                return ResultOperation.SuccessResponseNoMessage(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public async Task<ResultOperation<int>> UpdateCumplimentacionImprocedencia(Cumplimentacion cumplimentacion, UserInformationView userInformationView, Documento entityDocumentoOficio, DataFile dataFile)
        {

            var result = await _cumplimentacionRepository.UpdateImprocedenciaAsync(cumplimentacion, entityDocumentoOficio, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }
    }
}
