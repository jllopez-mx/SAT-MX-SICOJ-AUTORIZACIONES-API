using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ComercioExteriorRequerimientoEvents
    {
        public static void NoRequerimiento(ref Autorizacion entityAutorizacion,
           string? usuario_creacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoNo(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entityAutorizacion.usuario_modificacion = usuario_creacion;
            entityAutorizacion.requerimiento = false;
        }

        public static Requerimiento Create(ref Autorizacion entityAutorizacion,
           string? oficio_requerimiento,
           DateTime fecha_oficio,
           string? usuario_creacion)
        {            
            ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoCreate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.ValidateStringNumero(ref oficio_requerimiento!, "Oficio Requerimiento");

            var entity = new Requerimiento
            {
                id_autorizacion = entityAutorizacion.id,
                oficio_requerimiento = oficio_requerimiento!,
                fecha_oficio = fecha_oficio!,
                usuario_creacion = usuario_creacion!
            };

            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Requerido;

            return entity;
        }

        public static void Update(ref Autorizacion entityAutorizacion,
           ref Requerimiento entity,
           Modificacion modificacion,
           string? oficio_requerimiento,
           DateTime fecha_oficio,
           DateTime fecha_notificacion,
           bool? atendioRequerimiento,
           DateTime? fechaAtencion,
           string? usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            entity.oficio_requerimiento = oficio_requerimiento!;
            entity.fecha_oficio = fecha_oficio;
            entity.fecha_notificacion = fecha_notificacion;
            entity.atendio_requerimiento = atendioRequerimiento;
            entity.fecha_atencion = fechaAtencion;
            entity.usuario_modificacion = usuario_modificacion!;

            if (entityAutorizacion.id_estado_procesal.Equals(EnumEstadoProcesalCons.En_Reparacion))
            {
                if (modificacion is null)
                {
                    throw new Exception("No existe el registro de modificación");
                }
                entityAutorizacion.id_estado_procesal = modificacion.id_estado_procesal;
            }
            else
            {
                if (atendioRequerimiento is not null)
                {
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
                }
                else
                {
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Requerido;
                }
            }

            entityAutorizacion.usuario_modificacion = usuario_modificacion!;
        }

        public static void DescartarSeccion(ref Autorizacion entityAutorizacion,
           string? usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoDescartarSeccion(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
            entityAutorizacion.usuario_modificacion = usuario_modificacion!;
        }

        public static void DescartarUltimo(ref Autorizacion entityAutorizacion,
           ref Requerimiento entity,
           string? usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoDescartarUltimo(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entityAutorizacion.usuario_modificacion = usuario_modificacion!;
        }
    }
}
