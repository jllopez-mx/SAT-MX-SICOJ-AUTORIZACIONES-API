using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseResolucion
    {
        public int id { get; set; }
        public int idAsunto { get; set; } = new();        
        public string fechaVencimiento { get; set; } = null!;
        public string oficioResolucion { get; set; } = null!;
        public string fechaOficio { get; set; } = null!;
        public int? idSentido { get; set; }
        public string? sentido { get; set; }
        public string fechaNotificacion { get; set; } = null!;
        public bool primera { get; set; }
    }
}
