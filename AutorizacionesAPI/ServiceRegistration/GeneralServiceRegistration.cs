using AutorizacionesAPI.Model.DAO.Repository;
using AutorizacionesAPI.Model.DAO.ServicesDAO;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.IDAO.IServiceDAO;
using Sicoj.Utils.Extentions;

namespace AutorizacionesAPI.ServiceRegistration
{
    public static class GeneralServiceRegistration
    {
        public static IServiceCollection AddGeneralServices(this IServiceCollection services)
        {
            #region Repositorios
            services.AddScoped<IAutorizacionRepository, AutorizacionRepository>();
            services.AddScoped<IPerfilOficialPartesDisconnectedRepository, PerfilOficialPartesDisconnectedRepository>();
            services.AddScoped<IDocumentoRepository, DocumentoRepository>();
            services.AddScoped<IPerfilAdministradorDisconnectedRepository, PerfilAdministradorDisconnectedRepository>();
            services.AddScoped<IPerfilOficialPartesDisconnectedRepository, PerfilOficialPartesDisconnectedRepository>();
            services.AddScoped<IAbogadoRepository, AbogadoRepository>();
            services.AddScoped<IRemisionRepository, RemisionRepository>();
            services.AddScoped<IPersonasAutorizadasRepository, PersonasAutorizadasRepository>();
            services.AddScoped<IPerfilAbogadoDisconnectedRepository, PerfilAbogadoDisconnectedRepository>();
            services.AddScoped<IRequerimientoProdeconRepository, RequerimientoProdeconRepository>();
            services.AddScoped<IRequerimientoRepository, RequerimientoRepository>();
            services.AddScoped<IResolucionRepository, ResolucionRepository>();
            services.AddScoped<IAvisosComunicadosRepository, AvisosComunicadosRepository>();
            services.AddScoped<IPerfilAdministradorGlobalDisconnectedRepository, PerfilAdministradorGlobalDisconnectedRepository>();
            services.AddScoped<ISolicitudOpinionInformacionRepository, SolicitudOpinionInformacionRepository>();
            services.AddScoped<IModificacionRepository, ModificacionRepository>();
            services.AddScoped<IDescartarRepository, DescartarRepository>();
            services.AddScoped<IPerfilAdministradorUnidadCentralDisconnectedRepository, PerfilAdministradorUnidadCentralDisconnectedRepository>();
            services.AddScoped<ICumplimentacionRepository, CumplimentacionRepository>();
            services.AddScoped<IAvisoSinRespuestaRepository, AvisoSinRespuestaRepository>();
            services.AddScoped<IAvisoConRespuestaRepository, AvisoConRespuestaRepository>();
            services.AddScoped<IMediosDefensaRepository, MediosDefensaRepository>();
            #endregion

            #region Servicios
            services.AddScoped<IOficialPartesService, OficialPartesService>();
            services.AddScoped<IAdministradorService, AdministradorService>();
            services.AddScoped<IAbogadoService, AbogadoService>();
            services.AddScoped<IAdminAbogadoService, AdminAbogadoService>();
            services.AddScoped<IAdministradorGlobalService, AdministradorGlobalService>();
            services.AddScoped<IGenericService, GenericService>();
            services.AddScoped<IAdministradorUnidadCentralService, AdministradorUnidadCentralService>();
            services.AddScoped<ApiService>();
            services.AddScoped<IGenericImplementation, GenericImplementation>();
            #endregion

            return services;
        }
    }
}
