namespace AutorizacionesAPI.Model.DTO.CatalogsContracts
{
    public class AdminsitracionCentral
    {
        public int id { get; set; }
        public string nombre { get; set; } = null!;
        public int idAdministracionGeneral { get; set; }
    }

    public class AdministracionResponse
    {
        public int id { get; set; }
        public string nombre { get; set; } = null!;
        public string descripcion { get; set; } = null!;
        public DateTime fechaInicio { get; set; }
        public int idAdministradion { get; set; }
        public bool isCentral { get; set; }
    }
}
