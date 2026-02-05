using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.Genericos
{
    public class ComercioExteriorModificacionEvents
    {
        public static Modificacion Create(
            ref Autorizacion entityComercioExterior,
            int? id_seccion,
            int? id_renglon_seccion,
            string usuarioCreacion
        )
        {

            Modificacion entity = new()
            {
                id_autorizacion = entityComercioExterior.id,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_renglon_seccion = id_renglon_seccion,
                id_estado_procesal = entityComercioExterior.id_estado_procesal,
                usuario_creacion = usuarioCreacion,
            };

            entityComercioExterior.id_estado_procesal = EnumEstadoProcesalCons.En_Reparacion;
            entityComercioExterior.fecha_control_solicitudes = DateTime.Now;
            return entity;
        }
    }
}