namespace AutorizacionesAPI.Model.Entities
{
    public class Cumplimentacion
    {
        public int id { get; set; }
        public int id_autorizacion { get; set; }
        public int? id_tipo_asunto { get; set; }
        public int id_tipo_modalidad { get; set; }
        public string? no_asunto_cumplimentacion { get; set; }
        public string? no_juicio_recuro_amparo { get; set; }
        public DateTime? fecha_recepcion_solicitud { get; set; }
        public DateTime? fecha_firmeza { get; set; }
        public int? id_plazo_cumplimiento { get; set; }
        public DateTime? fecha_vencimiento { get; set; }
        public int? id_organo_jurisdiccional { get; set; }
        public string? organo_jurisdiccional { get; set; }
        public int? id_unidad_administrativa_cumplimiento { get; set; }
        public string? unidad_administrativa_cumplimiento { get; set; }
        public string? oficio_resolucion_impugnada { get; set; }
        public DateTime? fecha_oficio_resolucion_impugnada { get; set; }
        public DateTime? fecha_turnado { get; set; }
        public int id_unidad_administrativa_central { get; set; }
        public int? id_unidad_administrativa { get; set; }
        public int? id_subadministracion { get; set; }
        public string? no_empleado_turna { get; set; }
        public int id_estado_tarea { get; set; }
        public int id_estado_procesal { get; set; }
        public DateTime? fecha_control_solicitudes { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string? usuario_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
    }
}
