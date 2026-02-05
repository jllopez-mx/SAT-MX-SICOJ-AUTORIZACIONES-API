using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ImpuestosInternosSolicitudOpinionInformacionEvents
    {
        public static SolicitudOpinionInformacion CreateImpuestosInternosSolicitudOpinionInformacion(
           Autorizacion entityAutorizacion,
           int? idUnidadAdministrativa,
           string? noOficioSolicitud,
           DateTime fechaOficioSolicitud,
           bool unidadInterna,
           string? unidadAdministrativaExterna,
           string usuario_creacion
           )
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateSolicitudOpinionInformacionCreate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            Guard.ValidateStringAlphanumeric(ref noOficioSolicitud!, "Número de oficio de solicitud");

            SolicitudOpinionInformacion entity = new()
            {
                idAutorizacion = entityAutorizacion.id,
                idUnidadAdministrativa = idUnidadAdministrativa,
                noOficioSolicitud = noOficioSolicitud,
                fechaOficioSolicitud = fechaOficioSolicitud,
                unidadInterna = unidadInterna,
                unidadAdministrativaExterna = unidadAdministrativaExterna,
                usuario_creacion = usuario_creacion!
            };
            return entity;
        }

        public static void UpdateImpuestosInternosSolicitudOpinonInformacion(ref SolicitudOpinionInformacion entity,
            Autorizacion entityAutorizacion,
            int? idUnidadAdministrativa,
            string? noOficioSolicitud,
            DateTime fechaOficioSolicitud,
            bool? atendioSolicitud,
            string? noOficioRespuesta,
            DateTime? fechaOficioRespuesta,
            DateTime? fechaRecepcion,
            bool unidadInterna,
            string? unidadAdministrativaExterna,
            string usuario_modificacion
            )
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateSolicitudOpinionInformacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            Guard.ValidateStringAlphanumeric(ref noOficioSolicitud!, "Número de oficio de solicitud");
            Guard.ValidateStringNumero(ref noOficioRespuesta!, "Número de oficio de respuesta", false);

            entity.idUnidadAdministrativa = idUnidadAdministrativa;
            entity.noOficioSolicitud = noOficioSolicitud;
            entity.fechaOficioSolicitud = fechaOficioSolicitud;
            entity.atendioSolicitud = atendioSolicitud;
            entity.noOficioRespuesta = noOficioRespuesta;
            entity.fechaOficioRespuesta = fechaOficioRespuesta;
            entity.fechaRecepcion = fechaRecepcion;
            entity.unidadInterna = unidadInterna;
            entity.unidadAdministrativaExterna = unidadAdministrativaExterna;
            entity.usuario_modificacion = usuario_modificacion!;
            entityAutorizacion.usuario_modificacion = usuario_modificacion!;
        }

        public static void NoSolicitudOpinionInformacion(ref Autorizacion entityAutorizacion,
            string? usuario_creacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateSolicitudOpinionInformacionNo(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entityAutorizacion.usuario_modificacion = usuario_creacion;
            entityAutorizacion.solicitud_opinion = false;
        }

    }
}
