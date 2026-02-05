namespace AutorizacionesAPI.Model.DTO
{
    public class RequestRequerimientoProdeconCreate
    {
        public int idTipoAsunto { get; set; }
        public int idAsunto { get; set; }
        public string numeroOficio { get; set; } = null!;
        public string? numeroExpediente { get; set; }
        public string fechaOficio { get; set; } = null!;
        public string fechaIngresoSat { get; set; } = null!;
        public bool requiereAccion { get; set; }
        public string? atencion { get; set; }
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
        public string? numeroFolio { get; set; } = null!;
    }
}