using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.DTO.RequestFilters
{
    public class RequestPagerQueryDocumentosHistoricoSeccionFilters : PagerQuery
    {
        public int idAsunto { get; set; }
        public int idTipoAsunto { get; set; }
        public int idSeccion { get; set; }
        public int? idRenglonSeccion { get; set; }
    }
}
