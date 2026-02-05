using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.AdministradorGlobal
{
    public class ValidacionEstadosAdministradorGlobalEvents
    {
        public static void ValidateDescartarDatosGenerales(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new List<int>()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Aviso_Pendiente.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                    EnumEstadoProcesal.En_Reparacion.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new List<int>()
                {
                    EnumEstadoTarea.Asignado.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }

        public static void ValidateDescartarCumplimentacionPorImprocedencia(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new List<int>()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Aviso_Pendiente.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                    EnumEstadoProcesal.En_Reparacion.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new List<int>()
                {
                    EnumEstadoTarea.Asignado.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }

        public static void ValidateDescartarNoAsunto(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new List<int>()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Aviso_Pendiente.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                    EnumEstadoProcesal.En_Reparacion.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new List<int>()
                {
                    EnumEstadoTarea.Asignado.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }
    }
}
