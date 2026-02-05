using AutorizacionesAPI.Model.DAO.Repository;
using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.DTO.CatalogsContracts;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.Entities.Events.Genericos;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;
using System.Linq.Expressions;

namespace AutorizacionesAPI.Model.DAO.ServicesDAO
{
    public class GenericService : IGenericService
    {
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        private readonly CatalogosEnpoints _catologosEnpoints;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IAutorizacionRepository _repositoryAutorizacion;
        private readonly IDocumentoRepository _repositoryArchivosComercioExterior;
        private readonly IModificacionRepository _comercioExteriorModificacionRepository;
        private readonly IDescartarRepository _repositoryComercioExteriorDescartar;
        private readonly ICumplimentacionRepository _repositoryCumplimentacion;
        private readonly IAvisoSinRespuestaRepository _repositoryAvisoSinRespuesta;
        private readonly IAvisoConRespuestaRepository _repositoryAvisoConRespuesta;

        public GenericService(
            IApiService apiService,
            IRedisClient redisClient,
            IOptions<CatalogosEnpoints> catologosEnpoints,
            IOptions<ProxyEnpoints> proxyEnpoints,
            IAutorizacionRepository repositoryComercioExterior, 
            IDocumentoRepository repositoryArchivosComercioExterior,
            IModificacionRepository comercioExteriorModificacionRepository,
            IDescartarRepository repositoryComercioExteriorDescartar, 
            ICumplimentacionRepository repositoryCumplimentacion,
            IAvisoSinRespuestaRepository repositoryAvisoSinRespuesta,
            IAvisoConRespuestaRepository repositoryAvisoConRespuesta)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _catologosEnpoints = catologosEnpoints.Value ?? throw new ArgumentNullException(nameof(catologosEnpoints));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _repositoryAutorizacion = repositoryComercioExterior ?? throw new ArgumentNullException(nameof(repositoryComercioExterior));
            _repositoryArchivosComercioExterior = repositoryArchivosComercioExterior ?? throw new ArgumentNullException(nameof(repositoryArchivosComercioExterior));            
            _comercioExteriorModificacionRepository = comercioExteriorModificacionRepository ?? throw new ArgumentNullException(nameof(comercioExteriorModificacionRepository));
            _repositoryComercioExteriorDescartar = repositoryComercioExteriorDescartar ?? throw new ArgumentNullException(nameof(repositoryComercioExteriorDescartar));
            _repositoryCumplimentacion = repositoryCumplimentacion ?? throw new ArgumentNullException(nameof(repositoryCumplimentacion));
            _repositoryAvisoSinRespuesta = repositoryAvisoSinRespuesta ?? throw new ArgumentNullException(nameof(repositoryAvisoSinRespuesta));
            _repositoryAvisoConRespuesta = repositoryAvisoConRespuesta ?? throw new ArgumentNullException(nameof(repositoryAvisoConRespuesta));

        }

        #region Autorizacion
        public async Task<Autorizacion> GetAutorizacionById(int id) =>
            await _repositoryAutorizacion.GetByIdAsync(id);

        public async Task<Autorizacion> GetAutorizacion(int? id) =>
           await _repositoryAutorizacion.GeAutorizacionAsync(id);

        public async Task<ResultOperation<int>> UpdateAutorizacionAsync(Autorizacion entity, AvisoSinRespuesta avisoSinRespuesta, AvisoConRespuesta avisoConRespuesta, UserInformationView userInformationView, int? idSeccion = null)
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

            if (!responseUnidadAdministrativa.Result.isCentral)
                entity.id_subadministracion = entity.id_unidad_administrativa;

