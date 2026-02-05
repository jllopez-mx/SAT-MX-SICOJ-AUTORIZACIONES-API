using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseImpuestosInternosById : PortadorClass
    {
        public int? idTipoAsunto { get; set; }
        public string? tipoAsunto { get; set; }
        public int? idTipoModalidad { get; set; }
        public string? tipoModalidad { get; set; }

        public string? noAsunto { get; set; }

        public string rfc { get; set; } = null!;

        public string promovente { get; set; } = null!;

        public bool promoventeNoContribuyente { get; set; }

        public string? rfcContribuyente { get; set; }

        public string? contribuyente { get; set; }

        public string domicilioPromovente { get; set; } = null!;

        public string fechaPresentacion { get; set; } = null!;

        public string fechaRecepcion { get; set; } = null!;

        public string fechaVencimiento { get; set; } = null!;

        public int? idUnidadAdministrativa { get; set; }
        public string? unidadAdministrativa { get; set; }
        public int? idEstadoProcesal { get; set; }
        public string? estadoProcesal { get; set; }
        public int? idEstadoTarea { get; set; }
        public string? estadoTarea { get; set; }
        public bool remitido { get; set; }
    }
}
