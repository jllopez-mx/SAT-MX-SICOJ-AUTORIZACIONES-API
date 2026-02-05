namespace AutorizacionesAPI.Model.DTO
{
    public class RequestSolicitudOpinionInformacionAbogadoUpdate
    {
        public int id { get; set; }
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public Boolean unidadInterna { get; set; }
        public int? idUnidadAdministrativa { get; set; }
        public string? unidadAdministrativaExterna { get; set; }
        public string noOficioSolicitud { get; set; } = string.Empty;
        public string fechaOficioSolicitud { get; set; } = null!;
        public Boolean? atendioSolicitud { get; set; }
        public string? noOficioRespuesta { get; set; }
        public string? fechaOficioRespuesta { get; set; }
        public string? fechaRecepcion { get; set; } 
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
        public string? numeroFolio { get; set; }
    }
}