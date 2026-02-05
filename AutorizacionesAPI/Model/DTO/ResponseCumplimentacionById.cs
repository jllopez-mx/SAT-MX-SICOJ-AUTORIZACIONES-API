using Sicoj.Utils.Redis;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseCumplimentacionById : PortadorClass
    {
        public int idAsunto { get; set; }
        public int? idTipoAsunto { get; set; }
        public string? tipoAsunto { get; set; }
        public string? noAsunto { get; set; }
        public int? idTipoModalidad { get; set; }
        public string? tipoModalidad { get; set; }
        public string rfc { get; set; } = null!;
        public string promovente { get; set; } = null!;
        public bool promoventeNoContribuyente { get; set; }
        public string? rfcContribuyente { get; set; }
        public string? contribuyente { get; set; }
        public string? noAsuntoCumplimentacion { get; set; }
        public string? noJuicioRecuroAmparo { get; set; }
        public string fechaRecepcionSolicitud { get; set; } = null!;
        public string fechaFirmeza { get; set; } = null!;
        public int? idPlazoCumplimiento { get; set; }
        public string fechaVencimiento { get; set; } = null!;
        public int? idOrganoJurisdiccional { get; set; }
        public string? organoJurisdiccional { get; set; }
        public int? idUnidadAdministrativaCumplimiento { get; set; }
        public string? unidadAdministrativaCumplimiento { get; set; }
        public string? oficioResolucionImpugnada { get; set; }
        public string fechaOficioResolucionImpugnada { get; set; } = null!;
        public int? idUnidadAdministrativa { get; set; }
        public string? unidadAdministrativa { get; set; }
        public int? idSubadministracion { get; set; }
        public string? subadministracion { get; set; }
        public int? idTipoAsuntoCumplimentar { get; set; }
        public string? tipoAsuntoCumplimentar { get; set; }
        public string? idAbogado { get; set; }
        public string? abogado { get; set; }
        public int? idEstadoProcesal { get; set; }
        public string? estadoProcesal { get; set; }
        public int? idEstadoTarea { get; set; }
        public string? estadoTarea { get; set; }
        public bool externoCumplimentacion { get; set; }
        public int? idSeccionModificar { get; set; }
        public int? idSeccionDescartar { get; set; }
    }
}
