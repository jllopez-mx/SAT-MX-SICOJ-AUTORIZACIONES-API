namespace AutorizacionesAPI.Model.DTO
{
    public class RequestAsignar
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public string idAbogado { get; set; } = null!;
    }
}