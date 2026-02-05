using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.AdministradorGlobal
{
    public class CumplimentacionAdminstradorGlobalEvents
    {
        public static void DescartarPorImprocedencia(
            ref Cumplimentacion cumplimentacion,
            string? usuario_modificacion
        )
        {
            ValidacionEstadosAdministradorGlobalEvents.ValidateDescartarCumplimentacionPorImprocedencia(true, cumplimentacion.id_estado_procesal, true, cumplimentacion.id_estado_tarea);
            cumplimentacion.id_estado_procesal = EnumEstadoProcesalCons.En_Reparacion;
            cumplimentacion.fecha_control_solicitudes = DateTime.Now;
            cumplimentacion.usuario_modificacion = usuario_modificacion!;
        }

        public static void DescartarNoAsunto(
            ref Cumplimentacion cumplimentacion,
            string? usuario_modificacion
        )
        {
            ValidacionEstadosAdministradorGlobalEvents.ValidateDescartarCumplimentacionPorImprocedencia(true, cumplimentacion.id_estado_procesal, true, cumplimentacion.id_estado_tarea);
            cumplimentacion.id_estado_procesal = EnumEstadoProcesalCons.En_Reparacion;
            cumplimentacion.fecha_control_solicitudes = DateTime.Now;
            cumplimentacion.usuario_modificacion = usuario_modificacion!;
        }
    }
}
