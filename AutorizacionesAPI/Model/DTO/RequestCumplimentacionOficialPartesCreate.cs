namespace AutorizacionesAPI.Model.DTO
{
    public class RequestCumplimentacionOficialPartesCreate
    {
        public int? idAsunto { get; set; }
        public int? idTipoAsunto { get; set; }
        public string? noAsuntoExterno {  get; set; }
    }
}