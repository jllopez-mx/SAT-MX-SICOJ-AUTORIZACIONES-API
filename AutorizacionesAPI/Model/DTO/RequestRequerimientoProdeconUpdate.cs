namespace AutorizacionesAPI.Model.DTO
{
    public class RequestRequerimientoProdeconUpdate
    {        
        public int idTipoAsunto { get; set; }
        public int idAsunto { get; set; }
        public int id { get; set; }
        public string numeroOficio { get; set; } = null!;
        public string? numeroExpediente { get; set; }
        public string fechaOficio { get; set; } = null!;
        public string fechaIngresoSat { get; set; } = null!;
        public bool requiereAccion { get; set; }
        public string? atencion { get; set; }
    }
}