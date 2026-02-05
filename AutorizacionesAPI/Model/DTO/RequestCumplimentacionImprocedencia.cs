namespace AutorizacionesAPI.Model.DTO
{
    public class RequestCumplimentacionImprocedencia
    {
        public int id { get; set; }
        public IFormFile documento { get; set; } = null!;
        public string numeroFolio { get; set; } = null!;
    }
}
