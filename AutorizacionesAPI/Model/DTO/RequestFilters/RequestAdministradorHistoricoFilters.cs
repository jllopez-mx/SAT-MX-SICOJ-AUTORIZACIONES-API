namespace AutorizacionesAPI.Model.DTO
{
    public class RequestAdministradorHistoricoFilters
    {
        public List<string> ByNoAsunto { get; set; } = new()!;
        public List<string> ByFechaRecepcionDesde { get; set; } = new()!;
        public List<string> ByFechaRecepcionHasta { get; set; } = new()!;
        public List<string> ByFechaPresentacionDesde { get; set; } = new()!;
        public List<string> ByFechaPresentacionHasta { get; set; } = new()!;
        public List<string> ByFechaVencimientoDesde { get; set; } = new()!;
        public List<string> ByFechaVencimientoHasta { get; set; } = new()!;
        public List<string> ByRfc { get; set; } = new()!;
        public List<string> ByPromovente { get; set; } = new()!;
        public List<string> ByIdTipoAsunto { get; set; } = new()!;
        public List<string> ByIdTema { get; set; } = new()!;
        public List<string> ByAlerta { get; set; } = new()!;
        public List<string> ByAdministracion { get; set; } = new()!;
        public List<string> BySubadministracion { get; set; } = new()!;
        public List<string> ByAbogado { get; set; } = new()!;
        public List<string> ByIdEstadoTarea { get; set; } = new()!;
        public List<string> ByIdTipoEntrada { get; set; } = new()!;
        public List<string> ByIdEstadoProcesal { get; set; } = new()!;
    }
}
