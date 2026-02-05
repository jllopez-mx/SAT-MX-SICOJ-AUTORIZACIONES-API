namespace AutorizacionesAPI.Model.Entities
{
    public class Autorizacion
    {
        public int id { get; set; }
        public int? id_tipo_asunto { get; set; }
        public int id_tipo_modalidad { get; set; }
        public int? id_tipo_autorizacion { get; set; }
        public string no_asunto { get; set; } = null!;
        public string? rfc { get; set; }
        public string? promovente { get; set; } = null!;
        public bool? promovente_no_contribuyente { get; set; }
        public string? rfc_contribuyente { get; set; }
        public string? contribuyente { get; set; }
        public string? despachos_autorizados { get; set; }
        public int? id_autoridad_dirigida { get; set; }
        public string? otra_autoridad_dirigida { get; set; }
        public string domicilio_promovente { get; set; } = null!;
        public string? domicilio_notificaciones { get; set; }
        public DateTime fecha_presentacion { get; set; }
        public int? id_fundamento_solicitud { get; set; }
        public string? otro_fundamento_solicitud { get; set; }
        public int? id_tema { get; set; }
        public string? otro_tema { get; set; }
        public bool? no_indica_monto { get; set; }
        public decimal? monto { get; set; }
        public DateTime fecha_recepcion { get; set; }
        public DateTime fecha_vencimiento { get; set; }
        public DateTime fecha_turnado { get; set; }
        public int id_unidad_administrativa_central { get; set; }
        public int? id_unidad_administrativa { get; set; }
        public int? id_subadministracion { get; set; }
        public string? no_empleado_turna { get; set; }
        public int id_estado_tarea { get; set; }
        public int id_estado_procesal { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string? usuario_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
        public Abogado abogado { get; set; } = null!;
        public bool? requerimiento { get; set; }
        public bool? solicitud_opinion {  get; set; }
        public bool? solicitud_opinion_completo { get; set; }
        public DateTime? fecha_control_solicitudes { get; set; }
        public bool externo_cumplimentacion { get; set; }
    }
}
