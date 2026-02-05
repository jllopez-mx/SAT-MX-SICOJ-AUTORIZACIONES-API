namespace AutorizacionesAPI.Model.Entities.Events.Genericos
{
    public class ComercioExteriorDescartarEvents
    {
        public static Descartar Create(
            Autorizacion entityAutorizacion,
            Cumplimentacion cumplimentacion,
            int? id_seccion,
            string? usuario_modificacion
        )
        {
            Descartar entity = new()
            {
                id_autorizacion = entityAutorizacion is null ? null! : entityAutorizacion.id,
                id_cumplimentacion = cumplimentacion is null ? null! : cumplimentacion.id,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_estado_procesal = entityAutorizacion is null ? cumplimentacion!.id_estado_procesal! : entityAutorizacion.id_estado_procesal,
                usuario_creacion = usuario_modificacion,
            };
            return entity;
        }

        public static Descartar CreateCumplimentacionPorImprocedencia(
            Cumplimentacion cumplimentacion,
            int? id_seccion,
            string? usuario_modificacion
        )
        {
            Descartar entity = new()
            {
                id_cumplimentacion = cumplimentacion is null ? null! : cumplimentacion.id,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_estado_procesal = cumplimentacion!.id_estado_procesal!,
                usuario_creacion = usuario_modificacion,
            };
            return entity;
        }

        public static Descartar CreateCumplimentacionNoAsunto(
            Cumplimentacion cumplimentacion,
            int? id_seccion,
            string? usuario_modificacion
        )
        {
            Descartar entity = new()
            {
                id_cumplimentacion = cumplimentacion is null ? null! : cumplimentacion.id,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_estado_procesal = cumplimentacion!.id_estado_procesal!,
                usuario_creacion = usuario_modificacion,
            };
            return entity;
        }
    }
}
