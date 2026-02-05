using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.CatalogsContracts;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Mapster;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DAO.ServicesDAO
{
    public class OficialPartesService : IOficialPartesService
    {
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        private readonly CatalogosEnpoints _catologosEnpoints;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IAutorizacionRepository _autorizacionRepository;
        private readonly IPerfilOficialPartesDisconnectedRepository _disconnectedRepository;
        private readonly ICumplimentacionRepository _cumplimentacionRepository;

        public OficialPartesService(
            IApiService apiService,
            IRedisClient redisClient,
            IOptions<CatalogosEnpoints> catologosEnpoints,
            IOptions<ProxyEnpoints> proxyEnpoints,
            IAutorizacionRepository repositoryComercioExterior,
            IPerfilOficialPartesDisconnectedRepository repositoryAutorizacionesDisconnected,
            ICumplimentacionRepository repositoryCumplimentacion)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _catologosEnpoints = catologosEnpoints.Value ?? throw new ArgumentNullException(nameof(catologosEnpoints));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _autorizacionRepository = repositoryComercioExterior ?? throw new ArgumentNullException(nameof(repositoryComercioExterior));
            _disconnectedRepository = repositoryAutorizacionesDisconnected ?? throw new ArgumentNullException(nameof(repositoryAutorizacionesDisconnected));
            _cumplimentacionRepository = repositoryCumplimentacion ?? throw new ArgumentNullException(nameof(repositoryCumplimentacion));
        }

        public async Task<ResultOperation> GetBandejaPendientesAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, UserInformationView userInformationView)
        {
            var countResult = await _disconnectedRepository.GetBandejaPendientesCountAsync(null!, userInformationView.IdAdministracionCentral);

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAutorizacionesOficialPartesBandeja>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _disconnectedRepository.GetBandejaPendientesAsync(
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc,
                null!,
                userInformationView.IdAdministracionCentral
            );

            var autorizacionesList = result.Where(c => !c.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION)).ToList();
            if (autorizacionesList.Any())
            {
                _redisClient.ValidateTakeList(ref autorizacionesList, EnumModulosRedis.AUTORIZACIONES);
            }

            var cumplimentacionList = result.Where(c => c.idTipoAsuntoCumplimentacion.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION)).ToList();
            if (cumplimentacionList.Any())
            {
                _redisClient.ValidateTakeList(ref cumplimentacionList, EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION);
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseAutorizacionesOficialPartesBandeja>(new(Page, Fetch, countResult.GetValueOrDefault()),
                result)
            );
        }

        public async Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, string? noAsunto, DateTime? fechaDesde, DateTime? fechaHasta, string? rfc, string? promovente, List<int>? TipoAsunto, List<int>? EstadoTarea, List<int>? TipoModalidad, List<int>? EstadoProcesal, UserInformationView userInformationView)
        {
            if (EstadoTarea is null || !EstadoProcesal!.Any())
            {
                EstadoTarea = new()
                    {
                        EnumEstadoTarea.Pendiente_de_asignar.GetHashCode(),
                        //EnumEstadoTarea.Asingado.GetHashCode(),
                        //EnumEstadoTarea.Atendido.GetHashCode(),
                        //EnumEstadoTarea.Concluido_Remitido.GetHashCode(),
                        //EnumEstadoTarea.Reasingado.GetHashCode(),
                    };
            }

            if (EstadoProcesal is null || !EstadoProcesal!.Any())
            {
                EstadoProcesal = new()
                    {
                        EnumEstadoProcesal.En_estudio.GetHashCode(),
                        //EnumEstadoProcesal.Aviso_Pendiente.GetHashCode(),
                        //EnumEstadoProcesal.Requerido.GetHashCode(),
                        //EnumEstadoProcesal.Resuelto.GetHashCode(),
                        //EnumEstadoProcesal.Aviso_Concluido.GetHashCode(),
                        //EnumEstadoProcesal.Concluido_Notificado.GetHashCode(),
                        //EnumEstadoProcesal.Concluido_Remitido.GetHashCode(),
                    };
            }

            var countResult = await _disconnectedRepository.GetHistoricoCountAsync(
                    noAsunto,
                    fechaDesde,
                    fechaHasta,
                    rfc,
                    promovente,
                    TipoAsunto,
                    EstadoTarea,
                    TipoModalidad,
                    EstadoProcesal,
                    userInformationView.IdAdministracionCentral
                );

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAutorizacionesOficialPartesHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _disconnectedRepository.GetHistoricoAsync(
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc,
                noAsunto,
                fechaDesde,
                fechaHasta,
                rfc,
                promovente,
                TipoAsunto,
                EstadoTarea,
                TipoModalidad,
                EstadoProcesal,
                userInformationView.IdAdministracionCentral
            );

            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesOficialPartesHistorico>>("No se encontraron resultados");
            }

            var autorizacionesList = result.Where(c => !c.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION)).ToList();
            if (autorizacionesList.Any())
            {
                _redisClient.ValidateTakeList(ref autorizacionesList, EnumModulosRedis.AUTORIZACIONES);
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseAutorizacionesOficialPartesHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()),
                result)
            );
        }

        public async Task<ResultOperation> GetAutorizacionByIdDisconnected(int id)
        {
            var resultGeneric = await _autorizacionRepository.GetDisconnectedById(id);
            if (resultGeneric is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseComercioExteriorOficialPartesById>("No se encontraron resultados.");
            }
            var result = resultGeneric.Adapt<ResponseComercioExteriorOficialPartesById>();

            _redisClient.ValidateTake(result, EnumModulosRedis.AUTORIZACIONES);
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

            if (result.idTipoAutorizacion > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAutorizacion, result.idTipoAutorizacion.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.tipoAutorizacion = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de autorización.");
            }

            if (result.idFundamentoSolicitud > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.FundamentoSolicitud, result.idFundamentoSolicitud.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.fundamentoSolicitud = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del fundamento solicitud.");
            }

            if (result.idAutoridadDirigida > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.AutoridadDirige, result.idAutoridadDirigida.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.autoridadDirigida = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de autoridad a la que se dirige.");
            }

            if (result.idTema > 0)
            {
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TemaAutorizaciones, result.idTema.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.tema = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de autoridad a la que se dirige.");
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

            return resultOperation;
        }

        public async Task<ResultOperation> GetImpuestosInternosByIdDisconnected(int id)
        {
            var resultGeneric = await _autorizacionRepository.GetDisconnectedById(id);
            if (resultGeneric is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseImpuestosInternosById>("No se encontraron resultados.");
            }
            var result = resultGeneric.Adapt<ResponseImpuestosInternosById>();

            _redisClient.ValidateTake(result, EnumModulosRedis.AUTORIZACIONES);
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

            return resultOperation;
        }

        public async Task<ResultOperation<int>> AddAutorizacionFisico(Autorizacion entity, UserInformationView userInformationView)
        {
            DateTime dateTime = DateTime.Now;
            if (entity.fecha_presentacion.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_recepcion.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_recepcion.Date < entity.fecha_presentacion.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser menor a la fecha de recepción.");
            }

            var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdministracionResponse>($"{_catologosEnpoints.RouteAdministracion}/{entity.id_unidad_administrativa}");
            if (responseUnidadAdministrativa is null || !responseUnidadAdministrativa.Success || responseUnidadAdministrativa.Result is null)
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo validar la unidad administrativa");
            }

            if (userInformationView.EsCentral.GetValueOrDefault() && !responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones centrales.");
            else if (!userInformationView.EsCentral.GetValueOrDefault() && responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones desconcentradas.");

            entity.id_subadministracion = responseUnidadAdministrativa.Result.isCentral ? null! : entity.id_unidad_administrativa;

            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.AUTORIZACIONES.GetHashCode()}&baseDate={entity.fecha_presentacion:yyyy-MM-dd}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entity.fecha_vencimiento = fechaVencimiento;

            var result = await _autorizacionRepository.AddAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            entity.id = result.Result.GetValueOrDefault();
            await _redisClient.AddChangeLog(new()
            {
                Log = entity,
                Module = EnumModulosSicoj.AUTORIZACIONES,
                Operation = OperationChangeLog.INSERT,
                RequestPath = "",
                Role = EnumRolesSicoj.OFICIAL_DE_PARTES.ToString(),
                TableName = "Autorizaciones",
                User = userInformationView.Rfc!
            });
            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> UpdateAutorizacion(Autorizacion entity, UserInformationView userInformationView)
        {
            DateTime dateTime = DateTime.Now;
            if (entity.fecha_presentacion.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_recepcion.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser mayor a la fecha actual.");
            }

            if (entity.fecha_recepcion.Date < entity.fecha_presentacion.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser menor a la fecha de recepción.");
            }

            var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdministracionResponse>($"{_catologosEnpoints.RouteAdministracion}/{entity.id_unidad_administrativa}");
            if (responseUnidadAdministrativa is null || !responseUnidadAdministrativa.Success || responseUnidadAdministrativa.Result is null)
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo validar la unidad administrativa");
            }

            if (userInformationView.EsCentral.GetValueOrDefault() && !responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones centrales.");
            else if (!userInformationView.EsCentral.GetValueOrDefault() && responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones desconcentradas.");

            entity.id_subadministracion = responseUnidadAdministrativa.Result.isCentral ? null! : entity.id_unidad_administrativa;

            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.AUTORIZACIONES.GetHashCode()}&baseDate={entity.fecha_presentacion:yyyy-MM-dd}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entity.fecha_vencimiento = fechaVencimiento;

            var result = await _autorizacionRepository.UpdateAsync(entity, null!, null!);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> UpdateAutorizacionLinea(Autorizacion entity, UserInformationView userInformationView)
        {
            DateTime dateTime = DateTime.Now;
            if (entity.fecha_recepcion.Date > dateTime.Date)
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser mayor a la fecha actual.");
            }

            var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdministracionResponse>($"{_catologosEnpoints.RouteAdministracion}/{entity.id_unidad_administrativa}");
            if (responseUnidadAdministrativa is null || !responseUnidadAdministrativa.Success || responseUnidadAdministrativa.Result is null)
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo validar la unidad administrativa");
            }

            if (userInformationView.EsCentral.GetValueOrDefault() && !responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones centrales.");
            else if (!userInformationView.EsCentral.GetValueOrDefault() && responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones desconcentradas.");

            entity.id_subadministracion = responseUnidadAdministrativa.Result.isCentral ? null! : entity.id_unidad_administrativa;

            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.AUTORIZACIONES.GetHashCode()}&baseDate={entity.fecha_presentacion:yyyy-MM-dd}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<int>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entity.fecha_vencimiento = fechaVencimiento;

            var result = await _autorizacionRepository.UpdateAsync(entity, null!, null!);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<ResponseTurnado>> TurnarAsync(Autorizacion entity)
        {
            var result = await _autorizacionRepository.TurnarAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseTurnado>($"{result.MsgError!}:{result.DetailError}");

            var resultOperation = ResultOperation.SuccessResponseNoMessage(new ResponseTurnado());

            var entityExists = await _autorizacionRepository.GetByIdAsync(result.Result.GetValueOrDefault());
            if (entityExists is not null)
            {
                resultOperation.Result.noAsunto = entityExists.no_asunto;
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.Administracion, entity.id_unidad_administrativa.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.unidadAdministrativa = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitración.");
            }
            else
                resultOperation.AddWarningMessage("No se pudo recuperar el la información de la autorización despues de turnarla.");

            return resultOperation;
        }

        public async Task<ResultOperation<bool>> DeleteAutorizacionAsync(Autorizacion entity)
        {
            var result = await _autorizacionRepository.DeleteAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<bool>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(true);
        }

        #region Cumplimentación
        public async Task<ResultOperation> AddCumplimentacionAsync(Autorizacion entityAutorizacion, Cumplimentacion entityCumplimentacion, UserInformationView userInformationView)
        {
            if (!entityAutorizacion.externo_cumplimentacion)
            {
                var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdministracionResponse>($"{_catologosEnpoints.RouteAdministracion}/{entityAutorizacion!.id_unidad_administrativa}");
                if (responseUnidadAdministrativa is null || !responseUnidadAdministrativa.Success || responseUnidadAdministrativa.Result is null)
                {
                    return ResultOperation.FailureWarningResponse<int>("No se pudo validar la unidad administrativa");
                }

                if (userInformationView.EsCentral.GetValueOrDefault() && !responseUnidadAdministrativa.Result.isCentral)
                    return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones centrales.");
                else if (!userInformationView.EsCentral.GetValueOrDefault() && responseUnidadAdministrativa.Result.isCentral)
                    return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones desconcentradas.");
            }
            
            var result = await _cumplimentacionRepository.AddCumplimentacionAsync(entityAutorizacion, entityCumplimentacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation> GetCumplimentacionByIdDisconnectedAsync(int id)
        {
            try
            {
                var result = await _cumplimentacionRepository.GetCumplimentacionByIdDisconnectedAsync(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }

                _redisClient.ValidateTake(result, EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION);

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

                if (result.idTipoAsuntoCumplimentar > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAsuntoAutorizaciones, result.idTipoAsuntoCumplimentar.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.tipoAsuntoCumplimentacion = catalogValue;
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

                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Autorizacion> GetAutorizacionByNoAsunto(string noAsunto) =>
           await _autorizacionRepository.GetByNumeroAsuntoAsync(noAsunto);

        public async Task<ResultOperation> GetCumplimentacionBuscarNoAsuntoAsync(string noAsunto, UserInformationView userInformationView)
        {
            var resultOperation = ResultOperation.SuccessResponseNoMessage(new ResponseCumplimentacionNumeroAsunto());
            var result = await this.GetAutorizacionByNoAsunto(noAsunto);
            if (result is not null)
            {
                if (result.externo_cumplimentacion)
                {
                    return ResultOperation.SuccessResponseNoMessage($"Ya existe una cumplimentación con el número de asunto externo {noAsunto}");
                }
                else
                {
                    var cumplimentacionExterna = await _cumplimentacionRepository.GetCumplimentacionByIdAsync(null!, result.id);
                    if (cumplimentacionExterna is not null)
                    {
                        return ResultOperation.SuccessResponseNoMessage($"Ya existe una cumplimentación para la autorización {noAsunto}");
                    }
                }

                string tipoAsunto = null!;
                if (result!.id_tipo_asunto > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.TipoAsuntoAutorizaciones, result.id_tipo_asunto.ToString()!);
                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        tipoAsunto = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
                }

                return ResultOperation.SuccessResponseNoMessage(new ResponseCumplimentacionNumeroAsunto()
                {
                    idAsunto = result.id,
                    idTipoAsunto = result.id_tipo_asunto,
                    noAsunto = result.no_asunto,
                    promovente = result.promovente!,
                    rfc = result.rfc!,
                    tipoAsunto = tipoAsunto
                });
            }

            return ResultOperation.SuccessResponseNoMessage(new ResponseCumplimentacionNumeroAsunto());
        }

        public async Task<ResultOperation<ResponseTurnado>> TurnarCumplimentacionAsync(Cumplimentacion entity)
        {
            var result = await _cumplimentacionRepository.TurnarCumplimentacionAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseTurnado>($"{result.MsgError!}:{result.DetailError}");

            var resultOperation = ResultOperation.SuccessResponseNoMessage(new ResponseTurnado());

            var entityExists = await _cumplimentacionRepository.GetCumplimentacionByIdAsync(result.Result.GetValueOrDefault(), null!);
            if (entityExists is not null)
            {
                resultOperation.Result.noAsunto = entityExists.no_asunto_cumplimentacion!;
                var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.Administracion, entity.id_unidad_administrativa.ToString()!);
                if (!string.IsNullOrEmpty(catalogValue))
                {
                    resultOperation.Result.unidadAdministrativa = catalogValue;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitración.");
            }
            else
                resultOperation.AddWarningMessage("No se pudo recuperar el la información de la autorización despues de turnarla.");

            return resultOperation;
        }

        public async Task<ResultOperation<int>> UpdateCumplimentacion(Autorizacion entityAutorizacion, Cumplimentacion entityCumplimentacion, UserInformationView userInformationView)
        {
            var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdministracionResponse>($"{_catologosEnpoints.RouteAdministracion}/{entityCumplimentacion.id_unidad_administrativa}");
            if (responseUnidadAdministrativa is null || !responseUnidadAdministrativa.Success || responseUnidadAdministrativa.Result is null)
            {
                return ResultOperation.FailureWarningResponse<int>("No se pudo validar la unidad administrativa");
            }

            if (userInformationView.EsCentral.GetValueOrDefault() && !responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones centrales.");
            else if (!userInformationView.EsCentral.GetValueOrDefault() && responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones desconcentradas.");

            entityCumplimentacion.id_subadministracion = responseUnidadAdministrativa.Result.isCentral ? null! : entityCumplimentacion.id_unidad_administrativa;

            if (entityCumplimentacion.id_plazo_cumplimiento == 1 || entityCumplimentacion.id_plazo_cumplimiento == 2)
            {
                int dias = entityCumplimentacion.id_plazo_cumplimiento == 1 ? 30 : 120;
                var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.AUTORIZACIONES.GetHashCode()}&baseDate={entityCumplimentacion.fecha_firmeza!.Value:yyyy-MM-dd}&addDays={dias}&nextDay={true}");
                if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
                {
                    return ResultOperation.FailureWarningResponse<int>("No se pudo calcular la fecha de vencimiento");
                }

                if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
                {
                    return ResultOperation.FailureWarningResponse<int>("La fecha de vencimiento no tiene el formato correcto.");
                }

                entityCumplimentacion.fecha_vencimiento = fechaVencimiento;
            }

            var result = await _cumplimentacionRepository.UpdateAsync(entityAutorizacion, entityCumplimentacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }
        #endregion
    }
}
