namespace AutorizacionesAPI.Model.ViewModels.Enums
{
    public enum EnumEstadoTarea
    {
        Pendiente_de_registrar = 1,
        Pendiente_de_turnar = 2,
        Pendiente_de_asignar = 3,
        Asignado = 4,
        Concluido_Remitido = 5,
        Atendido = 6,
        Reasingado = 7,
    }

    public class EnumEstadoTareaCons
    {
        public const int Pendiente_de_registrar = 1;
        public const int Pendiente_de_turnar = 2;
        public const int Pendiente_de_asignar = 3;
        public const int Asignado = 4;
        public const int Concluido_Remitido = 5;
        public const int Atendido = 6;
        public const int Reasingado = 7;
    }
}
