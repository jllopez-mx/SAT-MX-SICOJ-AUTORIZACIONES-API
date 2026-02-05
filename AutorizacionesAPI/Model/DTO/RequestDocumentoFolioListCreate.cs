namespace AutorizacionesAPI.Model.DTO
{
    public class RequestDocumentoFolioListCreate
    {
        public List<RequestDocumentoFolio> documentoList { get; set; } = null!;
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public int idSeccion { get; set; }
    }

    public class RequestDocumentoFolio
    {
        public IFormFile documento { get; set; } = null!;
        public int idTipoArchivo { get; set; }
        public string numeroFolio { get; set; } = null!;
    }
}