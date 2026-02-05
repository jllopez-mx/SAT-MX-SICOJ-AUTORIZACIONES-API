namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseSolicitudOpinionInformacionAbogadoById
    {
        public int id { get; set; }
        public int idAsunto { get; set; }
        public Boolean unidadInterna { get; set; }
        public int? idUnidadAdministrativa { get; set; }
        public string? unidadAdministrativaExterna { get; set; } 
        public string? unidadAdministrativa { get; set; }
        public string noOficioSolicitud { get; set; } = string.Empty;
        public string? noOficioRespuesta { get; set; }
        public Boolean? atendioSolicitud { get; set; }
        public string fechaOficioSolicitud { get; set; } = null!;
        public string? fechaOficioRespuesta { get; set; }
        public string? fechaRecepcion { get; set; }
    }
}
