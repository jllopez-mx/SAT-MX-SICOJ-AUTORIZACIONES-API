using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class CumplimentaciónEvents
    {
        public static void UpdateCumplimentacion(ref Autorizacion entityAutorizacion, ref Cumplimentacion entityCumplimentacion,
           int? idTipoAsunto,
           string? rfc,
           string? promovente,
           bool? promoventeNoContribuyente,
           string? rfcContribuyente,
           string? contribuyente,
           string? no_juicio_recurso_amparo,
           DateTime? fecha_recepcion_solicitud,
           DateTime? fecha_firmeza,
           int? id_plazo_cumplimiento,
           DateTime? fechaVencimiento,
           int? id_organo_jurisdiccional,
           string? organo_jurisdiccional,
           int? id_unidad_administrativa_cumplimiento,
           string? unidad_administrativa_cumplimiento,
           string? oficio_resolucion_impugnada,
           DateTime? fecha_oficio_resolucion_impugnada,
           string usuarioModificacion
        )
        {
            if (entityAutorizacion.externo_cumplimentacion)
            {
                entityAutorizacion.id_tipo_asunto = idTipoAsunto;
                entityAutorizacion.rfc = rfc;
                entityAutorizacion.promovente = promovente;
                entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
                entityAutorizacion.rfc_contribuyente = rfcContribuyente;
                entityAutorizacion.contribuyente = contribuyente;
            }

            entityCumplimentacion.no_juicio_recuro_amparo = no_juicio_recurso_amparo;
            entityCumplimentacion.fecha_recepcion_solicitud = fecha_recepcion_solicitud!;
            entityCumplimentacion.fecha_firmeza = fecha_firmeza;
            entityCumplimentacion.id_plazo_cumplimiento = id_plazo_cumplimiento;
            if (id_plazo_cumplimiento.Equals(3))
            {
                entityCumplimentacion.fecha_vencimiento = fechaVencimiento;
            }
            else
            {
                entityCumplimentacion.fecha_vencimiento = entityCumplimentacion.fecha_vencimiento!;
            }

            entityCumplimentacion.id_organo_jurisdiccional = id_organo_jurisdiccional;
            entityCumplimentacion.organo_jurisdiccional = organo_jurisdiccional;
            entityCumplimentacion.id_unidad_administrativa_cumplimiento = id_unidad_administrativa_cumplimiento;
            entityCumplimentacion.unidad_administrativa_cumplimiento = unidad_administrativa_cumplimiento;
            entityCumplimentacion.oficio_resolucion_impugnada = oficio_resolucion_impugnada;
            entityCumplimentacion.fecha_oficio_resolucion_impugnada = fecha_oficio_resolucion_impugnada;
            entityCumplimentacion.usuario_modificacion = usuarioModificacion;
        }
    }
}
