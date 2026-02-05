namespace AutorizacionesAPI.Model.DTO.RequestFilters
{
    public class RequestOficialPartesHistoricoFilters
    {
        public List<string> ByNoAsunto { get; set; } = new()!;
        public List<string> ByFechaRecepcionDesde { get; set; } = new()!;
        public List<string> ByFechaRecepcionHasta { get; set; } = new()!;
        public List<string> ByRfc { get; set; } = new()!;
        public List<string> ByPromovente { get; set; } = new()!;
        public List<string> ByIdTipoAsunto { get; set; } = new()!;
        public List<string> ByIdEstadoTarea { get; set; } = new()!;
        public List<string> ByIdTipoModalidad { get; set; } = new()!;
        public List<string> ByIdEstadoProcesal { get; set; } = new()!;
    }
}