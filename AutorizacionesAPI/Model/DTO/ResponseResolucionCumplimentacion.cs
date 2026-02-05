namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseResolucionCumplimentacion
    {
        public int id { get; set; }
        public int idAsunto { get; set; } = new();
        public string fechaVencimiento { get; set; } = null!;
        public string oficioResolucionCumplimentacion { get; set; } = null!;
        public string fechaOficioResolucionCumplimentacion { get; set; } = null!;
        public int? idSentidoCumplimentacion { get; set; }
        public string? sentidoCumplimentacion { get; set; }
        public string fechaNotificacionCumplimentacion { get; set; } = null!;
    }
}
