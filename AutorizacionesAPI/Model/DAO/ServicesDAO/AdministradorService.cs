using AutorizacionesAPI.Model.DAO.Repository;
using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.CatalogsContracts;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.Entities.Events.Genericos;
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
    public class AdministradorService : IAdministradorService
    {
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        private readonly CatalogosEnpoints _catologosEnpoints;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IAutorizacionRepository _autorizacionRepository;
        private readonly IPerfilAdministradorDisconnectedRepository _disconnectedRepository;        
        private readonly IRemisionRepository _remisionRepository;
        private readonly IRequerimientoProdeconRepository _requerimientoProdeconRepository;
        private readonly ICumplimentacionRepository _cumplimentacionRepository;

        public AdministradorService(
            IApiService apiService, 
            IRedisClient redisClient,
            IOptions<CatalogosEnpoints> catologosEnpoints,
            IOptions<ProxyEnpoints> proxyEnpoints,
            IAutorizacionRepository repositoryComercioExterior, 
            IPerfilAdministradorDisconnectedRepository repositoryAdminAutorizaciones, 
            IRemisionRepository autorizacionesComercioExteriorRemisionRepository, 
            IRequerimientoProdeconRepository autorizacionesComercioExteriorRequerimientoProdeconRepository,
            ICumplimentacionRepository repositoryCumplimentacion)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _catologosEnpoints = catologosEnpoints.Value ?? throw new ArgumentNullException(nameof(catologosEnpoints));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _autorizacionRepository = repositoryComercioExterior ?? throw new ArgumentNullException(nameof(repositoryComercioExterior));
            _disconnectedRepository = repositoryAdminAutorizaciones ?? throw new ArgumentNullException(nameof(repositoryAdminAutorizaciones));
            _remisionRepository = autorizacionesComercioExteriorRemisionRepository ?? throw new ArgumentNullException(nameof(autorizacionesComercioExteriorRemisionRepository));
            _requerimientoProdeconRepository = autorizacionesComercioExteriorRequerimientoProdeconRepository ?? throw new ArgumentNullException(nameof(autorizacionesComercioExteriorRequerimientoProdeconRepository));
            _cumplimentacionRepository = repositoryCumplimentacion ?? throw new ArgumentNullException(nameof(repositoryCumplimentacion));
        }

        #region BandejaPendientes
        public async Task<ResultOperation> GetBandejaPendientesAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, UserInformationView userInformationView)
        {
            var countResult = await _disconnectedRepository.GetBandejaPendientesCountAsync(
                null,
                userInformationView.IdAdministracionCentral,
                userInformationView.IdAdministracion);

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAutorizacionesAdministradorBandeja>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _disconnectedRepository.GetBandejaPendientesAsync(
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc,
                null,
                userInformationView.IdAdministracionCentral,
                userInformationView.IdAdministracion
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
                new DataTableView<ResponseAutorizacionesAdministradorBandeja>(new(Page, Fetch, countResult.GetValueOrDefault()),
                result)
            );
        }
        #endregion

        #region Histórico
        public async Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, string? noAsunto, DateTime? fechaDesde, DateTime? fechaHasta, string? rfc, string? promovente, List<int>? TipoAsunto, List<int>? EstadoTarea, List<int>? TipoModalidad, List<int>? alerta, List<int>? EstadoProcesal, UserInformationView userInformationView)
        {
            if (EstadoTarea is null || !EstadoProcesal!.Any())
            {
                EstadoTarea = new()
                    {
                        EnumEstadoTarea.Asignado.GetHashCode(),
                        EnumEstadoTarea.Atendido.GetHashCode(),
                        EnumEstadoTarea.Concluido_Remitido.GetHashCode(),
                        EnumEstadoTarea.Reasingado.GetHashCode(),
                        EnumEstadoTarea.Atendido.GetHashCode(),
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
                        EnumEstadoProcesal.Resolucion_Emitida.GetHashCode(),
                        EnumEstadoProcesal.Resolucion_Notificada.GetHashCode(),
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
                    new DataTableView<ResponseAutorizacionesAdministradorHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
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

            var cumplimentacionList = result.Where(c => c.idTipoAsuntoCumplimentacion.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION)).ToList();
            if (cumplimentacionList.Any())
            {
                _redisClient.ValidateTakeList(ref cumplimentacionList, EnumModulosRedis.AUTORIZACIONES_CUMPLIMENTACION);
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseAutorizacionesAdministradorHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), result)
            );
        }
        #endregion

        #region Get By Id Disconnected
        public async Task<ResultOperation> GetComercioExteriorByIdDisconnected(int id)
        {
            var resultGeneric = await _autorizacionRepository.GetDisconnectedById(id);
            if (resultGeneric is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseComercioExteriorAdministradorById>("No se encontraron resultados.");
            }
            var result = resultGeneric.Adapt<ResponseComercioExteriorAdministradorById>();

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

            return resultOperation;
        }

        public async Task<ResultOperation> GetImpuestosInternosByIdDisconnected(int id)
        {
            var resultGeneric = await _autorizacionRepository.GetDisconnectedById(id);
            if (resultGeneric is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseComercioExteriorAdministradorById>("No se encontraron resultados.");
            }

            var result = resultGeneric.Adapt<ResponseComercioExteriorAdministradorById>();
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

            return ResultOperation.SuccessResponseNoMessage(result);
        }
        #endregion

        #region ComercioExterior        
        public async Task<ResultOperation<ResponseAsignar>> AsignarAsync(Autorizacion entity, Cumplimentacion cumplimentacion, Abogado entityAbogado)
        {
            var responseAbogado = await _apiService.GetResultOperationAsync<UserInformationView>($"{_catologosEnpoints.RouteInfoUsuario}/{entityAbogado.id_abogado}");
            if (responseAbogado is null || !responseAbogado.Success || responseAbogado.Result is null)
            {
                return ResultOperation.FailureErrorResponse<ResponseAsignar>($"No se pudo realizar la validación para verificar que el abogado seleccionado pertenezca a la administracion/subadministración.");
            }

            if (entity != null)
            {
                if (!UserSession.ValidateUser(responseAbogado!.Result!,EnumRolesSicoj.ABOGADO,EnumModulosSicoj.RECURSO_DE_REVOCACIÓN,out string message, false, null!, true, entity.id_unidad_administrativa_central, true, entity.id_unidad_administrativa, true, entity.id_subadministracion))
                {
                    return ResultOperation.FailureErrorResponse<ResponseAsignar>(message);
                }
            }
            else if (cumplimentacion != null)
            {
                if (!UserSession.ValidateUser(responseAbogado!.Result!, EnumRolesSicoj.ABOGADO, EnumModulosSicoj.RECURSO_DE_REVOCACIÓN, out string message, false, null!, true, cumplimentacion.id_unidad_administrativa_central, true, cumplimentacion.id_unidad_administrativa, true, cumplimentacion.id_subadministracion))
                {
                    return ResultOperation.FailureErrorResponse<ResponseAsignar>(message);
                }
            }

            var result = await _autorizacionRepository.AsignarAsync(entity!, cumplimentacion!, entityAbogado);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseAsignar>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseAsignar()
            {
                noAsunto = entity != null ? entity.no_asunto : cumplimentacion!.no_asunto_cumplimentacion!,
                abogado = responseAbogado.Result.Nombre!
            });
        }

        public async Task<ResultOperation<bool>> RemitirAsync(Autorizacion entity, Remision entityRemision, Documento entityDocumento, DataFile dataFile)
        {
            var result = await _autorizacionRepository.RemitirAsync(entity, entityRemision, entityDocumento, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<bool>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(true);
        }
        #endregion

        #region Remision
        public async Task<ResultOperation> GetRemisionAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int idComercioExterior)
        {
            var countResult = await _remisionRepository.GetRemisionesDisconnectedCount(idComercioExterior);

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseRemision>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _remisionRepository.GetRemisionesDisconnected(
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc,
                idComercioExterior
                );

            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<List<ResponseRemision>>("No se encontraron resultados");
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseRemision>(new(Page, Fetch, countResult.GetValueOrDefault()),
                result)
            );
        }
        #endregion

        public async Task<ResultOperation<ResponseReasignar>> ReAsignarComercioExteriorAsync(int[] idList, string rfcAbogado, UserInformationView userInformationView)
        {
            var responseAbogado = await _apiService.GetResultOperationAsync<UserInformationView>($"{_catologosEnpoints.RouteInfoUsuario}/{rfcAbogado}");
            if (responseAbogado is null || !responseAbogado.Success || responseAbogado.Result is null)
            {
                return ResultOperation.FailureErrorResponse<ResponseReasignar>($"No se pudo realizar la validación para verificar que el abogado seleccionado pertenezca a la administracion/subadministración.");
            }

            List<Autorizacion> entityList = await _autorizacionRepository.GetListByIdsAsync(idList);
            if (entityList is null || !entityList.Any())
            {
                return ResultOperation.FailureErrorResponse<ResponseReasignar>($"No existen las autorizaciones.");
            }

                var resultOperation = ResultOperation.SuccessResponseNoMessage(new ResponseReasignar());

            List<int> listInvalidos = new ();
            List<Reasignar> listReasignacion = new();
            foreach (var item in idList)
            {
                var entity = entityList.FirstOrDefault(c => c.id == item && c.activo);
                if (entity is null)
                {
                    resultOperation.AddWarningMessage($"La autorización con el identificador {item} no existe o se eliminó.");
                    listInvalidos.Add(item);
                    continue;
                }

                try
                {
                    Reasignar entityReasignar = ComercioExteriorEvents.UpdateReasignarAbogado(ref entity,
                        userInformationView.Rfc,
                        responseAbogado.Result.Rfc,
                        entity.abogado.id_abogado, 
                        userInformationView.IdAdministracion, 
                        userInformationView.IdSubadministracion, 
                        responseAbogado.Result.IdAdministracion, 
                        responseAbogado.Result.IdSubadministracion);

                    if (!UserSession.ValidateUser(responseAbogado!.Result!, EnumRolesSicoj.ABOGADO, EnumModulosSicoj.RECURSO_DE_REVOCACIÓN, out string message, false, null!, true, entity.id_unidad_administrativa_central, true, entity.id_unidad_administrativa, true, entityReasignar.id_subadministracion_reasignado))
                    {
                        resultOperation.AddWarningMessage($"La autorización de Comercio Exterior con el número de asunto {entity.no_asunto}: {message}");
                    }

                    listReasignacion.Add(entityReasignar);
                }
                catch (Exception _e)
                {
                    resultOperation.AddWarningMessage($"La autorización de Comercio Exterior con el número de asunto {entity.no_asunto}: {_e.Message}");
                    listInvalidos.Add(item);
                    continue;
                }                
            }

            resultOperation.Result.ReasignacionesExitosas = listReasignacion.Count;
            resultOperation.Result.ReasignacionesIncorrectas = listInvalidos.Count;
            resultOperation.Result.abogado = responseAbogado.Result.Nombre!;

            if (!listReasignacion.Any())
            {
                return resultOperation;
            }

            var result = await _autorizacionRepository.ReasignarAsync(listReasignacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseReasignar>($"{result.MsgError!}:{result.DetailError}");

            return resultOperation;
        }

        public async Task<ResultOperation<int>> RechazarAvisoSinRespuestaAsync(Autorizacion entity, List<int> secciones)
        {
            var result = await _autorizacionRepository.RechazarAvisoSinRespuestaAsync(entity, secciones.ToArray());
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> AprobarAvisoSinRespuestaAsync(Autorizacion entity, AvisoSinRespuesta entityAviso)
        {
            var result = await _autorizacionRepository.AprobarAvisoSinRespuestaAsync(entity, entityAviso);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }
    }
}
