namespace AutorizacionesAPI.Model.DTO
{
    public class RequestRequerimientoCreate
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public string oficioRequerimiento { get; set; } = null!;
        public string fechaOficio { get; set; } = null!;
        public IFormFile documento { get; set; } = null!;
        public string numeroFolio { get; set; } = null!;
    }
}