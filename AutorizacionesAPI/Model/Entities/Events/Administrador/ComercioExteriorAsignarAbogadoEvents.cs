using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.Administrador
{
    public class ComercioExteriorAsignarAbogadoEvents
    {
        public static Abogado Create(
            string? id_abogado,
            string usuarioCreacion
        )
        {
            Guard.ValidateStringRfc(ref id_abogado!, "Abogado");

            Abogado entity = new()
            {
                id_abogado = id_abogado,
            };
            return entity;
        }
    }
}
