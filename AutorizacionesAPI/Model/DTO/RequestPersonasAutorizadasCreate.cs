namespace AutorizacionesAPI.Model.DTO
{
    public class RequestPersonasAutorizadasCreate
    {
        public string? Nombre { get; set; }
        public string? Rfc { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int idAsunto {  get; set; }
    }
}