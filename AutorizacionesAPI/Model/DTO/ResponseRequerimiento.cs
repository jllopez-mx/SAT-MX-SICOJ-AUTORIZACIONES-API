using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseRequerimiento
    {
        public int id { get; set; }
        public int idAsunto { get; set; }
        public int? idTipoAsunto { get; set; }
        public string? tipoAsunto { get; set; }
        public string oficioRequerimiento { get; set; } = null!;
        public string fechaOficio { get; set; } = null!;
        public string? fechaNotificacion { get; set; }
        public string? fechaVencimiento { get; set; }
        public bool? atendioRequerimiento { get; set; }
        public string? fechaAtencion { get; set; }
        public bool? primeraProrroga { get; set; }
        public bool? segundaProrroga { get; set; }
    }
}