            if (userInformationView.EsCentral.GetValueOrDefault() && !responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones centrales.");
            else if (!userInformationView.EsCentral.GetValueOrDefault() && responseUnidadAdministrativa.Result.isCentral)
                return ResultOperation.FailureWarningResponse<int>("El usuario solo puede registrar información de administraciones desconcentradas.");

            if (!responseUnidadAdministrativa.Result.isCentral)
                entity.id_subadministracion = entity.id_unidad_administrativa;

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

            var result = await _repositoryAutorizacion.UpdateAsync(entity, avisoSinRespuesta, avisoConRespuesta, idSeccion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }
        #endregion

        public async Task<List<Documento>> GetArchivosAutorizacionesByIds(int[] ids) =>
           await _repositoryArchivosComercioExterior.GetByIdsAsync(ids);

        public async Task<ResultOperation<ResponseDocumentoCreate>> AddArchivo(Documento entity, DataFile dataFile)
        {
            var result = await _repositoryArchivosComercioExterior.AddAsync(entity, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseDocumentoCreate>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseDocumentoCreate()
            {
                id = result.Result.GetValueOrDefault(),
                nombreDocumento = entity.file_name
            });
        }

        public async Task<ResultOperation<int>> UpdateArchivo(Documento entity, DataFile dataFile)
        {
            var result = await _repositoryArchivosComercioExterior.UpdateAsync(entity, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<bool>> DeleteArchivo(int[] ids, string usuarioModificacion)
        {
            var result = await _repositoryArchivosComercioExterior.DeleteAsync(ids, usuarioModificacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<bool>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(true);
        }

        public async Task<ResultOperation> GetDocumentosAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAutorizacion, int? idCumplimentacion)
        {
            List<int> listSecciones = new()
            {
                EnumSecciones.AVISOS_Y_COMUNICADOS.GetHashCode(),
                EnumSecciones.AVISO_SIN_RESPUESTA.GetHashCode(),
                EnumSecciones.DATOS_GENERALES.GetHashCode(),
                EnumSecciones.EMISION_RESOLUCION.GetHashCode(),
                EnumSecciones.MEDIOS_DE_DEFENSA.GetHashCode(),
                EnumSecciones.REQUERIMIENTO.GetHashCode(),
                EnumSecciones.REQUERIMIENTO_PRODECON.GetHashCode(),
                EnumSecciones.SOLICITUD_DE_OPINION.GetHashCode(),
                EnumSecciones.SOLICITUD_DE_TRANSPARENCIA.GetHashCode(),
                EnumSecciones.CUMPLIMENTACION.GetHashCode(),
            };

            var countResult = await _repositoryArchivosComercioExterior.GetDocumentosFolioDisconnectedCount(idAutorizacion, idCumplimentacion, listSecciones, null!, true);

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseDocumentoHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _repositoryArchivosComercioExterior.GetDocumentosHistoricoDisconnected(
                true,
                idAutorizacion,
                idCumplimentacion,
                listSecciones,
                null!,
                true,
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc
                );

            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<List<ResponseDocumentoHistorico>>("No se encontraron resultados");
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseDocumentoHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()),
                result)
            );
        }

