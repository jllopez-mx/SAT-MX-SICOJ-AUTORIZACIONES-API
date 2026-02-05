namespace AutorizacionesAPI.Model.DTO
{
    public class RequestResolucionUpdate
    {
        public int? id { get; set; }
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public string oficioResolucion { get; set; } = null!;
        public string fechaOficio { get; set; } = null!;
        public int idSentido { get; set; }
        public string fechaNotificacion { get; set; } = null!;
        public string? fechaVencimiento { get; set; } = null!;
        public IFormFile? documento { get; set; } = null!;
        public string? numeroFolio { get; set; } = null!;
        public int? idTipoArchivo { get; set; } = null!;
    }
}
