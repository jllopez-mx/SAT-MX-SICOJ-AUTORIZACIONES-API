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
    public class AbogadoService : IAbogadoService
    {
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        private readonly CatalogosEnpoints _catologosEnpoints;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IPerfilAbogadoDisconnectedRepository _repositoryDisconnected;
        private readonly IAutorizacionRepository _autorizacionRepository;
        private readonly IRequerimientoProdeconRepository _requerimientoProdeconRepository;
        private readonly IModificacionRepository _modificacionRepository;
        private readonly IDescartarRepository _descartarRepository;

        public AbogadoService(
            IApiService apiService, 
            IRedisClient redisClient,
            IOptions<CatalogosEnpoints> catologosEnpoints,
            IOptions<ProxyEnpoints> proxyEnpoints, 
            IPerfilAbogadoDisconnectedRepository repositoryDisconnected, 
            IAutorizacionRepository repositoryComercioExterior, 
            IRequerimientoProdeconRepository autorizacionesComercioExteriorRequerimientoProdeconRepository, 
            IModificacionRepository comercioExteriorModificacionRepository, 
            IDescartarRepository comercioExteriorDescartarRepository)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _catologosEnpoints = catologosEnpoints.Value ?? throw new ArgumentNullException(nameof(catologosEnpoints));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _repositoryDisconnected = repositoryDisconnected ?? throw new ArgumentNullException(nameof(repositoryDisconnected));
            _autorizacionRepository = repositoryComercioExterior ?? throw new ArgumentNullException(nameof(repositoryComercioExterior));
            _requerimientoProdeconRepository = autorizacionesComercioExteriorRequerimientoProdeconRepository ?? throw new ArgumentNullException(nameof(autorizacionesComercioExteriorRequerimientoProdeconRepository));
            _modificacionRepository = comercioExteriorModificacionRepository ?? throw new ArgumentNullException(nameof(comercioExteriorModificacionRepository));
            _descartarRepository = comercioExteriorDescartarRepository ?? throw new ArgumentNullException(nameof(comercioExteriorDescartarRepository));
        }

        #region BandejaPendientes
        public async Task<ResultOperation> GetBandejaPendientesAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string? noAsunto,
            DateTime? fechaPresentacionDesde,
            DateTime? fechaPresentacionHasta,
            DateTime? fechaVencimientoDesde,
            DateTime? fechaVencimientoHasta,
            string? rfc,
            string? promovente,
            List<int>? temaList,
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            UserInformationView userInformationView)
        {
            var countResult = await _repositoryDisconnected.GetBandejaPendientesCountAsync(
                noAsunto,
                fechaPresentacionDesde,
                fechaPresentacionHasta,
                fechaVencimientoDesde,
                fechaVencimientoHasta,
                rfc,
                promovente,
                temaList,
                tipoAsuntoList,
                estadoTareaList,
                tipoModalidadList,
                estadoProcesalList,
                userInformationView.IdAdministracionCentral,
                userInformationView.IdAdministracion,
                userInformationView.IdSubadministracion,
                userInformationView.Rfc
                );

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAutorizacionesAbogadoBandeja>(new(page, pageSize, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref page, pageSize);

            var result = await _repositoryDisconnected.GetBandejaPendientesAsync(
                pageSize,
                page,
                orderByColumn,
                orderDesc,
                noAsunto,
                fechaPresentacionDesde,
                fechaPresentacionHasta,
                fechaVencimientoDesde,
                fechaVencimientoHasta,
                rfc,
                promovente,
                temaList,
                tipoAsuntoList,
                estadoTareaList,
                tipoModalidadList,
                estadoProcesalList,
                userInformationView.IdAdministracionCentral,
                userInformationView.IdAdministracion,
                userInformationView.IdSubadministracion,
                userInformationView.Rfc
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
                new DataTableView<ResponseAutorizacionesAbogadoBandeja>(new(page, pageSize, countResult.GetValueOrDefault()),
                result)
            );
        }
        #endregion

        #region Hist�rico
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
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            List<int>? alertaList,
            UserInformationView userInformationView)
        {
            if (estadoTareaList is null || !estadoProcesalList!.Any())
            {
                estadoTareaList = new()
                {
                    EnumEstadoTarea.Asignado.GetHashCode(),
                    EnumEstadoTarea.Atendido.GetHashCode(),
                    EnumEstadoTarea.Concluido_Remitido.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };
            }

            if (estadoProcesalList is null || !estadoProcesalList!.Any())
            {
                estadoProcesalList = new()
                {
                    //EnumEstadoProcesal.En_estudio.GetHashCode(),
                    //EnumEstadoProcesal.Requerido.GetHashCode(),
                    //EnumEstadoProcesal.Resuelto.GetHashCode(),
                    //EnumEstadoProcesal.Aviso_Concluido.GetHashCode(),
                    EnumEstadoProcesal.Concluido_Notificado.GetHashCode(),
                    EnumEstadoProcesal.Concluido_Remitido.GetHashCode(),
                    EnumEstadoProcesal.Resolucion_Emitida.GetHashCode(),
                    EnumEstadoProcesal.Resolucion_Notificada.GetHashCode(),
                    EnumEstadoProcesal.Aviso_Concluido.GetHashCode(),
                };
            }

            var countResult = await _repositoryDisconnected.GetHistoricoCountAsync(
                noAsunto,
                fechaDesde,
                fechaHasta,
                rfc,
                promovente,
                tipoAsuntoList,
                estadoTareaList,
                tipoModalidadList,
                estadoProcesalList,
                alertaList,
                userInformationView.IdAdministracionCentral,
                userInformationView.IdAdministracion
                );
            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAutorizacionesAbogadoHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _repositoryDisconnected.GetHistoricoAsync(
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc,
                noAsunto,
                fechaDesde,
                fechaHasta,
                rfc,
                promovente,
                tipoAsuntoList,
                estadoTareaList,
                tipoModalidadList,
                estadoProcesalList,
                alertaList,
                userInformationView.IdAdministracionCentral,
                userInformationView.IdAdministracion
                );

            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<List<ResponseAutorizacionesAbogadoHistorico>>("No se encontraron resultados");
            }
            return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAutorizacionesAbogadoHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), result));
        }
        #endregion

        #region Get By Id Disconnected
        public async Task<ResultOperation> GetComercioExteriorByIdDisconnected(int id)
        {
            var resultGeneric = await _autorizacionRepository.GetDisconnectedById(id);
            if (resultGeneric is null)
            {
                return ResultOperation.FailureWarningResponse<ResponseComercioExteriorAbogadoById>("No se encontraron resultados.");
            }
            var result = resultGeneric.Adapt<ResponseComercioExteriorAbogadoById>();

            _redisClient.ValidateTake(result, EnumModulosRedis.AUTORIZACIONES);
            var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
            var modificacionList = await _modificacionRepository.GetByIdAsuntoAsync(id, null!);
            if (modificacionList is not null && modificacionList.Any(c => c.activo))
            {
                resultOperation.Result.idSeccionModificar = modificacionList.FirstOrDefault(c => c.activo)!.id_seccion;
            }

            var descartarList = await _descartarRepository.GetByIdAsuntoAsync(id, null);
            if (descartarList is not null && descartarList.Any(c => c.activo))
            {
                resultOperation.Result.idSeccionDescartar = descartarList.FirstOrDefault(c => c.activo)!.id_seccion;
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
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo de autorizaci�n.");
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
                var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdminsitracionCentral>($"{_catologosEnpoints.RouteAdministracion}/{result.idUnidadAdministrativa}");
                if (responseUnidadAdministrativa is not null && responseUnidadAdministrativa.Success && responseUnidadAdministrativa.Result is not null)
                {
                    resultOperation.Result.unidadAdministrativa = responseUnidadAdministrativa.Result.nombre;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitraci�n.");
            }

            if (result.idSubadministracion > 0)
            {
                var response = await _apiService.GetResultOperationAsync<CatalogSicoj>($"{_catologosEnpoints.RouteSubadministracion}/{result.idSubadministracion}");
                if (response is not null && response.Success && response.Result is not null)
                {
                    resultOperation.Result.subadministracion = response.Result.nombre;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitraci�n.");
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
                return ResultOperation.FailureWarningResponse<ResponseComercioExteriorAbogadoById>("No se encontraron resultados.");
            }
            var result = resultGeneric.Adapt<ResponseComercioExteriorAbogadoById>();
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

            var modificacionList = await _modificacionRepository.GetByIdAsuntoAsync(id, null!);
            if (modificacionList is not null && modificacionList.Any(c => c.activo))
            {
                resultOperation.Result.idSeccionModificar = modificacionList.FirstOrDefault(c => c.activo)!.id_seccion;
            }

            var descartarList = await _descartarRepository.GetByIdAsuntoAsync(id, null!);
            if (descartarList is not null && descartarList.Any(c => c.activo))
            {
                resultOperation.Result.idSeccionDescartar = descartarList.FirstOrDefault(c => c.activo)!.id_seccion;
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
                var responseUnidadAdministrativa = await _apiService.GetResultOperationAsync<AdminsitracionCentral>($"{_catologosEnpoints.RouteAdministracion}/{result.idUnidadAdministrativa}");
                if (responseUnidadAdministrativa is not null && responseUnidadAdministrativa.Success && responseUnidadAdministrativa.Result is not null)
                {
                    resultOperation.Result.unidadAdministrativa = responseUnidadAdministrativa.Result.nombre;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitraci�n.");
            }

            if (result.idSubadministracion > 0)
            {
                var response = await _apiService.GetResultOperationAsync<CatalogSicoj>($"{_catologosEnpoints.RouteSubadministracion}/{result.idSubadministracion}");
                if (response is not null && response.Success && response.Result is not null)
                {
                    resultOperation.Result.subadministracion = response.Result.nombre;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre de la unidad administrativa/subadminsitraci�n.");
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
    }
}
