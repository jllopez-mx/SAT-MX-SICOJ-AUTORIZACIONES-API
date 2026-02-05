using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.OficialPartes
{
    public class ValidacionEstadosOficialPartesEvents
    {
        public static void ValidateAutorizacionUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                if (!idEstadoProcesal.Equals(EnumEstadoProcesalCons.Activo))
                    throw new Exception("Para modificar se requiere el estado procesal: Activo");
            }
            if (validateEstadoTarea)
            {
                if (!idEstadoTarea.Equals(EnumEstadoTareaCons.Pendiente_de_turnar))
                    throw new Exception("Para modificar se requiere el estado de tarea: Pendiente de turnar");
            }
        }

        public static void ValidateAutorizacionTurnar(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                if (!idEstadoProcesal.Equals(EnumEstadoProcesalCons.Activo) && !idEstadoProcesal.Equals(EnumEstadoProcesalCons.Remitido))
                    throw new Exception("Para modificar se requiere el estado procesal: Activo o Remitido");
            }
            if (validateEstadoTarea)
            {
                if (!idEstadoTarea.Equals(EnumEstadoTareaCons.Pendiente_de_turnar))
                    throw new Exception("Para modificar se requiere el estado de tarea: Pendiente de turnar");
            }
        }

        public static void ValidateCumplimentacionCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new List<int>()
                {
                    EnumEstadoProcesal.Concluido_Notificado.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("No se puede cumplimentar el número de asunto debido a que el estado procesal es diferente y no puede continuar el registro");
                }
            }

            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new List<int>()
                {
                    EnumEstadoTarea.Asignado.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                    EnumEstadoTarea.Atendido.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }
    }
}
