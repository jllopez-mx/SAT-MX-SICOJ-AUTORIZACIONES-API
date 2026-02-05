using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.AdministradorGlobal
{
    public class ComercioExteriorAdministradorGlobalEvents
    {
        public static void Reactivar(
            ref Autorizacion autorizacion,
            ref Cumplimentacion cumplimentacion,
            string usuarioMidificacion
        )
        {
            if (autorizacion is not null)
            {
                if (autorizacion.id_estado_procesal != EnumEstadoProcesalCons.Concluido_Notificado && autorizacion.id_estado_procesal != EnumEstadoProcesalCons.Concluido_Remitido)
                    throw new Exception("La autorización no se puede reactivar debido a que no tiene en el estado procesal: Concluido Notificado o Concluido Remitido.");

                if (!autorizacion.id_estado_tarea.Equals(EnumEstadoTareaCons.Reasingado))
                    autorizacion.id_estado_tarea = EnumEstadoTareaCons.Asignado;

                autorizacion.id_estado_procesal = EnumEstadoProcesalCons.Resuelto;
                autorizacion.usuario_modificacion = usuarioMidificacion;
            }
            else if (cumplimentacion is not null)
            {
                //if (cumplimentacion.id_estado_procesal != EnumEstadoProcesalCons.Concluido_Notificado)
                //    throw new Exception("La cumplimentación no se puede reactivar debido a que no tiene en el estado procesal: Concluido Notificado.");

                if (!cumplimentacion.id_estado_tarea.Equals(EnumEstadoTareaCons.Reasingado))
                    cumplimentacion.id_estado_tarea = EnumEstadoTareaCons.Asignado;

                cumplimentacion.id_estado_procesal = EnumEstadoProcesalCons.Resuelto;
                cumplimentacion.usuario_modificacion = usuarioMidificacion;
            }
            
        }

        public static void Descartar(
            ref Autorizacion autorizacion,
            ref Cumplimentacion cumplimentacion,
            string? usuario_modificacion
        )
        {
            if (autorizacion is not null)
            {
                ValidacionEstadosAdministradorGlobalEvents.ValidateDescartarDatosGenerales(true, autorizacion.id_estado_procesal, true, autorizacion.id_estado_tarea);
                autorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_Reparacion;
                autorizacion.fecha_control_solicitudes = DateTime.Now;
                autorizacion.usuario_modificacion = usuario_modificacion!;
            }
            else if (cumplimentacion is not null)
            {
                ValidacionEstadosAdministradorGlobalEvents.ValidateDescartarDatosGenerales(true, cumplimentacion.id_estado_procesal, true, cumplimentacion.id_estado_tarea);
                cumplimentacion.id_estado_procesal = EnumEstadoProcesalCons.En_Reparacion;
                cumplimentacion.fecha_control_solicitudes = DateTime.Now;
                cumplimentacion.usuario_modificacion = usuario_modificacion!;
            }
        }
    }
}

