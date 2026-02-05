using AutorizacionesAPI.Model.Entities.Events.Administrador;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;
using System.Threading;

namespace AutorizacionesAPI.Model.Entities.Events.OficialPartes
{
    public class AutorizacionOficialPartesEvents
    {
        public static Autorizacion CreateModalidadFisico(
            int idTipoAsunto,
            int? idTipoAutorizacion,
            string? rfc,
            string promovente,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string despachosAutorizados,
            int? idAutoridadDirigida,
            string? autoridadDirigida,
            string domicilioPromovente,
            string? domicilioNotificaciones,
            DateTime fechaPresentacion,
            int? idFundamentoSolicitud,
            string? fundamentoSolicitud,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            DateTime fechaRecepcion,
            int? idUnidadAdministrativaCentral,
            int? idUnidadAdministrativa,
            string usuarioCreacion
        )
        {
            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateStringAlphanumeric(ref promovente!, "Promovente");
            Guard.BooleanValue(promoventeNoContribuyente, "Promovente no es contribuyente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringEmpty(ref domicilioPromovente, "Domicilio promovente");
            Guard.CatalogValue(ref idUnidadAdministrativaCentral, "Unidad administrativa Central");
            Guard.CatalogValue(ref idUnidadAdministrativa, "Unidad administrativa", false);
            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            switch (idTipoAsunto)
            {
                case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    Guard.CatalogValue(ref idTipoAutorizacion, "Tipo de autorización");
                    Guard.ValidateStringAlphanumeric(ref despachosAutorizados!, "Despacho o Autorizados");
                    Guard.CatalogOtherValue(ref idAutoridadDirigida, ref autoridadDirigida, "Autoridad dirigida", EnumAutoridadDirigida.OTRO.GetHashCode());
                    Guard.ValidateStringAlphanumeric(ref domicilioNotificaciones, "Domicilio notificaciones", false);
                    Guard.CatalogOtherValue(ref idFundamentoSolicitud, ref fundamentoSolicitud, "Fundamento solicitud", EnumFundamentoSolicitud.OTRO.GetHashCode(), false);
                    Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
                    Guard.BooleanValue(noIndidaMonto, "No lo indica");
                    Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());

                    return new()
                    {
                        id_tipo_asunto = idTipoAsunto,
                        id_tipo_modalidad = EnumTipoModalidad.FÍSICO.GetHashCode(),
                        id_tipo_autorizacion = idTipoAutorizacion.GetValueOrDefault(),
                        rfc = rfc,
                        promovente = promovente,
                        promovente_no_contribuyente = promoventeNoContribuyente,
                        rfc_contribuyente = rfcContribuyente,
                        contribuyente = contribuyente,
                        despachos_autorizados = despachosAutorizados,
                        id_autoridad_dirigida = idAutoridadDirigida,
                        otra_autoridad_dirigida = autoridadDirigida,
                        domicilio_promovente = domicilioPromovente,
                        domicilio_notificaciones = domicilioNotificaciones,
                        fecha_presentacion = fechaPresentacion,
                        id_fundamento_solicitud = idFundamentoSolicitud,
                        otro_fundamento_solicitud = fundamentoSolicitud,
                        id_tema = idTema,
                        otro_tema = otro_tema,
                        no_indica_monto = noIndidaMonto,
                        monto = monto,
                        fecha_recepcion = fechaRecepcion,
                        id_unidad_administrativa_central = idUnidadAdministrativaCentral!.Value,
                        id_unidad_administrativa = idUnidadAdministrativa,
                        id_estado_tarea = EnumEstadoTarea.Pendiente_de_turnar.GetHashCode(),
                        id_estado_procesal = EnumEstadoProcesal.Activo.GetHashCode(),
                        usuario_creacion = usuarioCreacion
                    };
                case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:                    
                    return new()
                    {
                        id_tipo_asunto = idTipoAsunto,
                        id_tipo_modalidad = EnumTipoModalidad.FÍSICO.GetHashCode(),
                        rfc = rfc,
                        promovente = promovente,
                        promovente_no_contribuyente = promoventeNoContribuyente,
                        rfc_contribuyente = rfcContribuyente,
                        contribuyente = contribuyente,
                        domicilio_promovente = domicilioPromovente,
                        fecha_presentacion = fechaPresentacion,
                        fecha_recepcion = fechaRecepcion,
                        id_unidad_administrativa_central = idUnidadAdministrativaCentral.GetValueOrDefault(),
                        id_unidad_administrativa = idUnidadAdministrativa,
                        id_estado_tarea = EnumEstadoTareaCons.Pendiente_de_turnar,
                        id_estado_procesal = EnumEstadoProcesalCons.Activo,
                        usuario_creacion = usuarioCreacion,
                    };
                case EnumTipoAsuntoConst.DONATARIAS:
                    return new()
                    {
                        id_tipo_asunto = idTipoAsunto,
                        id_tipo_modalidad = EnumTipoModalidad.FÍSICO.GetHashCode(),
                        rfc = rfc,
                        promovente = promovente,
                        promovente_no_contribuyente = promoventeNoContribuyente,
                        rfc_contribuyente = rfcContribuyente,
                        contribuyente = contribuyente,
                        domicilio_promovente = domicilioPromovente,
                        fecha_presentacion = fechaPresentacion,
                        fecha_recepcion = fechaRecepcion,
                        id_unidad_administrativa_central = idUnidadAdministrativaCentral.GetValueOrDefault(),
                        id_unidad_administrativa = idUnidadAdministrativa,
                        id_estado_tarea = EnumEstadoTareaCons.Pendiente_de_turnar,
                        id_estado_procesal = EnumEstadoProcesalCons.Activo,
                        usuario_creacion = usuarioCreacion,
                    };
                default:
                    return null!;
            }
        }

