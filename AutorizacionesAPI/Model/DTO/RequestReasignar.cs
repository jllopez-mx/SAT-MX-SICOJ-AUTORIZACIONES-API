namespace AutorizacionesAPI.Model.DTO
{
    public class RequestReasignar
    {
        public List<int> idAsuntoLista { get; set; } = null!;
        public string idAbogado { get; set; } = null!;
        public int idTipoAsunto { get; set; }
    }

    public class RequestAdministradorGlobalReasignar
    {
        public int idTipoAsunto { get; set; }
        public List<int> idAsuntoLista { get; set; } = null!;
        public string? idAbogado { get; set; }
        public string? rfcAdministrador { get; set; }
    }
}