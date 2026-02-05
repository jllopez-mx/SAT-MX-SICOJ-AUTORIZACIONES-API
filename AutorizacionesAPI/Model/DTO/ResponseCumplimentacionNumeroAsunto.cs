using Sicoj.Utils.ViewModels;

namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseCumplimentacionNumeroAsunto
    {
        public int idAsunto { get; set; }
        public int? idTipoAsunto { get; set; }
        public string? tipoAsunto { get; set; }
        public string noAsunto { get; set; } = null!;
        public string rfc { get; set; } = null!;
        public string promovente { get; set; } = null!;


    }
}
