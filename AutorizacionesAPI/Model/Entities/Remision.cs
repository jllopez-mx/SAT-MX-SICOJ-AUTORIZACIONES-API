namespace AutorizacionesAPI.Model.Entities
{
    public class Remision
    {
        public int id { get; set; }
        public int id_autorizacion { get; set; }
        public int id_unidad_administrativa_remite { get; set; }
        public int? id_unidad_administrativa_recibe { get; set; }
        public int? id_unidad_administrativa_externa { get; set; }
        public DateTime fecha_oficio { get; set; }
        public string numero_oficio { get; set; } = null!;
        public int id_tipo_autoridad { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
    }
}
