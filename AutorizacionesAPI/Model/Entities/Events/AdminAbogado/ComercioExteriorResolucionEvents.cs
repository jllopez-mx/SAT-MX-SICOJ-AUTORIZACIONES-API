using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ComercioExteriorResolucionEvents
    {
        public static Resolucion Create(ref Autorizacion entityAutorizacion,
          string? p_oficio_resolucion,
          DateTime p_fecha_oficio,
          int? p_id_sentido,
          string usuario_creacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateResolucionCreate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            var entity = new Resolucion
            {
                id_autorizacion = entityAutorizacion.id,
                fecha_vencimiento = entityAutorizacion.fecha_vencimiento!,
                oficio_resolucion = p_oficio_resolucion!,
                fecha_oficio = p_fecha_oficio,
                id_sentido = p_id_sentido.GetValueOrDefault(),
                usuario_creacion = usuario_creacion!
            };

            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Resuelto;
            entityAutorizacion.usuario_modificacion = usuario_creacion!;

            return entity;
        }

        public static void Update(ref Autorizacion entityAutorizacion,
          ref Resolucion entity,
          Modificacion modificacion,
          string? p_oficio_resolucion,
          DateTime p_fecha_oficio,
          int? p_id_sentido,
          DateTime p_fecha_notificacion,
          string usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateResolucionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            List<int> lista = new List<int>()
            {
                EnumEstadoProcesal.Resuelto.GetHashCode(),
                EnumEstadoProcesal.En_Reparacion.GetHashCode(),
            };
            int idEstadoProcesal = entityAutorizacion.id_estado_procesal;
            if (!lista.Any(c => c.Equals(idEstadoProcesal)))
            {
                throw new Exception("El estado procesal del asunto no es válido.");
            }

            entity.id_autorizacion = entityAutorizacion.id;
            entity.fecha_vencimiento = entityAutorizacion.fecha_vencimiento;
            entity.oficio_resolucion = p_oficio_resolucion!;
            entity.fecha_oficio = p_fecha_oficio;
            entity.id_sentido = p_id_sentido.GetValueOrDefault();
            entity.fecha_notificacion = p_fecha_notificacion;
            entity.usuario_modificacion = usuario_modificacion!;
            if (idEstadoProcesal.Equals(EnumEstadoProcesalCons.En_Reparacion))
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
        }

        public static void Concluir(ref Autorizacion entityAutorizacion,
          Resolucion entity,
          string usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateResolucionConcluir(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entityAutorizacion.id_estado_tarea = EnumEstadoTareaCons.Atendido;
            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Concluido_Notificado;
            entityAutorizacion.usuario_modificacion = usuario_modificacion!;
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
