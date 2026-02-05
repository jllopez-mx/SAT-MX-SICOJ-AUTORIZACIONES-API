namespace AutorizacionesAPI.Model.DTO
{
    public class RequestAvisosComunicadosUpdate
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public int id { get; set; }
        public bool requiereAccion { get; set; }
        public string? atencion { get; set; } = null!;        
    }
}