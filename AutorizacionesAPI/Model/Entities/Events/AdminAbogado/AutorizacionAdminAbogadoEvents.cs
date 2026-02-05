using AutorizacionesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;
using System.Net;
using System.Runtime.ConstrainedExecution;

namespace AutorizacionesAPI.Model.Entities.Events.AdminAbogado
{
    public class AutorizacionAdminAbogadoEvents
    {
        public static void Update(
            ref Autorizacion entityAutorizacion,
            ref AvisoSinRespuesta avisoSinRespuesta,
            ref AvisoConRespuesta avisoConRespuesta,
            Autorizacion entityRelacionado,
            string? NoAsuntoAviso,
            ref Modificacion modificacion,
            ref Descartar descartar,
            int? idTipoAutorizacion,
            string? rfc,
            string? promovente,
            bool promoventeNoContribuyente,
            string? rfcContribuyente,
            string? contribuyente,
            string? despachosAutorizados,
            int? idAutoridadDirigida,
            string? autoridadDirigida,
            string? domicilioNotificaciones,
            DateTime fechaPresentacion,
            int? idFundamentoSolicitud,
            string? fundamentoSolicitud,
            int? idTema,
            string? otro_tema,
            bool? noIndidaMonto,
            decimal? monto,
            DateTime fechaRecepcion,
            string usuarioModificacion
        )
        {
            ValidacionEstadosAdminAbogadoEvents.ValidateAutorizacionUpdate(true, entityAutorizacion.id_estado_procesal, true, entityAutorizacion.id_estado_tarea);

            Guard.ValidateStringRfc(ref rfc!, "RFC", false);
            Guard.ValidateStringAlphanumeric(ref promovente!, "Promovente");
            Guard.ValidateStringRfc(ref rfcContribuyente, "RFC contribuyente", promoventeNoContribuyente);
            Guard.ValidateStringAlphanumeric(ref contribuyente, "Contribuyente", promoventeNoContribuyente);
            if (entityAutorizacion.id_tipo_asunto.Equals(EnumTipoAsuntoConst.AUTORIZACION_DE_COMERCIO_EXTERIOR))
            {
                Guard.CatalogValue(ref idTipoAutorizacion, "Tipo de autorización");
                Guard.CatalogOtherValue(ref idAutoridadDirigida, ref autoridadDirigida, "Autoridad dirigida", EnumAutoridadDirigida.OTRO.GetHashCode());
            }
            Guard.ValidateStringAlphanumeric(ref despachosAutorizados!, "Despacho o Autorizados");
            Guard.ValidateStringAlphanumeric(ref domicilioNotificaciones, "Domicilio notificaciones", false);
            Guard.CatalogOtherValue(ref idFundamentoSolicitud, ref fundamentoSolicitud, "Fundamento solicitud", EnumFundamentoSolicitud.OTRO.GetHashCode(), false);
            Guard.CatalogOtherValue(ref idTema, ref otro_tema, "otro_tema", EnumTema.Otro.GetHashCode(), false);
            Guard.BooleanValue(noIndidaMonto, "No lo indica");
            Guard.ValidateDecimal(monto, "Monto", true, !noIndidaMonto.GetValueOrDefault());
            Guard.ValidateDate(fechaPresentacion, "Fecha de presentación");

            entityAutorizacion.id_tipo_autorizacion = idTipoAutorizacion.GetValueOrDefault();
            entityAutorizacion.rfc = rfc;
            entityAutorizacion.promovente = promovente.Trim();
            entityAutorizacion.promovente_no_contribuyente = promoventeNoContribuyente;
            entityAutorizacion.rfc_contribuyente = rfcContribuyente;
            entityAutorizacion.contribuyente = contribuyente;
            entityAutorizacion.despachos_autorizados = despachosAutorizados.Trim();
            entityAutorizacion.id_autoridad_dirigida = idAutoridadDirigida;
            entityAutorizacion.otra_autoridad_dirigida = autoridadDirigida;
            entityAutorizacion.domicilio_notificaciones = domicilioNotificaciones;
            entityAutorizacion.fecha_presentacion = fechaPresentacion;
            entityAutorizacion.id_fundamento_solicitud = idFundamentoSolicitud;
            entityAutorizacion.otro_fundamento_solicitud = fundamentoSolicitud;
            entityAutorizacion.id_tema = idTema;
            entityAutorizacion.otro_tema = otro_tema;
            entityAutorizacion.no_indica_monto = noIndidaMonto;
            entityAutorizacion.monto = monto;
            entityAutorizacion.fecha_recepcion = fechaRecepcion;
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


            if (entityAutorizacion.id_estado_procesal.Equals(EnumEstadoProcesalCons.En_Reparacion))
            {
                if (modificacion != null)
                {
                    entityAutorizacion.id_estado_procesal = modificacion.id_estado_procesal;
                }
                else if (descartar != null)
                {
                    //entityAutorizacion.id_estado_procesal = descartar.id_estado_procesal;
                    entityAutorizacion.id_estado_procesal = EnumEstadoProcesalCons.En_estudio;
                }
                else if (modificacion == null)
                {
                    throw new Exception("No existe el registro de modificación");
                }
                else if (descartar == null)
                {
                    throw new Exception("No existe el registro de descarte");
                }
            }

        }
    }
}
