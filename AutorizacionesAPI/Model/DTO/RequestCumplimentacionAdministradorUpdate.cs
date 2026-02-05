namespace AutorizacionesAPI.Model.DTO
{
    public class RequestCumplimentacionAdministradorUpdate
    {
        public int id { get; set; }
        public int? idTipoAsuntoCumplimentar { get; set; }
        public string? rfc { get; set; }
        public string? promovente { get; set; }
        public bool promoventeNoContribuyente { get; set; }
        public string? rfcContribuyente { get; set; }
        public string? contribuyenteAutorizacion { get; set; }
        public string? noJuicioRecuroAmparo { get; set; }
        public string? fechaRecepcionSolicitud { get; set; }
        public string? fechaFirmeza { get; set; }
        public int? idPlazoCumplimiento { get; set; }
        public int? idOrganoJurisdiccional { get; set; }
        public string? organoJurisdiccional { get; set; }
        public int? idUnidadAdministrativaCumplimiento { get; set; }
        public string? unidadAdministrativaCumplimiento { get; set; }
        public string? oficioResolucionImpugnada { get; set; }
        public string? fechaOficioResolucionImpugnada { get; set; }
        public int? idSubadministracion { get; set; }
        public bool Historico { get; set; }
        public string? fechaVencimiento { get; set; }
    }
}