using Sicoj.Utils.ViewModels;


namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseComercioExteriorPersonasAutorizadasAbogadoById
    {
        public int id { get; set; }

        public string? nombre { get; set; }

        public string? rfc { get; set; }

        public string? telefono { get; set; }

        public string? email { get; set; }

        public int idAsunto { get; set; } = new();
    }
}
