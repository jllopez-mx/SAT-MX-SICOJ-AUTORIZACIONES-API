using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Mapster;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DAO.ServicesDAO
{
    public class AdministradorGlobalService : IAdministradorGlobalService
    {
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        private readonly CatalogosEnpoints _catologosEnpoints;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IPerfilAdministradorGlobalDisconnectedRepository _disconnectedRepository;
        private readonly IAutorizacionRepository _autorizacionRepository;
        private readonly IAbogadoRepository _abogadoRepository;
        private readonly IRequerimientoProdeconRepository _requerimientoProdeconRepository;
        private readonly IResolucionRepository _resolucionRepository;
        private readonly IRequerimientoRepository _requerimientoRepository;

        public AdministradorGlobalService(
            IApiService apiService, 
            IRedisClient redisClient,
            IOptions<CatalogosEnpoints> catologosEnpoints,
            IOptions<ProxyEnpoints> proxyEnpoints, 
            IPerfilAdministradorGlobalDisconnectedRepository repositoryDisconnected, 
            IAutorizacionRepository repositoryComercioExterior,  
            IAbogadoRepository comercioExteriorAbogadoRepository, 
            IRequerimientoProdeconRepository autorizacionesComercioExteriorRequerimientoProdeconRepository, 
            IResolucionRepository impuestosInternosResolucionRepository, 
            IRequerimientoRepository impuestosInternosRequerimientoRepository)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _catologosEnpoints = catologosEnpoints.Value ?? throw new ArgumentNullException(nameof(catologosEnpoints));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _disconnectedRepository = repositoryDisconnected ?? throw new ArgumentNullException(nameof(repositoryDisconnected));
            _autorizacionRepository = repositoryComercioExterior ?? throw new ArgumentNullException(nameof(repositoryComercioExterior));            
            _abogadoRepository = comercioExteriorAbogadoRepository ?? throw new ArgumentNullException(nameof(comercioExteriorAbogadoRepository));
            _requerimientoProdeconRepository = autorizacionesComercioExteriorRequerimientoProdeconRepository ?? throw new ArgumentNullException(nameof(autorizacionesComercioExteriorRequerimientoProdeconRepository));
            _resolucionRepository = impuestosInternosResolucionRepository ?? throw new ArgumentNullException(nameof(impuestosInternosResolucionRepository));
            _requerimientoRepository = impuestosInternosRequerimientoRepository ?? throw new ArgumentNullException(nameof(impuestosInternosRequerimientoRepository));
        }

        public async Task<Abogado> GetAbogadoByIdAutorizacionAsync(int? idAutorizacion, int? idCumplimentacion) =>
            await _abogadoRepository.GetAbogadoAsignadoAsync(idAutorizacion, idCumplimentacion);

        #region Histórico
        public async Task<ResultOperation> GetHistoricoAsync(
            int Fetch, 
            int Page, 
            string? OrderByColumn, 
            bool OrderDesc, 
            string? noAsunto, 
            DateTime? fechaDesde, 
            DateTime? fechaHasta, 
            string? rfc, 
            string? promovente, 
            List<int>? TipoAsunto, 
            List<int>? EstadoTarea, 
            List<int>? TipoModalidad, 
            List<int>? alerta, 
            List<int>? EstadoProcesal, 
            UserInformationView userInformationView)
        {
            if (EstadoTarea is null || !EstadoProcesal!.Any())
            {
                EstadoTarea = new()
                    {
                        EnumEstadoTarea.Pendiente_de_asignar.GetHashCode(),
                        EnumEstadoTarea.Asignado.GetHashCode(),
                        EnumEstadoTarea.Atendido.GetHashCode(),
                        EnumEstadoTarea.Concluido_Remitido.GetHashCode(),
                        EnumEstadoTarea.Reasingado.GetHashCode(),
                    };
            }

            if (EstadoProcesal is null || !EstadoProcesal!.Any())
            {
                EstadoProcesal = new()
                    {
                        EnumEstadoProcesal.En_estudio.GetHashCode(),
                        EnumEstadoProcesal.Requerido.GetHashCode(),
                        EnumEstadoProcesal.Resuelto.GetHashCode(),
                        EnumEstadoProcesal.Aviso_Concluido.GetHashCode(),
                        EnumEstadoProcesal.Concluido_Notificado.GetHashCode(),
                        EnumEstadoProcesal.Concluido_Remitido.GetHashCode(),
                        EnumEstadoProcesal.En_Reparacion.GetHashCode(),
                    };
            }

            var countResult = await _disconnectedRepository.GetHistoricoCountAsync(
                    noAsunto,
                    fechaDesde,
                    fechaHasta,
                    rfc,
                    promovente,
                    alerta,
                    TipoAsunto,
                    EstadoTarea,
                    TipoModalidad,
                    EstadoProcesal,
                    userInformationView.IdAdministracionCentral,
                    userInformationView.IdAdministracion
                );

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAutorizacionesAdministradorGlobalHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
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
                alerta,
                TipoAsunto,
                EstadoTarea,
                TipoModalidad,
                EstadoProcesal,
                userInformationView.IdAdministracionCentral,
                userInformationView.IdAdministracion
            );

            var autorizacionesList = result.Where(c => !c.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION)).ToList();
            if (autorizacionesList.Any())
            {
                _redisClient.ValidateTakeList(ref autorizacionesList, EnumModulosRedis.AUTORIZACIONES);
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseAutorizacionesAdministradorGlobalHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), result)
            );
        }
        #endregion

        #region Get By Id Disconnected
        public async Task<ResultOperation> GetComercioExteriorByIdDisconnected(int id)
        {
            var resultGeneric = await _autorizacionRepository.GetDisconnectedById(id);
            if (resultGeneric is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseImpuestosInternosAdministradorGlobalById>("No se encontraron resultados.");
            }
            var result = resultGeneric.Adapt<ResponseImpuestosInternosAdministradorGlobalById>();

            _redisClient.ValidateTake(result, EnumModulosRedis.AUTORIZACIONES);
            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            if (result.requerimiento is null)
            {
                if (result.solicitudOpinion is null)
                {
                    var prodeconCount = await _requerimientoProdeconRepository.GetByIdAutorizacionCount(result.id);
                    if (prodeconCount <= 0)
                    {

                    }
                    else
                        resultOperation.Result.atencion = true;
                }
                else
                    resultOperation.Result.atencion = true;
            }
            else
                resultOperation.Result.atencion = true;

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

            if (result.idSubadministracion > 0)
            {
                var response = await _apiService.GetResultOperationAsync<CatalogSicoj>($"{_catologosEnpoints.RouteSubadministracion}/{result.idSubadministracion}");
                if (response is not null && response.Success && response.Result is not null)
                {
                    resultOperation.Result.subadministracion = response.Result.nombre;
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

            return resultOperation;
        }

        public async Task<ResultOperation> GetImpuestosInternosByIdDisconnected(int id)
        {
            var resultGeneric = await _autorizacionRepository.GetDisconnectedById(id);
            if (resultGeneric is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseComercioExteriorAdministradorGlobalById>("No se encontraron resultados.");
            }
            var result = resultGeneric.Adapt<ResponseImpuestosInternosAdministradorGlobalById>();

            result.idAutorizacionRelacionado = resultGeneric.idAutorizacionRelacionadoAvisoSinRespuesta is null ? resultGeneric.idAutorizacionRelacionadoAvisoConRespuesta : resultGeneric.idAutorizacionRelacionadoAvisoSinRespuesta;
            if (result.idAutorizacionRelacionado is not null)
            {
                result.idAutorizacionRelacionado = resultGeneric.idAutorizacionRelacionadoAvisoSinRespuesta is null ? resultGeneric.idAutorizacionRelacionadoAvisoConRespuesta : resultGeneric.idAutorizacionRelacionadoAvisoSinRespuesta;
                var asuntoRelacionado = await _autorizacionRepository.GetByIdAsync(result.idAutorizacionRelacionado.GetValueOrDefault());
                if (asuntoRelacionado is not null)
                {
                    result.noAsuntoAviso = asuntoRelacionado.no_asunto;
                }
            }
            else
            {
                result.noAsuntoAviso = string.IsNullOrEmpty(resultGeneric.noAsuntoExternoAvisoSinRespuesta) ? resultGeneric.noAsuntoExternoAvisoConRespuesta : resultGeneric.noAsuntoExternoAvisoSinRespuesta;
            }

            _redisClient.ValidateTake(result, EnumModulosRedis.AUTORIZACIONES);
            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            if (result.requerimiento is null)
            {
                if (result.solicitudOpinion is null)
                {
                    var prodeconCount = await _requerimientoProdeconRepository.GetByIdAutorizacionCount(result.id);
                    if (prodeconCount <= 0)
                    {

                    }
                    else
                        resultOperation.Result.atencion = true;
                }
                else
                    resultOperation.Result.atencion = true;
            }
            else
                resultOperation.Result.atencion = true;

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

            if (result.idSubadministracion > 0)
            {
                var response = await _apiService.GetResultOperationAsync<CatalogSicoj>($"{_catologosEnpoints.RouteSubadministracion}/{result.idSubadministracion}");
                if (response is not null && response.Success && response.Result is not null)
                {
                    resultOperation.Result.subadministracion = response.Result.nombre;
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

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        #endregion

        #region Reactivar
        public async Task<ResultOperation<ResponseReactivar>> ReactivarAsync(Autorizacion entity, Cumplimentacion cumplimentacion, Abogado entityAbogado, Reactivar entityReactivar)
        {
            var responseAbogado = await _apiService.GetResultOperationAsync<UserInformationView>($"{_catologosEnpoints.RouteInfoUsuario}/{entityAbogado.id_abogado}");
            if (responseAbogado is null || !responseAbogado.Success || responseAbogado.Result is null)
            {
                return ResultOperation.FailureErrorResponse<ResponseReactivar>($"No se pudo realizar la validación para verificar que el abogado seleccionado pertenezca a la administracion/subadministración.");
            }

            if (entity != null)
            {
                if (!UserSession.ValidateUser(responseAbogado!.Result!, EnumRolesSicoj.ABOGADO, EnumModulosSicoj.RECURSO_DE_REVOCACIÓN, out string message, false, null!, true, entity.id_unidad_administrativa_central, true, entity.id_unidad_administrativa, true, entity.id_subadministracion))
                {
                    return ResultOperation.FailureErrorResponse<ResponseReactivar>(message);
                }
            }
            else if (cumplimentacion != null)
            {
                if (!UserSession.ValidateUser(responseAbogado!.Result!, EnumRolesSicoj.ABOGADO, EnumModulosSicoj.RECURSO_DE_REVOCACIÓN, out string message, false, null!, true, cumplimentacion.id_unidad_administrativa_central, true, cumplimentacion.id_unidad_administrativa, true, cumplimentacion.id_subadministracion))
                {
                    return ResultOperation.FailureErrorResponse<ResponseReactivar>(message);
                }
            }

            var result = await _autorizacionRepository.ReactivarAsync(entity!, cumplimentacion!, entityAbogado, entityReactivar);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseReactivar>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseReactivar()
            {
                noAsunto = entity != null ? entity.no_asunto : cumplimentacion!.no_asunto_cumplimentacion!,
            });
        }
        #endregion

        #region Resolución
        public async Task<ResultOperation<ResponseResolucionDescartar>> ResolucionDescartarAsync(Autorizacion entityAutorizaciones, Descartar entity, List<int>? listaSecciones)
        {            
            var result = await _resolucionRepository.DescartarAsync(entityAutorizaciones, entity, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseResolucionDescartar>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseResolucionDescartar()
            {
                noAsunto = entityAutorizaciones.no_asunto
            });
        }
        #endregion

        #region Requerimiento
        public async Task<ResultOperation<ResponseRequerimientoDescartar>> RequerimientoDescartarAsync(Autorizacion entityAutorizaciones, Descartar entity, List<int>? listaSecciones)
        {
            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.AUTORIZACIONES.GetHashCode()}&baseDate={entityAutorizaciones.fecha_presentacion:yyyy-MM-dd}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<ResponseRequerimientoDescartar>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<ResponseRequerimientoDescartar>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entityAutorizaciones.fecha_vencimiento = fechaVencimiento;

            var result = await _requerimientoRepository.DescartarAsync(entityAutorizaciones, entity, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseRequerimientoDescartar()
            {
                noAsunto = entityAutorizaciones.no_asunto
            });
        }

        public async Task<ResultOperation<ResponseRequerimientoDescartar>> RequerimientoDescartarUltimoAsync(Autorizacion entityAutorizaciones, Requerimiento entity, int idSeccionRequerimiento)
        {
            var result = await _requerimientoRepository.DescartarUltimoAsync(entity, idSeccionRequerimiento);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartar>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseRequerimientoDescartar()
            {
                noAsunto = entityAutorizaciones.no_asunto
            });
        }
        #endregion
    }
}
