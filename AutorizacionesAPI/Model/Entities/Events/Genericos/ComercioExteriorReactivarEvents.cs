namespace AutorizacionesAPI.Model.Entities.Events.Genericos
{
    public class ComercioExteriorReactivarEvents
    {
        public static Reactivar Create(
            Autorizacion entityAutorizacion,
            Cumplimentacion cumplimentacion,
            string? usuario_modificacion
        )
        {
            Reactivar entity = new()
            {
                id_autorizacion = entityAutorizacion is null ? null : entityAutorizacion.id,
                id_cumplimentacion = cumplimentacion is null ? null : cumplimentacion.id,
                id_estado_procesal = entityAutorizacion is null ? cumplimentacion!.id_estado_procesal : entityAutorizacion.id_estado_procesal,
                usuario_creacion = usuario_modificacion,
            };
            return entity;
        }
    }
}
