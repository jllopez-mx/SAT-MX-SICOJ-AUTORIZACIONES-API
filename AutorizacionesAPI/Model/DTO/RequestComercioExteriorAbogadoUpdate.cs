namespace AutorizacionesAPI.Model.DTO
{
    public class RequestComercioExteriorAbogadoUpdate
    {
        public int Id { get; set; }
        public int IdTipoModalidad { get; set; }
        public int IdTipoAutorizacion { get; set; }
        public string? Rfc { get; set; }
        public string? Promovente { get; set; } = null!;
        public bool PromoventeNoContribuyente { get; set; }
        public string? RfcContribuyente { get; set; }
        public string? Contribuyente { get; set; }
        public string? DespachosAutorizados { get; set; } = null!;
        public int? IdAutoridadDirigida { get; set; }
        public string? OtroAutoridadDirigida { get; set; }
        public string? DomicilioNotificaciones { get; set; } = null!;
        public string FechaPresentacion { get; set; } = null!;
        public int? IdFundamentoSolicitud { get; set; }
        public string? OtroFundamentoSolicitud { get; set; }
        public int? IdTema { get; set; }
        public string? OtroTema { get; set; }
        public bool? NoIndicaMonto { get; set; }
        public decimal? Monto { get; set; }
        public string FechaRecepcion { get; set; } = null!;
        public int? idAsuntoRelacionado { get; set; }
        public string? noAsuntoExterno { get; set; }
    }
}