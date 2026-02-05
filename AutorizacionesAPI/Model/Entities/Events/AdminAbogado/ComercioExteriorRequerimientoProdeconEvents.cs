using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ComercioExteriorRequerimientoProdeconEvents
    {
        public static RequerimientoProdecon Create(Autorizacion entityAutorizacion,
           string? numero_oficio,
           string? numero_expediente,
           DateTime fecha_oficio,
           DateTime fecha_ingreso_sat,
           bool requiere_accion,
           string? atencion,
           string? usuario_creacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoProdeconCreate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.ValidateStringNumero(ref numero_oficio!, "Número Oficio");
            Guard.ValidateStringNumero(ref numero_expediente!, "Número Expediente");
            Guard.ValidateString(ref atencion, "Atención", requiere_accion);

            var entity = new RequerimientoProdecon
            {
                id_autorizacion = entityAutorizacion.id,
                numero_oficio = numero_oficio!,
                numero_expediente = numero_expediente!,
                fecha_oficio = fecha_oficio!,
                fecha_ingreso_sat = fecha_ingreso_sat,
                requiere_accion = requiere_accion,
                atencion = atencion,
                usuario_creacion = usuario_creacion!
            };

            return entity;
        }

        public static void Update(RequerimientoProdecon entity,
            Autorizacion entityAutorizacion,
           string? numero_oficio,
           string? numero_expediente,
           DateTime fecha_oficio,
           DateTime fecha_ingreso_sat,
           bool requiere_accion,
           string? atencion,
           string? usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoProdeconUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.ValidateStringNumero(ref numero_oficio!, "Número Oficio");
            Guard.ValidateStringNumero(ref numero_expediente!, "Número Expediente");
            Guard.ValidateString(ref atencion, "Atención", requiere_accion);

            entity.numero_oficio = numero_oficio!;
            entity.numero_expediente = numero_expediente!;
            entity.fecha_oficio = fecha_oficio!;
            entity.fecha_ingreso_sat = fecha_ingreso_sat;
            entity.requiere_accion = requiere_accion;
            entity.atencion = atencion;
            entity.usuario_modificacion = usuario_modificacion!;
        }
    }
}
