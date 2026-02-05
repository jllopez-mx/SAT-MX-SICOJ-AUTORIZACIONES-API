namespace AutorizacionesAPI.Model.Entities
{
    public class Documento
    {
        public int id { get; set; }
        public int? id_autorizacion { get; set; }
        public int? id_cumplimentacion { get; set; }
        public int id_tipo_documento { get; set; }
        public int id_seccion { get; set; }
        public int? id_renglon_seccion { get; set; }
        public int id_unidad_administrativa { get; set; }
        public string file_name { get; set; } = null!;
        public string file_path { get; set; } = null!;
        public string content_type { get; set; } = null!;
        public string file_size { get; set; } = null!;
        public string owner_name { get; set; } = null!;
        public string? numero_folio { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string? usuario_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
        public bool permanente { get; set; }
        public int id_rol { get; set; }
    }
}