        public static void UpdateGuardarModalidadFisico(ref Autorizacion entityAutorizacion,
            int? idTipoAutorizacion,
            string? rfc,
            string promovente,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string despachosAutorizados,
            int? idAutoridadDirigida,
            string? autoridadDirigida,
            string domicilioPromovente,
            string? domicilioNotificaciones,
            DateTime fechaPresentacion,
            int? idFundamentoSolicitud,
            string? fundamentoSolicitud,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            DateTime fechaRecepcion,
            int? idUnidadAdministrativa,
            string usuarioModificacion
        )
        {
            ValidacionEstadosOficialPartesEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateStringAlphanumeric(ref promovente!, "Promovente");
            Guard.BooleanValue(promoventeNoContribuyente, "Promovente no es contribuyente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringEmpty(ref domicilioPromovente, "Domicilio promovente");
            Guard.CatalogValue(ref idUnidadAdministrativa, "Unidad administrativa", false);

            switch (entityAutorizacion.id_tipo_asunto)
            {
                case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    Guard.CatalogValue(ref idTipoAutorizacion, "Tipo de autorización");
                    Guard.ValidateStringAlphanumeric(ref despachosAutorizados!, "Despacho o Autorizados");
                    Guard.CatalogOtherValue(ref idAutoridadDirigida, ref autoridadDirigida, "Autoridad dirigida", EnumAutoridadDirigida.OTRO.GetHashCode());
                    Guard.ValidateStringAlphanumeric(ref domicilioNotificaciones, "Domicilio notificaciones", false);
                    Guard.CatalogOtherValue(ref idFundamentoSolicitud, ref fundamentoSolicitud, "Fundamento solicitud", EnumFundamentoSolicitud.OTRO.GetHashCode(), false);
                    Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
                    Guard.BooleanValue(noIndidaMonto, "No lo indica");
                    Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());

                    entityAutorizacion.id_tipo_autorizacion = idTipoAutorizacion.GetValueOrDefault();
                    entityAutorizacion.rfc = rfc;
                    entityAutorizacion.promovente = promovente.Trim();
                    entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
                    entityAutorizacion.rfc_contribuyente = rfcContribuyente;
                    entityAutorizacion.contribuyente = contribuyente;
                    entityAutorizacion.despachos_autorizados = despachosAutorizados.Trim();
                    entityAutorizacion.id_autoridad_dirigida = idAutoridadDirigida;
                    entityAutorizacion.otra_autoridad_dirigida = autoridadDirigida;
                    entityAutorizacion.domicilio_promovente = domicilioPromovente;
                    entityAutorizacion.domicilio_notificaciones = domicilioNotificaciones;
                    entityAutorizacion.fecha_presentacion = fechaPresentacion;
                    entityAutorizacion.id_fundamento_solicitud = idFundamentoSolicitud;
                    entityAutorizacion.otro_fundamento_solicitud = fundamentoSolicitud;
                    entityAutorizacion.id_tema = idTema;
                    entityAutorizacion.otro_tema = otro_tema;
                    entityAutorizacion.no_indica_monto = noIndidaMonto;
                    entityAutorizacion.monto = monto;
                    entityAutorizacion.fecha_recepcion = fechaRecepcion;
                    entityAutorizacion.id_unidad_administrativa = idUnidadAdministrativa;
                    entityAutorizacion.usuario_modificacion = usuarioModificacion;
                    break;
                case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    entityAutorizacion.rfc = rfc;
                    entityAutorizacion.promovente = promovente;
                    entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
                    entityAutorizacion.rfc_contribuyente = rfcContribuyente;
                    entityAutorizacion.contribuyente = contribuyente;
                    entityAutorizacion.domicilio_promovente = domicilioPromovente;
                    entityAutorizacion.fecha_recepcion = fechaRecepcion;
                    entityAutorizacion.id_unidad_administrativa = idUnidadAdministrativa;
                    entityAutorizacion.usuario_modificacion = usuarioModificacion;
                    break;
                case EnumTipoAsuntoConst.DONATARIAS:
                    entityAutorizacion.rfc = rfc;
                    entityAutorizacion.promovente = promovente;
                    entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
                    entityAutorizacion.rfc_contribuyente = rfcContribuyente;
                    entityAutorizacion.contribuyente = contribuyente;
                    entityAutorizacion.domicilio_promovente = domicilioPromovente;
                    entityAutorizacion.fecha_recepcion = fechaRecepcion;
                    entityAutorizacion.id_unidad_administrativa = idUnidadAdministrativa;
                    entityAutorizacion.usuario_modificacion = usuarioModificacion;
                    break;
                default:
                    break;
            }
        }

