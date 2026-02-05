using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.Genericos
{
    public class ArchivoEvents
    {
        public static Documento CreateFolio(
            string? folio,
            int? id_autorizacion,
            int? id_tipo_archivo,
            int? id_seccion,
            int? id_renglon_seccion,
            int id_unidad_administrativa,
            string file_name,
            string file_path,
            string content_type,
            string file_size,
            string owner_name,
            string usuarioCreacion,
            bool permanente,
            int id_rol,
            int? idCumplimentacion
        )
        {

            Guard.ValidateString(ref folio, "Folio Documento", true);

            Documento entity = new()
            {
                numero_folio = folio,
                id_autorizacion = id_autorizacion,
                id_tipo_documento = id_tipo_archivo.GetValueOrDefault(),
                id_seccion = id_seccion.GetValueOrDefault(),
                id_renglon_seccion = id_renglon_seccion,
                id_unidad_administrativa = id_unidad_administrativa,
                file_name = file_name,
                file_path = file_path,
                content_type = content_type,
                file_size = file_size,
                owner_name = owner_name,
                usuario_creacion = usuarioCreacion,
                permanente = permanente,
                id_rol = id_rol,
                id_cumplimentacion = idCumplimentacion
            };
            return entity;
        }

        public static void UpdateWithFile(
            ref Documento entity,
            string? folio,
            int id_tipo_archivo,
            string file_name,
            string file_path,
            string content_type,
            string file_size,
            string owner_name,
            string usuarioModificacion
        )
        {
            entity.numero_folio = folio;
            entity.id_tipo_documento = id_tipo_archivo;
            entity.file_name = file_name;
            entity.file_path = file_path;
            entity.content_type = content_type;
            entity.file_size = file_size;
            entity.owner_name = owner_name;
            entity.usuario_modificacion = usuarioModificacion;
        }

        public static void Update(
            ref Documento entity,
            string? folio,
            int id_tipo_archivo,
            string owner_name,
            string usuarioModificacion
        )
        {
            entity.numero_folio = folio;
            entity.id_tipo_documento = id_tipo_archivo;
            entity.owner_name = owner_name;
            entity.usuario_modificacion = usuarioModificacion;
        }

        public static void Delete(ref Documento entity, string usuarioModificacion)
        {
            if (!entity.activo)
                throw new Exception("El documento ya se encuentra eliminado.");

            if (entity.permanente)
                throw new Exception("El documento no se puede eliminar.");

            entity.activo = false;
            entity.usuario_modificacion = usuarioModificacion;
        }
    }
}
