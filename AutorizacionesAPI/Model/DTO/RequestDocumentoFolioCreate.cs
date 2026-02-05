namespace AutorizacionesAPI.Model.DTO
{
    public class RequestDocumentoFolioCreate
    {
        public IFormFile documento { get; set; } = null!;
        public int? idAsunto { get; set; }
        public int idTipoArchivo { get; set; }
        public int idTipoAsunto { get; set; }
        public int idSeccion { get; set; }
        public string numeroFolio { get; set; } = null!;
        public int? idRenglonSeccion { get; set; } = null!;
    }
}
