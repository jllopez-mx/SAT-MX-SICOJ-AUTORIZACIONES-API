namespace AutorizacionesAPI.Model.DTO
{
    public class RequestRequerimientoUpdate
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public int id { get; set; }
        public string oficioRequerimiento { get; set; } = null!;
        public string fechaOficio { get; set; } = null!;
        public string fechaNotificacion { get; set; } = null!;
        public bool? atendioRequerimiento { get; set; }
        public string? fechaAtencion { get; set; }
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public string? numeroFolio { get; set; } = null!;
        public bool? primeraProrroga { get; set; }
        public bool? segundaProrroga { get; set; }
    }
}