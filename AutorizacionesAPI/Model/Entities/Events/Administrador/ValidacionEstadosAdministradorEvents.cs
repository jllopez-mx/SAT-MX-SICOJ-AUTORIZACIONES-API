using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.Administrador
{
    public class ValidacionEstadosAdministradorEvents
    {
        #region Autorización        
        public static void ValidateAutorizacionUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                if (!idEstadoProcesal.Equals(EnumEstadoProcesalCons.En_estudio))
                    throw new Exception("Para modificar se requiere el estado procesal: En estudio");
            }

            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new List<int>()
                {
                    EnumEstadoTarea.Pendiente_de_asignar.GetHashCode(),
                    EnumEstadoTarea.Asignado.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }

        public static void ValidateAutorizacionAsignar(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                if (!idEstadoProcesal.Equals(EnumEstadoProcesalCons.En_estudio))
                    throw new Exception("Para modificar se requiere el estado procesal: En estudio");
            }

            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new List<int>()
                {
                    EnumEstadoTarea.Pendiente_de_asignar.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }

        public static void ValidateImprocedencia(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                if (!idEstadoProcesal.Equals(EnumEstadoProcesalCons.En_Reparacion))
                    throw new Exception("Para modificar se requiere el estado procesal: En reparación");
            }

            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new List<int>()
                {
                    EnumEstadoTarea.Pendiente_de_asignar.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }
        #endregion

        #region Remisión
        public static void ValidateRemision(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new List<int>()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Activo.GetHashCode(),
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
                    EnumEstadoTarea.Pendiente_de_asignar.GetHashCode(),
                    EnumEstadoTarea.Asignado.GetHashCode(),
                    EnumEstadoTarea.Reasingado.GetHashCode(),
                };

                if (!listaEstadoTarea.Any(c => c.Equals(idEstadoTarea)))
                {
                    throw new Exception("Estado de tarea no válido.");
                }
            }
        }
        #endregion
    }
}
