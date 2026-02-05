namespace AutorizacionesAPI.Model.Entities
{
    public class Reactivar
    {
        public int id { get; set; }
        public int? id_autorizacion { get; set; }
        public int? id_cumplimentacion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string? usuario_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
        public int id_estado_procesal { get; set; }
    }
}
