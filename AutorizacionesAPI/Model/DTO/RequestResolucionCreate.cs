namespace AutorizacionesAPI.Model.DTO
{
    public class RequestResolucionCreate
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public string oficioResolucion { get; set; } = null!;
        public string fechaOficio { get; set; } = null!;
        public string? fechaVencimiento { get; set; } = null!;
        public int idSentido { get; set; }
        public IFormFile documento { get; set; } = null!;
        public string numeroFolio { get; set; } = null!;
    }
}