        public static void UpdateGuardarModalidadLinea(ref Autorizacion entityAutorizacion,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string? domicilioNotificaciones,
            int? idFundamentoSolicitud,
            string? fundamentoSolicitud,
            DateTime fechaRecepcion,
            int? idUnidadAdministrativa,
            string usuarioModificacion
        )
        {
            ValidacionEstadosOficialPartesEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.BooleanValue(promoventeNoContribuyente, "Promovente no es contribuyente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.CatalogValue(ref idUnidadAdministrativa, "Unidad administrativa", false);

            

            switch (entityAutorizacion.id_tipo_asunto)
            {
                case EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR:
                    Guard.ValidateStringAlphanumeric(ref domicilioNotificaciones, "Domicilio notificaciones", false);
                    Guard.CatalogOtherValue(ref idFundamentoSolicitud, ref fundamentoSolicitud, "Fundamento solicitud", false);
                    entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
                    entityAutorizacion.rfc_contribuyente = rfcContribuyente;
                    entityAutorizacion.contribuyente = contribuyente;
                    entityAutorizacion.domicilio_notificaciones = domicilioNotificaciones;
                    entityAutorizacion.id_fundamento_solicitud = idFundamentoSolicitud;
                    entityAutorizacion.otro_fundamento_solicitud = fundamentoSolicitud;
                    entityAutorizacion.fecha_recepcion = fechaRecepcion;
                    entityAutorizacion.id_unidad_administrativa = idUnidadAdministrativa;
                    entityAutorizacion.usuario_modificacion = usuarioModificacion;
                    break;
                case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                    entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
                    entityAutorizacion.rfc_contribuyente = rfcContribuyente;
                    entityAutorizacion.contribuyente = contribuyente;
                    entityAutorizacion.fecha_recepcion = fechaRecepcion;
                    entityAutorizacion.id_unidad_administrativa = idUnidadAdministrativa;
                    entityAutorizacion.usuario_modificacion = usuarioModificacion;
                    break;
                case EnumTipoAsuntoConst.DONATARIAS:
                    entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
                    entityAutorizacion.rfc_contribuyente = rfcContribuyente;
                    entityAutorizacion.contribuyente = contribuyente;
                    entityAutorizacion.fecha_recepcion = fechaRecepcion;
                    entityAutorizacion.id_unidad_administrativa = idUnidadAdministrativa;
                    entityAutorizacion.usuario_modificacion = usuarioModificacion;
                    break;
                default:
                    break;
            }
        }

        public static void UpdateTurnar(ref Autorizacion entityAutorizacion,
           string noEmpleado,
           string usuarioMidificacion
        )
        {
            ValidacionEstadosOficialPartesEvents.ValidateAutorizacionTurnar(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            if (entityAutorizacion.id_unidad_administrativa is null || entityAutorizacion.id_unidad_administrativa <= 0)
                throw new Exception("Aún no se ha asignado la unidad administrativa.");

            entityAutorizacion.id_estado_tarea = EnumEstadoTareaCons.Pendiente_de_asignar;
            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
            entityAutorizacion.no_empleado_turna = noEmpleado;
            entityAutorizacion.usuario_modificacion = usuarioMidificacion;
        }

        public static void UpdateDelete(ref Autorizacion entityAutorizacion,
           string usuarioMidificacion
        )
        {
            ValidacionEstadosOficialPartesEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entityAutorizacion.usuario_modificacion = usuarioMidificacion;
        }

    }
}
