using AutorizacionesAPI.Model.Entities.Events.AdminAbogado;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.Entities.Events.Administrador
{
    public class ImpuestosInternosAdministradorEvents
    {
        public static void UpdateModalidadFisico(
            ref Autorizacion entityAutorizacion,
            ref AvisoSinRespuesta avisoSinRespuesta,
            ref AvisoConRespuesta avisoConRespuesta,
            Autorizacion entityRelacionado,
            string? NoAsuntoAviso,
            string? rfc,
            string promovente,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string? despachos_autorizados,
            string domicilioPromovente,
            string? domicilio_notificaciones,
            DateTime fechaPresentacion,
            DateTime fechaRecepcion,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            int? idSubadministracion,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdministradorEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateStringAlphanumeric(ref promovente!, "Promovente");
            Guard.BooleanValue(promoventeNoContribuyente, "Promovente no es contribuyente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringEmpty(ref domicilioPromovente, "Domicilio promovente");
            Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
            Guard.BooleanValue(noIndidaMonto, "No lo indica");
            Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());
            Guard.CatalogValue(ref idSubadministracion, "Subadministración", false);

            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            entityAutorizacion.rfc = rfc;
            entityAutorizacion.promovente = promovente;
            entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
            entityAutorizacion.rfc_contribuyente = rfcContribuyente;
            entityAutorizacion.contribuyente = contribuyente;
            entityAutorizacion.despachos_autorizados = despachos_autorizados;
            entityAutorizacion.domicilio_notificaciones = domicilio_notificaciones;
            entityAutorizacion.domicilio_promovente = domicilioPromovente;
            entityAutorizacion.fecha_presentacion = fechaPresentacion;
            entityAutorizacion.fecha_recepcion = fechaRecepcion;
            entityAutorizacion.id_tema = idTema;
            entityAutorizacion.otro_tema = otro_tema;
            entityAutorizacion.no_indica_monto = noIndidaMonto;
            entityAutorizacion.monto = monto;
            entityAutorizacion.id_subadministracion = idSubadministracion;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;

            if (entityRelacionado is null && string.IsNullOrEmpty(NoAsuntoAviso))
            {
                if (avisoSinRespuesta is not null && (avisoSinRespuesta.id_autorizacion_relacionado is not null || !string.IsNullOrEmpty(avisoSinRespuesta.no_asunto_externo)))
                {
                    throw new Exception("Ya existe un registro de aviso sin respuesta por lo cual es requerido el número de asunto relacionado");
                }
                else if (avisoConRespuesta is not null && (avisoConRespuesta.id_autorizacion_relacionado is not null || !string.IsNullOrEmpty(avisoConRespuesta.no_asunto_externo)))
                {
                    throw new Exception("Ya existe un registro de aviso con respuesta por lo cual es requerido el número de asunto relacionado");
                }
            }
            if (entityAutorizacion.id_tema.GetValueOrDefault(0) == EnumTema.Aviso_Sin_Respuesta.GetHashCode() ||
                entityAutorizacion.id_tema.GetValueOrDefault(0) == EnumTema.Aviso_Con_Respuesta.GetHashCode())
            {
                if (entityRelacionado is not null)
                {
                    if (entityRelacionado.id == entityAutorizacion.id)
                        throw new Exception("EL número de folio relacionado no debe ser el mismo de la autorozación actual.");

                    if (entityRelacionado.id_estado_procesal != EnumEstadoProcesalCons.Concluido_Notificado)
                        throw new Exception("No se puede asociar la autorización ya que no tiene el estado: Concluido Notificado");

                    if ((entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                        entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS)) &&
                        !entityRelacionado.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS))
                    {
                        throw new Exception("El tipo de asunto de la autorizacion relacionada y la del aviso debe de ser el mismo.");
                    }
                    else if ((entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                        entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS)) &&
                        !entityRelacionado.id_tipo_asunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                    {
                        throw new Exception("El tipo de asunto de la autorizacion relacionada y la del aviso debe de ser el mismo.");
                    }
                }

                switch (entityAutorizacion.id_tema.GetValueOrDefault(0))
                {
                    case EnumTemaConst.Aviso_Sin_Respuesta:
                        if (entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                            entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                            entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                            entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS;
                        else if (entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                            entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                            entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS;

                        if (avisoSinRespuesta is null)
                        {
                            if (avisoConRespuesta is not null)
                            {
                                if (entityAutorizacion.requerimiento is not null || entityAutorizacion.solicitud_opinion is not null)
                                {
                                    throw new Exception("El asunto no puede ser un aviso sin respuesta ya que cuenta con atención");
                                }
                                avisoConRespuesta = null!;
                            }

                            avisoSinRespuesta = new()
                            {
                                id_autorizacion = entityAutorizacion.id,
                                id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id,
                                no_asunto_externo = NoAsuntoAviso,
                                usuario_creacion = usuarioModificacion
                            };
                            //entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Aviso_Pendiente;
                        }
                        else
                        {
                            avisoSinRespuesta.id_autorizacion = entityAutorizacion.id;
                            avisoSinRespuesta.id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id;
                            avisoSinRespuesta.no_asunto_externo = NoAsuntoAviso;
                            avisoSinRespuesta.usuario_modificacion = usuarioModificacion;
                        }
                        break;
                    case EnumTemaConst.Aviso_Con_Respuesta:
                        if (entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                            entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS) ||
                            entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                            entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS;

                        if (avisoConRespuesta is null)
                        {
                            if (avisoSinRespuesta is not null)
                            {
                                if (avisoSinRespuesta.fecha_ingreso is not null)
                                {
                                    throw new Exception("El asunto no puede ser un aviso con respuesta ya que cuenta con atención");
                                }
                                avisoSinRespuesta = null!;
                            }

                            avisoConRespuesta = new()
                            {
                                id_autorizacion = entityAutorizacion.id,
                                id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id,
                                no_asunto_externo = NoAsuntoAviso,
                                usuario_creacion = usuarioModificacion
                            };
                        }
                        else
                        {
                            avisoConRespuesta.id_autorizacion = entityAutorizacion.id;
                            avisoConRespuesta.id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id;
                            avisoConRespuesta.no_asunto_externo = NoAsuntoAviso;
                            avisoConRespuesta.usuario_modificacion = usuarioModificacion;
                        }
                        break;
                }
            }
        }


        public static void UpdateModalidadLinea(
            ref Autorizacion entityAutorizacion,
            ref AvisoSinRespuesta avisoSinRespuesta,
            ref AvisoConRespuesta avisoConRespuesta,
            Autorizacion entityRelacionado,
            string? NoAsuntoAviso,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string? despachos_autorizados,
            string? domicilio_notificaciones,
            DateTime fechaPresentacion,
            DateTime fechaRecepcion,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            int? idSubadministracion,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.BooleanValue(promoventeNoContribuyente, "Promovente no es contribuyente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
            Guard.BooleanValue(noIndidaMonto, "No lo indica");
            Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());
            Guard.CatalogValue(ref idSubadministracion, "Subadministración", false);
            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
            entityAutorizacion.rfc_contribuyente = rfcContribuyente;
            entityAutorizacion.contribuyente = contribuyente;
            entityAutorizacion.despachos_autorizados = despachos_autorizados;
            entityAutorizacion.domicilio_notificaciones = domicilio_notificaciones;
            entityAutorizacion.fecha_presentacion = fechaPresentacion;
            entityAutorizacion.fecha_recepcion = fechaRecepcion;
            entityAutorizacion.id_tema = idTema;
            entityAutorizacion.otro_tema = otro_tema;
            entityAutorizacion.no_indica_monto = noIndidaMonto;
            entityAutorizacion.monto = monto;
            entityAutorizacion.id_subadministracion = idSubadministracion;
            entityAutorizacion.usuario_modificacion = usuarioModificacion;

            if (entityRelacionado is null && string.IsNullOrEmpty(NoAsuntoAviso))
            {
                if (avisoSinRespuesta is not null)
                {
                    throw new Exception("Ya existe un registro de aviso sin respuesta por lo cual es requerido el número de asunto relacionado");
                }
                else if (avisoConRespuesta is not null)
                {
                    throw new Exception("Ya existe un registro de aviso con respuesta por lo cual es requerido el número de asunto relacionado");
                }
            }
            else
            {
                if (entityAutorizacion.id_tema.GetValueOrDefault(0) == EnumTema.Aviso_Sin_Respuesta.GetHashCode() ||
                    entityAutorizacion.id_tema.GetValueOrDefault(0) == EnumTema.Aviso_Con_Respuesta.GetHashCode())
                {
                    if (entityRelacionado is not null)
                    {
                        if (entityRelacionado.id == entityAutorizacion.id)
                            throw new Exception("EL número de folio relacionado no debe ser el mismo de la autorozación actual.");

                        if (entityRelacionado.id_estado_procesal != EnumEstadoProcesalCons.Concluido_Notificado)
                            throw new Exception("No se puede asociar la autorización ya que no tiene el estado: Concluido Notificado");

                        if ((entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                            entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS)) &&
                            !entityRelacionado.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS))
                        {
                            throw new Exception("El tipo de asunto de la autorizacion relacionada y la del aviso debe de ser el mismo.");
                        }
                        else if ((entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                            entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS)) &&
                            !entityRelacionado.id_tipo_asunto.Equals(EnumTipoAsuntoConst.DONATARIAS))
                        {
                            throw new Exception("El tipo de asunto de la autorizacion relacionada y la del aviso debe de ser el mismo.");
                        }
                    }

                    if (entityRelacionado is null && string.IsNullOrEmpty(NoAsuntoAviso))
                        throw new Exception("Debe de indicar el número de folio relacionado.");

                    switch (entityAutorizacion.id_tema.GetValueOrDefault(0))
                    {
                        case EnumTemaConst.Aviso_Sin_Respuesta:
                            if (entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                                entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                                entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_IMPUESTOS_INTERNOS;
                            else if (entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.DONATARIAS) ||
                                entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS))
                                entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_SIN_RESPUESTA_DE_DONATARIAS;

                            if (avisoSinRespuesta is null)
                            {
                                avisoSinRespuesta = new()
                                {
                                    id_autorizacion = entityAutorizacion.id,
                                    id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id,
                                    no_asunto_externo = NoAsuntoAviso,
                                    usuario_creacion = usuarioModificacion
                                };
                                entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.Aviso_Pendiente;
                            }
                            else
                            {
                                avisoSinRespuesta.id_autorizacion = entityAutorizacion.id;
                                avisoSinRespuesta.id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id;
                                avisoSinRespuesta.no_asunto_externo = NoAsuntoAviso;
                                avisoSinRespuesta.usuario_modificacion = usuarioModificacion;
                            }
                            break;
                        case EnumTemaConst.Aviso_Con_Respuesta:
                            if (entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_IMPUESTOS_INTERNOS) ||
                                entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS))
                                entityAutorizacion.id_tipo_asunto = EnumTipoAsuntoConst.AVISO_CON_RESPUESTA_DE_IMPUESTOS_INTERNOS;

                            if (avisoConRespuesta is null)
                            {
                                avisoConRespuesta = new()
                                {
                                    id_autorizacion = entityAutorizacion.id,
                                    id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id,
                                    no_asunto_externo = NoAsuntoAviso,
                                    usuario_creacion = usuarioModificacion
                                };
                            }
                            else
                            {
                                avisoConRespuesta.id_autorizacion = entityAutorizacion.id;
                                avisoConRespuesta.id_autorizacion_relacionado = entityRelacionado is null ? null! : entityRelacionado.id;
                                avisoConRespuesta.no_asunto_externo = NoAsuntoAviso;
                                avisoConRespuesta.usuario_modificacion = usuarioModificacion;
                            }
                            break;
                    }

                }
            }
        }

        public static void UpdateAsignar(ref Autorizacion entity,
          string noEmpleado,
          string usuarioMidificacion
        )
        {
            ValidacionEstadosAdministradorEvents.ValidateAutorizacionAsignar(true, entity.id_estado_procesal, true, entity.id_estado_tarea);

            if (entity.id_subadministracion is null || entity.id_subadministracion <= 0)
                throw new Exception("Aún no se ha asignado la unidad administrativa.");

            entity.id_estado_tarea = EnumEstadoTarea.Asignado.GetHashCode();
            entity.id_estado_procesal = EnumEstadoProcesal.En_estudio.GetHashCode();
            entity.no_empleado_turna = noEmpleado;
            entity.usuario_modificacion = usuarioMidificacion;
        }

        public static void UpdateRemitir(ref Autorizacion entity,
            Remision remision,
            string noEmpleado,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdministradorEvents.ValidateRemision(true, entity.id_estado_procesal, true, entity.id_estado_tarea);
            if (remision.id_tipo_autoridad == EnumTipoAutoridad.Interna.GetHashCode())
            {
                entity.id_estado_tarea = EnumEstadoTarea.Pendiente_de_turnar.GetHashCode();
                entity.id_estado_procesal = EnumEstadoProcesal.Remitido.GetHashCode();
            }
            else if (remision.id_tipo_autoridad == EnumTipoAutoridad.Externa.GetHashCode())
            {
                entity.id_estado_tarea = EnumEstadoTarea.Concluido_Remitido.GetHashCode();
                entity.id_estado_procesal = EnumEstadoProcesal.Concluido_Remitido.GetHashCode();
            }
            entity.usuario_modificacion = usuarioModificacion;
        }
    }
}