        public async Task<ResultOperation> GetDocumentosSeccionPaginadoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAutorizacion, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion)
        {
            List<int> listSecciones = new()
            {
                idSeccion,
            };

            var countResult = await _repositoryArchivosComercioExterior.GetDocumentosFolioDisconnectedCount(idAutorizacion, idCumplimentacion, listSecciones, idRenglonSeccion, true);

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseDocumentoHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _repositoryArchivosComercioExterior.GetDocumentosHistoricoDisconnected(
                true,
                idAutorizacion,
                idCumplimentacion,
                listSecciones,
                idRenglonSeccion,
                true,
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc
                );

            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<List<ResponseDocumentoHistorico>>("No se encontraron resultados");
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseDocumentoHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()),
                result)
            );
        }

        public async Task<ResultOperation> GetDocumentosSeccionAsync(int? idAutorizacion, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion)
        {
            List<int> listSecciones = new()
            {
                idSeccion,
            };

            var result = await _repositoryArchivosComercioExterior.GetDocumentosHistoricoDisconnected(
                false,
                idAutorizacion,
                idCumplimentacion,
                listSecciones,
                idRenglonSeccion,
                true
                );

            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseDocumentoHistorico>>(new());
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<List<Documento>> GetDocumentosByRenglonTipoAsync(int? idAutorizacion, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion, int? idTipoDocumento) =>
           await _repositoryArchivosComercioExterior.GetDocumentosByRenglonTipoAsync(idAutorizacion, idCumplimentacion, idSeccion, idRenglonSeccion, idTipoDocumento);

        #region Reasigar
        public async Task<ResultOperation<ResponseReasignar>> ReasignarAsync(int[] idList, UserInformationView userInformationView, string rfcAbogado = null!, string rfcAdministrador = null!)
        {
            List<Autorizacion> entityList = await _repositoryAutorizacion.GetListByIdsAsync(idList);
            if (entityList is null || !entityList.Any())
            {
                return ResultOperation.FailureErrorResponse<ResponseReasignar>($"No existen las autorizaciones.");
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(new ResponseReasignar());
            List<int> listInvalidos = new();
            List<Reasignar> listReasignacion = new();
            if (string.IsNullOrEmpty(rfcAbogado))
            {
                var responseAdministrador = await _apiService.GetResultOperationAsync<UserInformationView>($"{_catologosEnpoints.RouteInfoUsuario}/{rfcAdministrador}");
                if (responseAdministrador is null || !responseAdministrador.Success || responseAdministrador.Result is null)
                {
                    return ResultOperation.FailureErrorResponse<ResponseReasignar>($"No se pudo realizar la validación para verificar que el administrador seleccionado pertenezca a la administracion/subadministración.");
                }

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
                        Reasignar entityReasignar = ComercioExteriorEvents.UpdateReasignarAdministrador(ref entity,
                            userInformationView.Rfc,
                            entity.abogado.id_abogado,
                            userInformationView.IdAdministracion,
                            userInformationView.IdSubadministracion,
                            responseAdministrador.Result.IdAdministracion,
                            responseAdministrador.Result.IdSubadministracion);

                        if (!UserSession.ValidateUser(responseAdministrador!.Result!, EnumRolesSicoj.ADMINISTRADOR, EnumModulosSicoj.RECURSO_DE_REVOCACIÓN, out string message, false, null!, true, entity.id_unidad_administrativa_central))
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

                resultOperation.Result.administrador = responseAdministrador.Result.Nombre!;
            }
            else
            {
                var responseAbogado = await _apiService.GetResultOperationAsync<UserInformationView>($"{_catologosEnpoints.RouteInfoUsuario}/{rfcAbogado}");
                if (responseAbogado is null || !responseAbogado.Success || responseAbogado.Result is null)
                {
                    return ResultOperation.FailureErrorResponse<ResponseReasignar>($"No se pudo realizar la validación para verificar que el abogado seleccionado pertenezca a la administracion/subadministración.");
                }

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

                        if (!UserSession.ValidateUser(responseAbogado!.Result!, EnumRolesSicoj.ABOGADO, EnumModulosSicoj.RECURSO_DE_REVOCACIÓN, out string message, false, null!, true, entity.id_unidad_administrativa_central))
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

                resultOperation.Result.abogado = responseAbogado.Result.Nombre!;
            }

            resultOperation.Result.ReasignacionesExitosas = listReasignacion.Count;
            resultOperation.Result.ReasignacionesIncorrectas = listInvalidos.Count;
            

            if (!listReasignacion.Any())
            {
                return resultOperation;
            }

            var result = await _repositoryAutorizacion.ReasignarAsync(listReasignacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseReasignar>($"{result.MsgError!}:{result.DetailError}");

            return resultOperation;
        }
        #endregion

        #region Modificacion
        public async Task<ResultOperation<int>> AddModificacionAsync(Autorizacion entityAutorizacion, Modificacion entity)
        {
            var result = await _comercioExteriorModificacionRepository.AddAsync(entityAutorizacion, entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }
        public async Task<List<Modificacion>> GetModificacionByIdAsunto(int? idAutorizacion, int? idCumplimentacion) =>
           await _comercioExteriorModificacionRepository.GetByIdAsuntoAsync(idAutorizacion, idCumplimentacion);
        #endregion

        #region Descartar
        public async Task<ResultOperation<int>> DescartarAsync(Autorizacion autorizacion, Cumplimentacion cumplimentacion, Descartar descartar, List<int>? listaSecciones)
        {
            var result = await _repositoryComercioExteriorDescartar.DescartarAsync(autorizacion, cumplimentacion, descartar, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<List<Descartar>> GetDescartarByIdAsunto(int? idAutorizacion, int? idCumplimentacion) =>
           await _repositoryComercioExteriorDescartar.GetByIdAsuntoAsync(idAutorizacion, idCumplimentacion);
        #endregion

        #region Aviso Sin Respuesta
        public async Task<ResultOperation> GetAvisoSinRespuestaByIdDisconnected(int id)
        {
            var result = await _repositoryAvisoSinRespuesta.GetByIdDisconnected(id);
            if (result is null)
            {
                return ResultOperation.SuccessResponse<ResponseAvisoSinRespuestaRelacionado>(null!, "No se encontraron resultados.");
            }

            var folioList = await _repositoryAvisoSinRespuesta.GetFolios(new int[] { id });
            if (folioList is not null && folioList.Any())
            {
                result.folios = folioList.Select(c => c.folio).ToList();
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

        public async Task<ResultOperation> GetAvisoSinRespuestaRelacionadoAsync(int id)
        {
            var result = await _repositoryAvisoSinRespuesta.GetAutorizacionRelacionada(id);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseAvisoSinRespuestaRelacionado>());
            }

            var folioList = await _repositoryAvisoSinRespuesta.GetFolios(result.Select(c => c.id).ToArray());
            if (folioList is not null && folioList.Any())
            {
                foreach (var item in result)
                {
                    var tempList = folioList.Where(c => c.id_aviso_sin_respuesta.Equals(item.id)).ToList();
                    if (tempList is null || !tempList.Any())
                        continue;
                    item.folios = tempList.Select(c => c.folio).ToList();
                }
            }

            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<AvisoSinRespuesta> GetAvisoSinRespuestaById(int id) =>
            await _repositoryAvisoSinRespuesta.GetById(id);
        public async Task<AvisoSinRespuesta> GetAvisoSinRespuestaByIdAutorizacion(int idAutorizacion) =>
            await _repositoryAvisoSinRespuesta.GetByIdAutorizacion(idAutorizacion);

        public async Task<ResultOperation> GetAvisoSinRespuestaPendienteByIdAuntoDisconnected(int id)
        {
            var result = await _repositoryAvisoSinRespuesta.GetByIdAutorizacionDisconnected(id);
            if (result is null)
            {
                return ResultOperation.SuccessResponseNoMessage(new ResponseAvisoSinRespuestaRelacionado());
            }

            var folioList = await _repositoryAvisoSinRespuesta.GetFolios(new int[] { result.id });
            if (folioList is not null && folioList.Any())
            {
                result.folios = folioList.Select(c => c.folio).ToList();
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

        public async Task<ResultOperation> GetAvisoSinRespuestaByIdAsuntoDisconnected(int id)
        {
            var result = await _repositoryAvisoSinRespuesta.GetByIdAsuntoDisconnected(id);
            if (result is null)
            {
                return ResultOperation.SuccessResponseNoMessage(new List<ResponseAvisoSinRespuestaRelacionado>());
            }

            var folioList = await _repositoryAvisoSinRespuesta.GetFolios(new int[] { result.id });
            if (folioList is not null && folioList.Any())
            {
                result.folios = folioList.Select(c => c.folio).ToList();
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
        #endregion

        #region Cumplimentacion
        public async Task<Cumplimentacion> GetCumplimentacionById(int? id, int? idAutorizacion) =>
            await _repositoryCumplimentacion.GetCumplimentacionByIdAsync(id, idAutorizacion);

        public async Task<ResultOperation<int>> DescartarCumplimentacionPorImprocedenciaAsync(Cumplimentacion cumplimentacion, Descartar descartar)
        {
            var result = await _repositoryComercioExteriorDescartar.DescartarCumplimentacionPorImprocedenciaAsync(cumplimentacion, descartar);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> DescartarCumplimentacionNoAsuntoAsync(Cumplimentacion cumplimentacion, Descartar descartar)
        {
            var result = await _repositoryComercioExteriorDescartar.DescartarCumplimentacionAutorizacionAsync(cumplimentacion, descartar);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }
        #endregion

        public async Task<AvisoConRespuesta> GetAvisoConRespuestaByIdAutorizacion(int idAutorizacion) =>
            await _repositoryAvisoConRespuesta.GetByIdAutorizacion(idAutorizacion);
    }
}
