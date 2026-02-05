namespace AutorizacionesAPI.Model.DTO
{
    public class RequestImpuestosInternosAbogadoUpdate
    {
        public int Id { get; set; }
        public int IdTipoModalidad { get; set; }
        public string? Rfc { get; set; }
        public string Promovente { get; set; } = null!;
        public bool PromoventeNoContribuyente { get; set; }
        public string? RfcContribuyente { get; set; }
        public string? Contribuyente { get; set; }
        public string? DespachosAutorizados { get; set; }
        public string DomicilioPromovente { get; set; } = null!;
        public string? DomicilioNotificaciones { get; set; } = null!;
        public string FechaPresentacion { get; set; } = null!;
        public string FechaRecepcion { get; set; } = null!;
        public int IdTema { get; set; }
        public string? OtroTema { get; set; }
        public bool? NoIndicaMonto { get; set; }
        public decimal? Monto { get; set; }
        public string? NoAsuntoAviso { get; set; }
    }
}