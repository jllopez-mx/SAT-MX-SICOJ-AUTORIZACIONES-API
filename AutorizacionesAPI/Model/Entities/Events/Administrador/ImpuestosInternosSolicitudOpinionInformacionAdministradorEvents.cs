using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.Administrador
{
    public class ImpuestosInternosSolicitudOpinionInformacionAdministradorEvents
    {
        public static SolicitudOpinionInformacion CreateImpuestosInternosSolicitudOpinionInformacion(
           int idAutorizacion,
           int? idUnidadAdministrativa,
           string? noOficioSolicitud,
           DateTime fechaOficioSolicitud,
           bool unidadInterna,
           string? unidadAdministrativaExterna,
           string usuario_creacion
           )
        {
            Guard.ValidateStringAlphanumeric(ref noOficioSolicitud!, "Número de oficio de solicitud");

            SolicitudOpinionInformacion entity = new()
            {
                idAutorizacion = idAutorizacion,
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
            Autorizacion entityImpuestos,
            int idAutorizacion,
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
            Guard.ValidateStringAlphanumeric(ref noOficioSolicitud!, "Número de oficio de solicitud");
            Guard.ValidateStringNumero(ref noOficioRespuesta!, "Número de oficio de respuesta", false);

            entity.idAutorizacion = idAutorizacion;
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
            entityImpuestos.usuario_modificacion = usuario_modificacion!;

        }

        public static void NoSolicitudOpinionInformacion(ref Autorizacion entityAutorizaciones,
            string? usuario_creacion)
        {
            entityAutorizaciones.usuario_modificacion = usuario_creacion;
            entityAutorizaciones.solicitud_opinion = false;
        }

    }
}
