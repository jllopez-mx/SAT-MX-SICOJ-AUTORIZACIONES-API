namespace AutorizacionesAPI.Model.Entities
{
    public class Resolucion
    {
        public int id { get; set; }
        public int? id_autorizacion { get; set; }
        public int? id_cumplimentacion { get; set; }
        public DateTime? fecha_vencimiento { get; set; }
        public string oficio_resolucion { get; set; } = null!;
        public DateTime fecha_oficio { get; set; }
        public int id_sentido { get; set; }
        public DateTime? fecha_notificacion { get; set; }        
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
    }
}
