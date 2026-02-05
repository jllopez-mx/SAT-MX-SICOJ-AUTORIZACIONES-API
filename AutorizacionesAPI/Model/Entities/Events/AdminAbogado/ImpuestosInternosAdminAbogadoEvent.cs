using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ImpuestosInternosAdminAbogadoEvent
    {
        public static void UpdateModalidadFisico(ref Autorizacion entityAutorizacion,
            ref AvisoSinRespuesta avisoSinRespuesta,
            ref AvisoConRespuesta avisoConRespuesta,
            Modificacion modificacion,
            Descartar descartar,            
            string? rfc,
            string promovente,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string? despachos_autorizados,
            string domicilioPromovente,
            string? domicilio_notificaciones,
            DateTime fechaPresentacion,
            DateTime fechaRecepcion,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateStringAlphanumeric(ref promovente!, "Promovente");
            Guard.BooleanValue(promoventeNoContribuyente, "Promovente no es contribuyente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringEmpty(ref domicilioPromovente, "Domicilio promovente");
            Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
            Guard.BooleanValue(noIndidaMonto, "No lo indica");
            Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());

            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            entityAutorizacion.rfc = rfc;
            entityAutorizacion.promovente = promovente;
            entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
            entityAutorizacion.rfc_contribuyente = rfcContribuyente;
            entityAutorizacion.contribuyente = contribuyente;
            entityAutorizacion.despachos_autorizados = despachos_autorizados;
            entityAutorizacion.domicilio_notificaciones = domicilio_notificaciones;
            entityAutorizacion.domicilio_promovente = domicilioPromovente;
            entityAutorizacion.fecha_presentacion = fechaPresentacion;
            entityAutorizacion.fecha_recepcion = fechaRecepcion;
            entityAutorizacion.id_tema = idTema;
            entityAutorizacion.otro_tema = otro_tema;
            entityAutorizacion.no_indica_monto = noIndidaMonto;
            entityAutorizacion.monto = monto;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;

            if (entityAutorizacion.id_estado_procesal.Equals(EnumEstadoProcesalCons.En_Reparacion))
            {
                if (modificacion != null)
                {
                    entityAutorizacion.id_estado_procesal = modificacion.id_estado_procesal;
                }
                else if (descartar != null)
                {
                    //entityAutorizacion.id_estado_procesal = descartar.id_estado_procesal;
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
                }
                else if (modificacion == null)
                {
                    throw new Exception("No existe el registro de modificación");
                }
                else if (descartar == null)
                {
                    throw new Exception("No existe el registro de descarte");
                }
            }
        }


        public static void UpdateModalidadLinea(ref Autorizacion entityAutorizacion,
            Modificacion modificacion,
            Descartar descartar,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string? despachos_autorizados,
            string? domicilio_notificaciones,
            DateTime fechaPresentacion,
            DateTime fechaRecepcion,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.BooleanValue(promoventeNoContribuyente, "Promovente no es contribuyente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
            Guard.BooleanValue(noIndidaMonto, "No lo indica");
            Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());

            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
            entityAutorizacion.rfc_contribuyente = rfcContribuyente;
            entityAutorizacion.contribuyente = contribuyente;
            entityAutorizacion.despachos_autorizados = despachos_autorizados;
            entityAutorizacion.domicilio_notificaciones = domicilio_notificaciones;
            entityAutorizacion.fecha_presentacion = fechaPresentacion;
            entityAutorizacion.fecha_recepcion = fechaRecepcion;
            entityAutorizacion.id_tema = idTema;
            entityAutorizacion.otro_tema = otro_tema;
            entityAutorizacion.no_indica_monto = noIndidaMonto;
            entityAutorizacion.monto = monto;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;

            if (entityAutorizacion.id_estado_procesal.Equals(EnumEstadoProcesalCons.En_Reparacion))
            {
                if (modificacion != null)
                {
                    entityAutorizacion.id_estado_procesal = modificacion.id_estado_procesal;
                }
                else if (descartar != null)
                {
                    //entityAutorizacion.id_estado_procesal = descartar.id_estado_procesal;
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
                }
                else if (modificacion == null)
                {
                    throw new Exception("No existe el registro de modificación");
                }
                else if (descartar == null)
                {
                    throw new Exception("No existe el registro de descarte");
                }
            }
        }
    }
}
