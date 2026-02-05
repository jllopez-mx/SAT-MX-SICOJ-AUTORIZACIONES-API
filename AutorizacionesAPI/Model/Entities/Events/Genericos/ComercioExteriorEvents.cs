using AutorizacionesAPI.Model.ViewModels.Enums;

namespace AutorizacionesAPI.Model.Entities.Events.Genericos
{
    public class ComercioExteriorEvents
    {
        #region Reasignar
        public static Reasignar UpdateReasignarAbogado(ref Autorizacion entityAutorizacion,
          string? usuarioModificacion,
          string? rfcNuevoAbogado,
          string? rfcAntiguoAbogado,
          int? idUnidadAdministrativaReasignador,
          int? idSubadministrativaReasignador,
          int? idUnidadAdministrativaReasignado,
          int? idSubadministracioReasignado
        )
        {
            ValidacionEstadosGenericosEvents.ValidateReasignarAbogado(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);
            if (string.IsNullOrEmpty(rfcAntiguoAbogado))
                throw new Exception("El asunto no se tiene asignado un abogado.");

            if (rfcAntiguoAbogado == rfcNuevoAbogado)
                throw new Exception("El asunto no se puede asignar al mismo abogado.");

            entityAutorizacion.id_estado_tarea = EnumEstadoTareaCons.Reasingado;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;

            Reasignar entityReasignacion = new()
            {
                id_autorizacion = entityAutorizacion.id,
                rfc_funcionario_reasignador = usuarioModificacion!,
                rfc_funcionario_reasignado = rfcNuevoAbogado!,
                rfc_funcionario_retirado = rfcAntiguoAbogado!,
                id_unidad_administrativa_reasingador = idUnidadAdministrativaReasignador.GetValueOrDefault(),
                id_subadministracion_reasignador = idSubadministrativaReasignador.GetValueOrDefault(),
                id_unidad_administrativa_reasignado = idUnidadAdministrativaReasignado.GetValueOrDefault(),
                id_subadministracion_reasignado = idSubadministracioReasignado.GetValueOrDefault(),
                id_estado_procesal_previo = entityAutorizacion.id_estado_procesal
            };

            return entityReasignacion;
        }

        public static Reasignar UpdateReasignarAdministrador(ref Autorizacion entityAutorizacion,
           string? usuarioModificacion,
           string? rfcAntiguoAbogado,
           int? idUnidadAdministrativaReasignador,
           int? idSubadministrativaReasignador,
           int? idUnidadAdministrativaReasignado,
           int? idSubadministracioReasignado
        )
        {
            ValidacionEstadosGenericosEvents.ValidateReasignarAdministracion(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            if (string.IsNullOrEmpty(rfcAntiguoAbogado))
                throw new Exception("El asunto no se tiene asignado un abogado.");

            entityAutorizacion.id_estado_tarea = EnumEstadoTareaCons.Reasingado;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;

            Reasignar entityReasignacion = new()
            {
                id_autorizacion = entityAutorizacion.id,
                rfc_funcionario_reasignador = usuarioModificacion!,
                rfc_funcionario_reasignado = string.Empty!,
                rfc_funcionario_retirado = rfcAntiguoAbogado!,
                id_unidad_administrativa_reasingador = idUnidadAdministrativaReasignador.GetValueOrDefault(),
                id_subadministracion_reasignador = idSubadministrativaReasignador.GetValueOrDefault(),
                id_unidad_administrativa_reasignado = idUnidadAdministrativaReasignado.GetValueOrDefault(),
                id_subadministracion_reasignado = idSubadministracioReasignado.GetValueOrDefault(),
                id_estado_procesal_previo = entityAutorizacion.id_estado_procesal
            };

            return entityReasignacion;
        }
        #endregion
    }
}
