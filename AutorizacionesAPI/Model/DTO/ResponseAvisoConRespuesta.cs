using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseAvisoConRespuesta
    {
        public int id { get; set; }
        public int? idAsunto { get; set; }
        public string? asunto { get; set; }
        public int idAsuntoRelacionado { get; set; }
        public int? idTipoSentido { get; set; }
        public string? tipoSentido { get; set; }
        public string fechaVencimiento { get; set; } = null!;
        public string oficioResolucion { get; set; } = null!;
        public string? fechaOficio { get; set; }
        public string fechaNotificacion { get; set; } = null!;
    }
}
