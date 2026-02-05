namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class ComercioExteriorAvisosComunicadosEvents
    {
        public static AvisosComunicados Create(ref Autorizacion entityAutorizacion,
            int? id_tipo_aviso,
            string? numero_oficio,
            bool? sin_numero_oficio,
            DateTime? fecha_ingreso,
            string? observaciones,
            string usuario_creacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateAvisosComunicadosUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            var entity = new AvisosComunicados
            {
                id_autorizacion = entityAutorizacion.id,
                id_tipo_aviso = id_tipo_aviso.GetValueOrDefault(),
                numero_oficio = numero_oficio!,
                sin_numero_oficio = sin_numero_oficio.GetValueOrDefault(),
                fecha_ingreso = fecha_ingreso.GetValueOrDefault(),
                observaciones = observaciones,
                usuario_creacion = usuario_creacion!
            };

            return entity;
        }

        public static void Update(ref Autorizacion entityAutorizacion,
            ref AvisosComunicados entity,
            bool? requiere_accion,
            string? atencion,
            string usuario_modificacion)
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateAvisosComunicadosUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            entity.requiere_accion = requiere_accion;
            entity.atencion = atencion!;
            entity.usuario_modificacion = usuario_modificacion!;
        }
    }
}
