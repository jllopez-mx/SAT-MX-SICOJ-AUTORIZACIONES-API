using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ImpuestosInternosAvisoSinRespuestaEvents
    {        
        public static void Update(ref Autorizacion entityAutorizacion,
            ref AvisoSinRespuesta entityAviso,
            ref List<string> folio,
            Descartar entityDescarte,
            string? observaciones,
            string? usuarioModificacion)
        {
            if (entityDescarte is null)
            {
                if (entityAutorizacion.id_tipo_asunto != EnumTipoAsunto.AUTORIZACION_DE_IMPUESTOS_INTERNOS.GetHashCode() && entityAutorizacion.id_tipo_asunto != EnumTipoAsunto.DONATARIAS.GetHashCode()
                    && entityAutorizacion.id_tipo_asunto != EnumTipoAsunto.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS.GetHashCode() && entityAutorizacion.id_tipo_asunto != EnumTipoAsunto.AVISO_SIN_RESPUESTA_DE_DONATARIAS.GetHashCode())
                {
                    throw new Exception("Solo se permiten aviso sin respuesta en autorizaciones de impuestos internos y donatarias.");
                }
            }

            entityAviso.id_tipo_aviso = entityAutorizacion.id_tema.GetValueOrDefault();
            entityAviso.fecha_ingreso = entityAutorizacion.fecha_presentacion;
            entityAviso.observaciones = observaciones;
            entityAviso.usuario_modificacion = usuarioModificacion;
            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Aviso_Pendiente;

            if (entityDescarte is null)
            {
                switch (entityAutorizacion.id_tipo_asunto)
                {
                    case EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS:
                        entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS;
                        break;
                    case EnumTipoAsuntoConst.DONATARIAS:
                        entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void Rechazar(ref Autorizacion entityAutorizacion,
            Descartar entityDescarte,
            string? usuario_modificacion)
        {
            switch (entityAutorizacion.id_tipo_asunto)
            {
                case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS:
                    entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS;
                    break;
                case EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS:
                    entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.DONATARIAS;
                    break;
                default:
                    throw new Exception("Tipo de asunto no válido.");
            }

            ValidacionEstadosAdminAbogadoEvents.ValidateAvisosSinRespuestaRechazar(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
            entityAutorizacion.usuario_modificacion = usuario_modificacion;
        }

        public static void Aprobar(ref Autorizacion entityAutorizacion,
            ref AvisoSinRespuesta entityAviso,
            string? usuario_modificacion)
        {
            if (entityAutorizacion.id_tipo_asunto != EnumTipoAsunto.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS.GetHashCode() && entityAutorizacion.id_tipo_asunto != EnumTipoAsunto.AVISO_SIN_RESPUESTA_DE_DONATARIAS.GetHashCode())
            {
                throw new Exception("Tipo de asunto no válido.");
            }

            ValidacionEstadosAdminAbogadoEvents.ValidateAvisosSinRespuestaAprobar(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            entityAviso.aprobado = true;
            entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Aviso_Concluido;
            entityAutorizacion.usuario_modificacion = usuario_modificacion;
        }
    }
}
