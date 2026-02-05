using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.Administrador
{
    public class ComercioExteriorAdministradorEvents
    {
        public static void UpdateModalidadFisico(ref Autorizacion entityAutorizacion,
            int? idTipoAutorizacion,
            string? rfc,
            string promovente,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string despachosAutorizados,
            int? idAutoridadDirigida,
            string? autoridadDirigida,
            string? domicilioNotificaciones,
            DateTime fechaPresentacion,
            int? idFundamentoSolicitud,
            string? fundamentoSolicitud,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            DateTime fechaRecepcion,
            int? idSubadministracion,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdministradorEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.CatalogValue(ref idTipoAutorizacion, "Tipo de autorización");
            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateStringAlphanumeric(ref promovente!, "Promovente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref despachosAutorizados!, "Despacho o Autorizados");
            Guard.CatalogOtherValue(ref idAutoridadDirigida, ref autoridadDirigida, "Autoridad dirigida", EnumAutoridadDirigida.OTRO.GetHashCode());
            Guard.ValidateStringAlphanumeric(ref domicilioNotificaciones, "Domicilio notificaciones", false);
            Guard.CatalogOtherValue(ref idFundamentoSolicitud, ref fundamentoSolicitud, "Fundamento solicitud", EnumFundamentoSolicitud.OTRO.GetHashCode(), false);
            Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
            Guard.BooleanValue(noIndidaMonto, "No lo indica");
            Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());
            Guard.CatalogValue(ref idSubadministracion, "Subadministración", false);

            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            entityAutorizacion.id_tipo_autorizacion = idTipoAutorizacion.GetValueOrDefault();
            entityAutorizacion.rfc = rfc;
            entityAutorizacion.promovente = promovente.Trim();
            entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
            entityAutorizacion.rfc_contribuyente = rfcContribuyente;
            entityAutorizacion.contribuyente = contribuyente;
            entityAutorizacion.despachos_autorizados = despachosAutorizados.Trim();
            entityAutorizacion.id_autoridad_dirigida = idAutoridadDirigida;
            entityAutorizacion.otra_autoridad_dirigida = autoridadDirigida;
            entityAutorizacion.domicilio_notificaciones = domicilioNotificaciones;
            entityAutorizacion.fecha_presentacion = fechaPresentacion;
            entityAutorizacion.id_fundamento_solicitud = idFundamentoSolicitud;
            entityAutorizacion.otro_fundamento_solicitud = fundamentoSolicitud;
            entityAutorizacion.id_tema = idTema;
            entityAutorizacion.otro_tema = otro_tema;
            entityAutorizacion.no_indica_monto = noIndidaMonto;
            entityAutorizacion.monto = monto;
            entityAutorizacion.fecha_recepcion = fechaRecepcion;
            entityAutorizacion.id_subadministracion = idSubadministracion;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;
        }

        public static void UpdateModalidadLinea(ref Autorizacion entityAutorizacion,
            int? idTipoAutorizacion,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            int? idAutoridadDirigida,
            string? autoridadDirigida,
            string? domicilioNotificaciones,
            DateTime fechaPresentacion,
            int? idFundamentoSolicitud,
            string? fundamentoSolicitud,
            int? idTema,
            string? otro_tema,
            DateTime fechaRecepcion,
            int? idSubadministracion,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdministradorEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.CatalogValue(ref idTipoAutorizacion, "Tipo de autorización");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.CatalogOtherValue(ref idAutoridadDirigida, ref autoridadDirigida, "Autoridad dirigida", EnumAutoridadDirigida.OTRO.GetHashCode());
            Guard.ValidateStringAlphanumeric(ref domicilioNotificaciones, "Domicilio notificaciones", false);
            Guard.CatalogOtherValue(ref idFundamentoSolicitud, ref fundamentoSolicitud, "Fundamento solicitud", EnumFundamentoSolicitud.OTRO.GetHashCode(), false);
            Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
            Guard.CatalogValue(ref idSubadministracion, "Subadministración", false);

            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            entityAutorizacion.id_tipo_autorizacion = idTipoAutorizacion.GetValueOrDefault();
            entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
            entityAutorizacion.rfc_contribuyente = rfcContribuyente;
            entityAutorizacion.contribuyente = contribuyente;
            entityAutorizacion.id_autoridad_dirigida = idAutoridadDirigida;
            entityAutorizacion.otra_autoridad_dirigida = autoridadDirigida;
            entityAutorizacion.domicilio_notificaciones = domicilioNotificaciones;
            entityAutorizacion.fecha_presentacion = fechaPresentacion;
            entityAutorizacion.id_fundamento_solicitud = idFundamentoSolicitud;
            entityAutorizacion.otro_fundamento_solicitud = fundamentoSolicitud;
            entityAutorizacion.id_tema = idTema;
            entityAutorizacion.otro_tema = otro_tema;
            entityAutorizacion.fecha_recepcion = fechaRecepcion;
            entityAutorizacion.id_subadministracion = idSubadministracion;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;
        }

        public static void UpdateAsignar(ref Autorizacion entityAutorizacion,
          string noEmpleado,
          string usuarioMidificacion
        )
        {
            ValidacionEstadosAdministradorEvents.ValidateAutorizacionAsignar(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            if (entityAutorizacion.id_subadministracion is null || entityAutorizacion.id_subadministracion <= 0)
                throw new Exception("Aún no se ha asignado la subadministración.");

            entityAutorizacion.id_estado_tarea = EnumEstadoTarea.Asignado.GetHashCode();
            entityAutorizacion.id_estado_procesal = EnumEstadoProcesal.En_estudio.GetHashCode();
            entityAutorizacion.no_empleado_turna = noEmpleado;
            entityAutorizacion.usuario_modificacion = usuarioMidificacion;
        }

        public static void UpdateRemitir(ref Autorizacion entityAutorizacion,
            Remision remision,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdministradorEvents.ValidateRemision(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            
            if (remision.id_tipo_autoridad == EnumTipoAutoridad.Interna.GetHashCode())
            {
                entityAutorizacion.id_estado_tarea = EnumEstadoTarea.Pendiente_de_turnar.GetHashCode();
                entityAutorizacion.id_estado_procesal = EnumEstadoProcesal.Remitido.GetHashCode();
            }
            else if (remision.id_tipo_autoridad == EnumTipoAutoridad.Externa.GetHashCode())
            {
                entityAutorizacion.id_estado_tarea = EnumEstadoTarea.Concluido_Remitido.GetHashCode();
                entityAutorizacion.id_estado_procesal = EnumEstadoProcesal.Concluido_Remitido.GetHashCode();
            }
            entityAutorizacion.usuario_modificacion = usuarioModificacion;
        }

       
    }
}
