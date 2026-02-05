namespace AutorizacionesAPI.Model.Entities
{
    public class AvisoSinRespuesta
    {
        public int id { get; set; }
        public int id_autorizacion { get; set; }
        public int? id_autorizacion_relacionado { get; set; }
        public string? no_asunto_externo { get; set; }
        public int? id_tipo_aviso { get; set; }
        public List<AvisoSinRespuestaFolio> folios { get; set; } = null!;
        public bool aprobado { get; set; }
        public DateTime? fecha_ingreso { get; set; }
        public string? observaciones { get; set; } = null!;
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; } = null!;
        public bool activo { get; set; }
    }
}