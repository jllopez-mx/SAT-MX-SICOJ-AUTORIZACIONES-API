namespace AutorizacionesAPI.Model.Entities
{
    public class Requerimiento
    {
        public int id { get; set; }
        public int id_autorizacion { get; set; }
        public string oficio_requerimiento { get; set; } = null!;
        public DateTime fecha_oficio { get; set; }
        public DateTime? fecha_notificacion { get; set; }
        public DateTime? fecha_vencimiento { get; set; }
        public bool? atendio_requerimiento { get; set; }
        public DateTime? fecha_atencion { get; set; }
        public bool? primer_prorroga { get; set; }
        public bool? segunda_prorroga { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
    }
}
