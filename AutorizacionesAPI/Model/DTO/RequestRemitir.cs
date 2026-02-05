namespace AutorizacionesAPI.Model.DTO
{
    public class RequestRemitir
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public int idTipoAutoridad { get; set; }
        public int idUnidadAdministrativaRecibe { get; set; }
        public int idUnidadAdministrativaExterna { get; set; }        
        public string fechaOficio { get; set; } = null!;
        public string numeroOficio { get; set; } = null!;
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
        public string? numeroFolio { get; set; } = null!;
    }
}