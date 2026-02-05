using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseAvisoSinRespuestaRelacionado
    {
        public int id { get; set; }
        public int? idAsunto { get; set; }
        public string? asunto { get; set; }
        public int? idTipoAsunto { get; set; }
        public string? tipoAsunto { get; set; }
        public int idAsuntoRelacionado { get; set; }
        public int? idTipoAviso { get; set; }
        public string? tipoAviso { get; set; }
        public List<string> folios { get; set; } = new();
        public string fechaIngreso { get; set; } = null!;
        public string? observaciones { get; set; }
        public bool aprobado { get; set; }
    }
}
