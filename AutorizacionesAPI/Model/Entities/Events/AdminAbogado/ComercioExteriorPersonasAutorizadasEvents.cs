using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ComercioExteriorPersonasAutorizadasEvents
    {
        public static PersonasAutorizadas CreatePersonasAutorizadas(Autorizacion entityAutorizacion,
           string? nombre,
           string? rfc,
           string? telefono,
           string? email,
           int? id_autorizacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidatePersonasAutorizadasCreate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            if (entityAutorizacion.id_tipo_modalidad != EnumTipoModalidad.LÍNEA.GetHashCode())
            {
                throw new Exception("La autorización no es de tipo en linea.");
            }

            Guard.ValidateStringAlphanumeric(ref nombre!, "Nombre");
            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateTelefono(ref telefono, "Telefono");
            Guard.ValidateEmail(ref email, "Email");
            Guard.CatalogValue(ref id_autorizacion, "Tipo de autorización");

            var entity = new PersonasAutorizadas
            {
                Nombre = nombre,
                Rfc = rfc,
                Telefono = telefono,
                Email = email,
                id_autorizacion = id_autorizacion.GetValueOrDefault(),
            };
            return entity;
        }

        public static void UpdatePersonasAutorizadas(ref PersonasAutorizadas entityPersonasAut,
          Autorizacion entityAutorizacion,
          string? nombre,
          string? rfc,
          string? telefono,
          string? email,
          int? id_autorizacion
        )
        {
            ValidacionEstadosAdminAbogadoEvents.ValidatePersonasAutorizadasUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            if (entityAutorizacion.id_tipo_modalidad != EnumTipoModalidad.LÍNEA.GetHashCode())
            {
                throw new Exception("La autorización no es de tipo en linea.");
            }

            Guard.ValidateStringAlphanumeric(ref nombre!, "Nombre");
            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateTelefono(ref telefono, "Telefono");
            Guard.ValidateEmail(ref email, "Email");
            Guard.CatalogValue(ref id_autorizacion, "Tipo de autorización");

            entityPersonasAut.Nombre = nombre;
            entityPersonasAut.Rfc = rfc;
            entityPersonasAut.Telefono = telefono;
            entityPersonasAut.Email = email;
            entityPersonasAut.id_autorizacion = id_autorizacion.GetValueOrDefault();
        }

        public static void UpdateDelete(ref PersonasAutorizadas entityPersonasAut,
             Autorizacion entityAutorizacion,
             int id_autorizacion)
        {
            if (entityAutorizacion.id_tipo_modalidad != EnumTipoModalidad.LÍNEA.GetHashCode())
            {
                throw new Exception("La autorización no es de tipo en linea.");
            }
            entityPersonasAut.id_autorizacion = id_autorizacion;
        }
    }
}
