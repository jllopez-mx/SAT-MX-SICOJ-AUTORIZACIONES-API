namespace AutorizacionesAPI.Model.DTO
{
    public class ResponseReasignar
    {
        public int ReasignacionesExitosas { get; set; }
        public int ReasignacionesIncorrectas { get; set; }
        public string abogado { get; set; } = null!;
        public string administrador { get; set; } = null!;
    }
}
