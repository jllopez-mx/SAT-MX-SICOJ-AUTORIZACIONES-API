namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseMediosDefensa
    {
        public string? noAsunto {  get; set; }
        public int? idMediosDefensa { get; set; }
        public string? mediosDefensa { get; set; }
        public int idUnidadAdministrativa { get; set; }
        public string unidadAdministrativa { get; set; } = null!;
        public int idEstadoProcesal { get; set; }
        public string estadoProcesal { get; set; } = null!;
    }

    public class MediosDefensaViewResponse
    {
        public List<ResponseMediosDefensa> mediosDefensa { get; set; } = new();
        public bool estadoSeccion { get; set; }
    }
}
