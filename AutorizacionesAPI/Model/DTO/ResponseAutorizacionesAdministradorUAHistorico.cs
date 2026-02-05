using Sicoj.Utils.Redis;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseAutorizacionesAdministradorUAHistorico : PortadorClass
    {
        public string noAsunto { get; set; } = null!;
        public string fechaPresentacion { get; set; } = null!;
        public string fechaVencimiento { get; set; } = null!;
        public string rfc { get; set; } = null!;
        public string promovente { get; set; } = null!;
        public int? idTema { get; set; }
        public string? tema { get; set; }
        public int? idUnidadAdministrativa { get; set; }
        public string? unidadAdministrativa { get; set; }
        public int? idSubadministracion { get; set; }
        public string? subadministracion { get; set; }
        public string? idAbogado { get; set; }
        public string? abogado { get; set; }
        public int? idTipoAsunto { get; set; }
        public string? tipoAsunto { get; set; }
        public int? idTipoModalidad { get; set; }
        public string? tipoModalidad { get; set; }
        public int? idEstadoProcesal { get; set; }
        public string? estadoProcesal { get; set; }
        public int? idEstadoTarea { get; set; }
        public string? estadoTarea { get; set; }
    }
}
