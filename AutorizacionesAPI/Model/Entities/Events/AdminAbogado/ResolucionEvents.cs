using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ResolucionEvents
    {
        public static Resolucion Create(
            ref Autorizacion entityAutorizacion,
            ref Cumplimentacion entityCumplimentacion,
            Descartar entityDescarte,
            Resolucion entityResolucionAnterior,
            string? p_oficio_resolucion,
            DateTime p_fecha_oficio,
            int? p_id_sentido,
            DateTime? fechaVencimiento,
            string usuario_creacion)
        {
            if (entityAutorizacion is null && entityCumplimentacion is null)
                throw new Exception("Debe de indicar si el asunto es una autorización o cumplimentación");
            else if (entityCumplimentacion is not null && entityAutorizacion is not null)
                throw new Exception("Solo debe de indicar autorización o cumplimentación");

            if (entityDescarte is not null)
            {
                if (entityDescarte.id_seccion != EnumSeccionesCons.EMISION_RESOLUCION)
                {
                    if (entityResolucionAnterior is null)
                        throw new Exception("No existe una emisión de la resolución descartada.");
                }
            }

            var entity = new Resolucion
            {
                id_autorizacion = entityAutorizacion is null ? null! : entityAutorizacion.id,
                id_cumplimentacion = entityCumplimentacion is null ? null! : entityCumplimentacion.id,
                fecha_vencimiento = entityAutorizacion is null ? entityCumplimentacion!.fecha_vencimiento : entityAutorizacion.fecha_vencimiento!,
                oficio_resolucion = p_oficio_resolucion!,
                fecha_oficio = p_fecha_oficio,
                id_sentido = p_id_sentido.GetValueOrDefault(),
                usuario_creacion = usuario_creacion!
            };

            if (entityAutorizacion is not null)
            {
                ValidacionEstadosAdminAbogadoEvents.ValidateResolucionCreate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
                entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Resuelto;
                entityAutorizacion.usuario_modificacion = usuario_creacion!;

                if (fechaVencimiento is not null)
                {
                    entity.fecha_vencimiento = fechaVencimiento;
                }
                else
                {
                    entity.fecha_vencimiento = entityAutorizacion.fecha_vencimiento!;
                }
            }
            else if (entityCumplimentacion is not null)
            {
                ValidacionEstadosAdminAbogadoEvents.ValidateResolucionCreateCumplimentacion(true, entityCumplimentacion.id_estado_procesal, true, entityCumplimentacion.id_estado_tarea);
                entityCumplimentacion.id_estado_procesal = EnumEstadoProcesalCons.Resolucion_Emitida;
                entityCumplimentacion.usuario_modificacion = usuario_creacion!;

                if (fechaVencimiento is not null)
                {
                    entity.fecha_vencimiento = fechaVencimiento;
                }
                else
                {
                    entity.fecha_vencimiento = entityCumplimentacion.fecha_vencimiento!;
                }
            }

            return entity;
        }

        public static void Update(
            ref Autorizacion entityAutorizacion,
            ref Cumplimentacion entityCumplimentacion,
            ref Resolucion entity,
            Modificacion modificacion,
            string? p_oficio_resolucion,
            DateTime p_fecha_oficio,
            int? p_id_sentido,
            DateTime p_fecha_notificacion,
            string usuario_modificacion,
            bool primerResolucion = true,
            DateTime? fechaVencimiento = null)
        {

            if (entityAutorizacion is null && entityCumplimentacion is null)
                throw new Exception("Debe de indicar si el asunto es una autorización o cumplimentación");
            else if (entityCumplimentacion is not null && entityAutorizacion is not null)
                throw new Exception("Solo debe de indicar autorización o cumplimentación");

            if (entityAutorizacion is not null)
            {
                ValidacionEstadosAdminAbogadoEvents.ValidateResolucionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
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
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Resuelto;
                }

                entityAutorizacion.usuario_modificacion = usuario_modificacion!;
                if (fechaVencimiento is not null)
                {
                    entity.fecha_vencimiento = fechaVencimiento;
                }
                else
                {
                    entity.fecha_vencimiento = entityAutorizacion.fecha_vencimiento!;
                }
            }
            else if (entityCumplimentacion is not null)
            {
                ValidacionEstadosAdminAbogadoEvents.ValidateResolucionCumplimentacionUpdate(true, entityCumplimentacion.id_estado_procesal, true, entityCumplimentacion.id_estado_tarea);
                if (entityCumplimentacion.id_estado_procesal.Equals(EnumEstadoProcesalCons.En_Reparacion))
                {
                    if (modificacion is null)
                    {
                        throw new Exception("No existe el registro de modificación");
                    }
                    entityCumplimentacion.id_estado_procesal = modificacion.id_estado_procesal;
                }
                else
                {
                    entityCumplimentacion.id_estado_procesal = EnumEstadoProcesalCons.Resolucion_Notificada;
                }

                entityCumplimentacion.usuario_modificacion = usuario_modificacion!;
                if (primerResolucion)
                {
                    entity.fecha_vencimiento = entityCumplimentacion!.fecha_vencimiento;
                }
                else
                {
                    if (fechaVencimiento is not null)
                    {
                        entity.fecha_vencimiento = fechaVencimiento;
                    }
                    else
                    {
                        entity.fecha_vencimiento = entityCumplimentacion.fecha_vencimiento!;
                    }
                }
            }
             
            entity.oficio_resolucion = p_oficio_resolucion!;
            entity.fecha_oficio = p_fecha_oficio;
            entity.id_sentido = p_id_sentido.GetValueOrDefault();
            entity.fecha_notificacion = p_fecha_notificacion;
            entity.usuario_modificacion = usuario_modificacion!;            
        }

        public static void Concluir(
            ref Autorizacion entityAutorizacion,
            ref Cumplimentacion entityCumplimentacion,
            AvisoConRespuesta avisoConRespuesta,
            Resolucion entity,
            string usuario_modificacion)
        {
            if (entityAutorizacion is not null)
            {
                ValidacionEstadosAdminAbogadoEvents.ValidateResolucionConcluir(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
                if (avisoConRespuesta is not null)
                {
                    entityAutorizacion.id_estado_tarea = EnumEstadoTareaCons.Atendido;
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Concluido_Notificado;
                }
                else
                {
                    entityAutorizacion.id_estado_tarea = EnumEstadoTareaCons.Atendido;
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Concluido_Notificado;
                }

                entityAutorizacion.usuario_modificacion = usuario_modificacion!;
            }
            else if (entityCumplimentacion is not null)
            {
                ValidacionEstadosAdminAbogadoEvents.ValidateResolucionCumplimentacionConcluir(true, entityCumplimentacion.id_estado_procesal, true, entityCumplimentacion.id_estado_tarea);
                entityCumplimentacion.id_estado_tarea = EnumEstadoTareaCons.Atendido;
                entityCumplimentacion.id_estado_procesal = EnumEstadoProcesalCons.Concluido_Notificado;
            }            
        }

        public static void Descartar(ref Autorizacion entityAutorizacion,
          ref Resolucion entity,
          string usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateResolucionDescartar(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entity.usuario_modificacion = usuario_modificacion!;

            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
            entityAutorizacion.usuario_modificacion = usuario_modificacion!;
        }
    }
}
