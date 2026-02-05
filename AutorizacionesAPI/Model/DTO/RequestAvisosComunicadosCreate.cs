namespace AutorizacionesAPI.Model.DTO
{
    public class RequestAvisosComunicadosCreate
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public int idTipoAviso { get; set; }
        public string numeroOficio { get; set; } = null!;
        public bool sinNumeroOficio { get; set; }
        public string fechaIngreso { get; set; } = null!;
        public string? observaciones { get; set; }
        public IFormFile documento { get; set; } = null!;
        public string numeroFolio { get; set; } = null!;
    }
}