using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.DTO.RequestFilters
{
    public class RequestPagerQueryDocumentosHistoricoFilters : PagerQuery
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
    }
}
