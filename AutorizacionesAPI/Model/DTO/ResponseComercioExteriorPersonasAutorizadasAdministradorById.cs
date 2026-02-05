using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseComercioExteriorPersonasAutorizadasAdministradorById
    {
        public int Id {  get; set; }

        public string? Nombre { get; set; }

        public string? Rfc { get; set; }

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public int idAsunto { get; set; } = new();

    }
}
