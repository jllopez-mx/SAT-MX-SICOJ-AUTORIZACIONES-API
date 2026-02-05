namespace AutorizacionesAPI.Model.Entities
{
    public class AvisoConRespuesta
    {
        public int id { get; set; }
        public int id_autorizacion { get; set; }
        public int? id_autorizacion_relacionado { get; set; }
        public string? no_asunto_externo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; } = null!;
        public bool activo { get; set; }
    }
}
