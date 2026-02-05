namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseRequerimientoProdecon
    {
        public int id { get; set; }
        public int idAsunto { get; set; }
        public string numeroOficio { get; set; } = null!;
        public string numeroExpediente { get; set; } = null!;
        public string fechaOficio { get; set; } = null!;
        public string fechaIngreso_sat { get; set; } = null!;
        public bool requiereAccion { get; set; }
        public string? atencion { get; set; }
    }
}
