namespace AutorizacionesAPI.Model.Entities
{
    public class AvisosComunicados
    {
        public int id { get; set; }
        public int id_autorizacion { get; set; }
        public int id_tipo_aviso { get; set; }
        public string numero_oficio { get; set; } = null!;
        public bool sin_numero_oficio { get; set; }
        public DateTime fecha_ingreso { get; set; }
        public string? observaciones { get; set; }
        public bool? requiere_accion { get; set; }
        public string? atencion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string? usuario_creacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
    }
}
