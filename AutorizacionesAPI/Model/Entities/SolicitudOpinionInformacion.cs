namespace AutorizacionesAPI.Model.Entities
{
    public class SolicitudOpinionInformacion
    {
        public int id { get; set; }
        public int idAutorizacion { get; set; }
        public int? idUnidadAdministrativa { get; set; } 
        public Boolean unidadInterna { get; set; }
        public string? unidadAdministrativaExterna { get; set; }
        public string? unidad_administrativa { get; set; }
        public string? noOficioSolicitud { get; set; } = null!;
        public DateTime fechaOficioSolicitud { get; set; }
        public Boolean? atendioSolicitud { get; set; }
        public string? noOficioRespuesta { get; set; }
        public DateTime? fechaOficioRespuesta { get; set; } 
        public DateTime? fechaRecepcion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public string? usuario_modificacion { get; set; }
    }
}
