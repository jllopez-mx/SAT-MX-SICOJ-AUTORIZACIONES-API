namespace AutorizacionesAPI.Model.Entities
{
    public class Abogado
    {
        public int id { get; set; }
        public int? id_autorizacion { get; set; }
        public int? id_cumplimentacion { get; set; }
        public string id_abogado { get; set; } = null!;
        public DateTime fecha_asignacion { get; set; }
        public bool remitido { get; set; }
        public bool reasingado { get; set; }
    }
}
