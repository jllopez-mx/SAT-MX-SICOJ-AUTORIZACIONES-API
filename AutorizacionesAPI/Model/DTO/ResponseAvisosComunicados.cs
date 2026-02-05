using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseAvisosComunicados
    {
        public int id { get; set; }
        public int idAsunto { get; set; } = new();
        public int? idTipoAviso { get; set; }
        public string? tipoAviso { get; set; }
        public string numeroOficio { get; set; } = null!;
        public bool sinNumeroOficio { get; set; }
        public string fechaIngreso { get; set; } = null!;
        public string observaciones { get; set; } = null!;
        public bool? requiereAccion { get; set; }
        public string? atencion { get; set; }
    }
}
