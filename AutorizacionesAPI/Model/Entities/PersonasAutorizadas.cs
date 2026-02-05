namespace AutorizacionesAPI.Model.Entities
{
    public class PersonasAutorizadas
    {
        public int Id { get; set; }

        public string? Nombre { get; set; }

        public string? Rfc { get; set; }

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public int id_autorizacion {  get; set; }
    }
}
