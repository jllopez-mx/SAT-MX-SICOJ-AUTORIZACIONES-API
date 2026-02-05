namespace AutorizacionesAPI.Model.DTO
{
    public class RequestDocumentoUpdate
    {
        public IFormFile documento { get; set; } = null!;
        public int? idAsunto { get; set; }
        public int idTipoArchivo { get; set; }
        public int id { get; set; }
        public int idTipoAsunto { get; set; }
        public string numeroFolio { get; set; } = null!;
    }
}