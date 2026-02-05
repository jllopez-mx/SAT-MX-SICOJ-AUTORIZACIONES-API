namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseDocumentosConFolio
    {
        public int Id { get; set; }
        public int? idAsunto { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public string NombreDocumento { get; set; } = null!;
        public string TamanoDocumento { get; set; } = null!;
        public int? IdSeccion { get; set; }
        public string? Seccion { get; set; }
        public string NumeroFolio { get; set; } = null!;
        public int? IdRenglonSeccion { get; set; }
        public bool Permanente { get; set; }
    }

    public class ResponseDocumentoHistorico
    {
        public int Id { get; set; }
        public int? idAsunto { get; set; }
        public int IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; } = null!;
        public string NombreDocumento { get; set; } = null!;
        public string TamanoDocumento { get; set; } = null!;
        public int IdSeccion { get; set; } = new();
        public string Seccion { get; set; } = null!;
        public string Folio { get; set; } = null!;
        public int? IdRenglonSeccion { get; set; }
        public bool Permanente { get; set; }
    }
}
