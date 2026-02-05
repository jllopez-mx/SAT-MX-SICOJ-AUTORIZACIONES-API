namespace AutorizacionesAPI.Model.DTO
{
    public class RequestAvisoSinRespuesta
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public string? observaciones { get; set; }
        public List<string> folios { get; set; } = new();
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public string? numeroFolio { get; set; } = null!;
    }
}