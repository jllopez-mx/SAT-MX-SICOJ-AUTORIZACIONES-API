namespace AutorizacionesAPI.Model.DTO.RequestFilters
{
    public class RequestAbogadoHistoricoFilters
    {
        public List<string> ByNumeroAsunto { get; set; } = new()!;
        public List<string> ByFechaRecepcionDesde { get; set; } = new()!;
        public List<string> ByFechaRecepcionHasta { get; set; } = new()!;
        public List<string> ByRfc { get; set; } = new()!;
        public List<string> ByPromovente { get; set; } = new()!;
        public List<string> ByTipoAsunto { get; set; } = new()!;
        public List<string> BySubtipo { get; set; } = new()!;
        public List<string> ByAlerta { get; set; } = new()!;
        public List<string> ByAdministracion { get; set; } = new()!;
        public List<string> BySubadministracion { get; set; } = new()!;
        public List<string> ByAbogado { get; set; } = new()!;
        public List<string> ByTarea { get; set; } = new()!;
        public List<string> ByTipoEntrada { get; set; } = new()!;
        public List<string> ByTipoModalidad { get; set; } = new()!;
        public List<string> ByEstadoProcesal { get; set; } = new()!;
    }
}
