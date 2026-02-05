using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DAO.ServicesDAO
{
    public class AdministradorUnidadCentralService : IAdministradorUnidadCentralService
    {
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        private readonly CatalogosEnpoints _catologosEnpoints;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IPerfilAdministradorUnidadCentralDisconnectedRepository _disconnectedRepository;

        public AdministradorUnidadCentralService(IApiService apiService,
            IRedisClient redisClient,
            IOptions<CatalogosEnpoints> catologosEnpoints,
            IOptions<ProxyEnpoints> proxyEnpoints,
            IPerfilAdministradorUnidadCentralDisconnectedRepository repositoryAdminUnidadCentralAutorizaciones
            )
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _catologosEnpoints = catologosEnpoints.Value ?? throw new ArgumentNullException(nameof(catologosEnpoints));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _disconnectedRepository = repositoryAdminUnidadCentralAutorizaciones ?? throw new ArgumentNullException(nameof(repositoryAdminUnidadCentralAutorizaciones));
        }

        #region Histórico
        public async Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, string? noAsunto, DateTime? fechaDesde, DateTime? fechaHasta, string? rfc, string? promovente, List<int>? TipoAsunto, List<int>? EstadoTarea, List<int>? TipoModalidad, List<int>? EstadoProcesal, UserInformationView userInformationView)
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
                    TipoAsunto,
                    EstadoTarea,
                    TipoModalidad,
                    EstadoProcesal,
                    userInformationView.IdAdministracion
                );

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAutorizacionesAdministradorUAHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
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
                userInformationView.IdAdministracion
            );

            var autorizacionesList = result.Where(c => !c.idTipoAsunto.Equals(EnumTipoAsuntoConst.CUMPLIMENTACION)).ToList();
            if (autorizacionesList.Any())
            {
                _redisClient.ValidateTakeList(ref autorizacionesList, EnumModulosRedis.AUTORIZACIONES);
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseAutorizacionesAdministradorUAHistorico>(new(Page, Fetch, countResult.GetValueOrDefault()), result)
            );
        }
        #endregion
    }
}