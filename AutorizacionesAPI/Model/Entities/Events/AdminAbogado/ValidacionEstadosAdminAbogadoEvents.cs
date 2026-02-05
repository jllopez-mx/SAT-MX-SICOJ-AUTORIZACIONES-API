using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ValidacionEstadosAdminAbogadoEvents
    {
        #region Autorización        
        public static void ValidateAutorizacionUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                    EnumEstadoProcesal.Aviso_Pendiente.GetHashCode(),
                    EnumEstadoProcesal.En_Reparacion.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        #endregion

        #region Personas Autorizadas
        public static void ValidatePersonasAutorizadasCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(), 
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidatePersonasAutorizadasUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        #endregion

        #region Solicitud de Opinión e Información
        public static void ValidateSolicitudOpinionInformacionNo(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateSolicitudOpinionInformacionCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateSolicitudOpinionInformacionUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        #endregion

        #region Avisos Comunicados
        public static void ValidateAvisosComunicadosCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateAvisosComunicadosUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        #endregion

        #region Requerimiento
        public static void ValidateRequerimientoNo(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateRequerimientoCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateRequerimientoUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.En_Reparacion.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        public static void ValidateRequerimientoDescartarSeccion(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateRequerimientoDescartarUltimo(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        #endregion

        #region Requerimiento PRODECON
        public static void ValidateRequerimientoProdeconCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateRequerimientoProdeconUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        #endregion

        #region Aviso sin Respuesta
        public static void ValidateAvisoSinRespuestaCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateAvisosSinRespuestaAprobar(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.Aviso_Pendiente.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateAvisosSinRespuestaRechazar(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.Aviso_Pendiente.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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
        #endregion

        #region Resolución
        public static void ValidateResolucionCreate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateResolucionCreateCumplimentacion(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new()
                {
                    EnumEstadoProcesal.En_estudio.GetHashCode(),
                    EnumEstadoProcesal.Requerido.GetHashCode(),
                    EnumEstadoProcesal.Concluido_Notificado.GetHashCode(),
                    EnumEstadoProcesal.Resolucion_Notificada.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new()
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

        public static void ValidateResolucionUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
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
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateResolucionConcluir(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateResolucionDescartar(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new ()
                {
                    EnumEstadoProcesal.Resuelto.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new ()
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

        public static void ValidateResolucionCumplimentacionUpdate(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new()
                {
                    EnumEstadoProcesal.Resolucion_Emitida.GetHashCode(),
                    EnumEstadoProcesal.Resolucion_Notificada.GetHashCode(),
                    EnumEstadoProcesal.En_Reparacion.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new()
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

        public static void ValidateResolucionCumplimentacionConcluir(bool validateEstadoProcesal = true, int? idEstadoProcesal = null, bool validateEstadoTarea = true, int? idEstadoTarea = null)
        {
            if (validateEstadoProcesal)
            {
                List<int> listaEstadoProcesal = new()
                {
                    EnumEstadoProcesal.Resolucion_Notificada.GetHashCode(),
                };

                if (!listaEstadoProcesal.Any(c => c.Equals(idEstadoProcesal)))
                {
                    throw new Exception("Estado procesal no válido.");
                }
            }
            if (validateEstadoTarea)
            {
                List<int> listaEstadoTarea = new()
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
        #endregion
    }
}
