namespace AutorizacionesAPI.Model.Entities
{
    public class Reasignar
    {
        public int id { get; set; }
        public int id_autorizacion { get; set; }
        public DateTime fecha_reasignacion { get; set; }
        public string rfc_funcionario_reasignador { get; set; } = null!;
        public string rfc_funcionario_retirado { get; set; } = null!;
        public string rfc_funcionario_reasignado { get; set; } = null!;
        public int id_unidad_administrativa_reasingador { get; set; }
        public int id_subadministracion_reasignador { get; set; }
        public int id_unidad_administrativa_reasignado { get; set; }
        public int id_subadministracion_reasignado { get; set; }
        public int id_estado_procesal_previo { get; set; }
    }
}
