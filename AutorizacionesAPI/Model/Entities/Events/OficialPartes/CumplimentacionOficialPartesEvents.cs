using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.Entities.Events.OficialPartes
{
    public class CumplimentacionOficialPartesEvents
    {
        public static Cumplimentacion Create(
            ref Autorizacion? autorizacion,
            string noAsuntoExterno,
            int idUnidadAdministrativaCentral,
            string usuarioCreacion
        )
        {
            if (autorizacion is null)
            {
                autorizacion = new()
                {
                    no_asunto = noAsuntoExterno,
                    externo_cumplimentacion = true,
                };
            }
            else
            {
                if (!autorizacion.id_estado_procesal.Equals(EnumEstadoProcesalCons.Concluido_Notificado) &&
                !autorizacion.id_estado_procesal.Equals(EnumEstadoProcesalCons.Resuelto))
                {
                    throw new Exception($"No se puede cumplimentar el número de asunto {autorizacion.no_asunto} debido a que el estado procesal es diferente a “Concluido notificado” y “Resuelto notificado”.");
                }
            }

            Cumplimentacion entity = new()
            {
                id_autorizacion = autorizacion is null ? 0: autorizacion.id!,
                id_unidad_administrativa_central = idUnidadAdministrativaCentral,
                id_tipo_modalidad = EnumTipoModalidad.FÍSICO,
                id_tipo_asunto = EnumTipoAsuntoConst.CUMPLIMENTACION,
                id_estado_tarea = EnumEstadoTareaCons.Pendiente_de_turnar,
                id_estado_procesal = EnumEstadoProcesalCons.Activo,
                usuario_creacion = usuarioCreacion
            };
            return entity;
        }

        
        public static void UpdateCumplimentacion(
            ref Autorizacion entityAutorizacion,
            ref Cumplimentacion entityCumplimentacion,
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
           int? id_unidad_administrativa,
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
            entityCumplimentacion.id_unidad_administrativa = id_unidad_administrativa;
            entityCumplimentacion.usuario_modificacion = usuarioModificacion;
        }

        public static void UpdateTurnarCumplimentacion(ref Cumplimentacion entityAutorizacion,
           string noEmpleado,
           string usuarioMidificacion
        )
        {
            ValidacionEstadosOficialPartesEvents.ValidateAutorizacionTurnar(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            entityAutorizacion.id_estado_tarea = EnumEstadoTareaCons.Pendiente_de_asignar;
            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
            entityAutorizacion.no_empleado_turna = noEmpleado;
            entityAutorizacion.usuario_modificacion = usuarioMidificacion;
        }
    }
}
