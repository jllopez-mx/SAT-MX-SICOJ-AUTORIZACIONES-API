namespace AutorizacionesAPI.Model.DTO
{
    public class RequestDocumentoDelete
    {
        public int? idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public List<int> Ids { get; set; } = new();
    }
}
