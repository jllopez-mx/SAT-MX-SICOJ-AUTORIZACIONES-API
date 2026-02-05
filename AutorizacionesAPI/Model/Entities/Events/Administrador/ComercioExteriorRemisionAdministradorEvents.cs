using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.Administrador
{
    public class ComercioExteriorRemisionAdministradorEvents
    {
        public static Remision Create(
            int? id_autorizacion,
            int? id_tipo_autoridad,
            int? id_unidad_administrativa_remite,
            int? id_unidad_administrativa_recibe,
            int? id_unidad_administrativa_externa,
            DateTime fecha_oficio,
            string numero_oficio,            
            string usuarioCreacion
        )
        {
            Guard.CatalogValue(ref id_autorizacion, "Unidad administrativa Central");
            Guard.CatalogValue(ref id_tipo_autoridad, "Tipo autoridad");
            Guard.CatalogValue(ref id_unidad_administrativa_remite, "Unidad administrativa que remite");
            Guard.CatalogValue(ref id_unidad_administrativa_recibe, "Unidad administrativa que recibe", id_unidad_administrativa_recibe.GetValueOrDefault() == EnumTipoAutoridad.Interna.GetHashCode());
            Guard.CatalogValue(ref id_unidad_administrativa_externa, "Unidad administrativa externa", id_unidad_administrativa_externa.GetValueOrDefault() == EnumTipoAutoridad.Externa.GetHashCode());

            Remision entity = new()
            {
                id_autorizacion = id_autorizacion.GetValueOrDefault(),
                id_tipo_autoridad = id_tipo_autoridad.GetValueOrDefault(),
                id_unidad_administrativa_remite = id_unidad_administrativa_remite.GetValueOrDefault(),
                id_unidad_administrativa_recibe = id_unidad_administrativa_recibe,                
                id_unidad_administrativa_externa = id_unidad_administrativa_externa,
                fecha_oficio = fecha_oficio,
                numero_oficio = numero_oficio,                
                usuario_creacion = usuarioCreacion
            };

            return entity;
        }
    }
}
