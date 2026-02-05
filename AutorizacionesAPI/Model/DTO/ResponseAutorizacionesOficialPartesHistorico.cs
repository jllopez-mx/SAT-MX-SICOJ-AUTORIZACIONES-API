using Sicoj.Utils.Redis;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseAutorizacionesOficialPartesHistorico : PortadorClass
    {
        public string? noAsunto { get; set; }
        public int? idTipoAsunto { get; set; }
        public string? tipoAsunto { get; set; }
        public int? idTipoEntrada { get; set; }
        public string? tipoEntrada { get; set; }
        public string rfc { get; set; } = null!;
        public string promovente { get; set; } = null!;
        public string fecha_recepcion { get; set; } = null!;
        public int? idEstadoProcesal { get; set; }
        public string? estadoProcesal { get; set; }
        public int? idEstadoTarea { get; set; }
        public string? estadoTarea { get; set; }
    }
}