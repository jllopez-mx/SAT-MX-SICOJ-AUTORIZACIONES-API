namespace AutorizacionesAPI.Model.ViewModels.Enums
{
    public class EnumFunctions
    {
        public const string AutorizacionByIds = "sicoj_autorizaciones.fn_autorizacion_by_ids";
        public const string AutorizacionByNumerosAsunto = "sicoj_autorizaciones.fn_autorizacion_by_numeros_asunto";
        public const string AutorizacionUpdate = "sicoj_autorizaciones.fn_autorizacion_update";
        public const string AutorizacionByIdDisconnected = "sicoj_autorizaciones.fn_autorizacion_by_id_disconnected";

        #region Oficial de Partes
        public const string OpBandejaPendientes = "sicoj_autorizaciones.fn_op_bandeja_pendientes";
        public const string OpBandejaPendientesCount = "sicoj_autorizaciones.fn_op_bandeja_pendientes_count";

        public const string OpHistorico = "sicoj_autorizaciones.fn_op_historico";
        public const string OpHistoricoCount = "sicoj_autorizaciones.fn_op_historico_count";

        public const string OpAutorizacionCreate = "sicoj_autorizaciones.fn_op_autorizacion_create";
        public const string OpAutorizacionUpdate = "sicoj_autorizaciones.fn_op_autorizacion_update";
        public const string OpAutorizacionDelete = "sicoj_autorizaciones.fn_op_autorizacion_delete";
        public const string OpAutorizacionTurnar = "sicoj_autorizaciones.fn_op_autorizacion_turnar_update";

        public const string OpCumplimentacionCreate = "sicoj_autorizaciones.fn_op_cumplimentacion_create";
        public const string OpCumplimentacionUpdate = "sicoj_autorizaciones.fn_op_cumplimentacion_update";
        public const string OpCumplimentacionByIdDisconnected = "sicoj_autorizaciones.fn_op_cumplimentacion_by_id_disconnected";
        public const string OpBuscarCumplimentacionNumeroAsunto = "sicoj_autorizaciones.fn_op_buscar_cumplimentacion_numero_asunto";
        public const string OpCumplimentacionById = "sicoj_autorizaciones.fn_op_cumplimentacion_by_id";
        public const string OpTurnarCumplimentacion = "sicoj_autorizaciones.fn_op_cumplimentacion_turnar_update";
       
        #endregion

        #region Documentos
        public const string DocumentosCreate = "sicoj_autorizaciones.fn_documentos_create";
        public const string DocumentosUpdate = "sicoj_autorizaciones.fn_documentos_update";
        public const string DocumentosByIds = "sicoj_autorizaciones.fn_documentos_by_ids";
        public const string DocumentosFolio = "sicoj_autorizaciones.fn_documentos_folio";
        public const string DocumentosFolioCount = "sicoj_autorizaciones.fn_documentos_folio_count";
        public const string DocumentosDelete = "sicoj_autorizaciones.fn_documentos_delete";
        public const string DocumentosByRenglonTipo = "sicoj_autorizaciones.fn_documentos_by_id_renglon_tipo";
        #endregion

        #region Administrador
        public const string AdminAutorizacionBandejaPendientes = "sicoj_autorizaciones.fn_admin_bandeja_pendientes";
        public const string AdminAutorizacionBandejaPendientesCount = "sicoj_autorizaciones.fn_admin_bandeja_pendientes_count";

        public const string AdminAbogadoAsignado = "sicoj_autorizaciones.fn_admin_abogado_asignado";
        public const string AdminAsignarAbogado = "sicoj_autorizaciones.fn_admin_asignar_abogado";
        public const string AdminRemision = "sicoj_autorizaciones.fn_admin_remision_create";

        public const string AdminHistorico = "sicoj_autorizaciones.fn_admin_historico";
        public const string AdminHistoricoCount = "sicoj_autorizaciones.fn_admin_historico_count";

        public const string AdminAvisoSinRespuestaRechazar = "sicoj_autorizaciones.fn_admin_aviso_sin_respuesta_rechazar";
        public const string AdminAvisoSinRespuestaAprobar = "sicoj_autorizaciones.fn_admin_aviso_sin_respuesta_aprobar";

        #endregion

        #region Abogado
        public const string AbogadoBandejaPendientes = "sicoj_autorizaciones.fn_abo_pendientes";
        public const string AbogadoBandejaPendientesCount = "sicoj_autorizaciones.fn_abo_pendientes_count";

        public const string AbogadoHistorico = "sicoj_autorizaciones.fn_abo_historico";
        public const string AbogadoHistoricoCount = "sicoj_autorizaciones.fn_abo_historico_count";

        #endregion

        #region Administrador Global
        public const string AgHistorico = "sicoj_autorizaciones.fn_ag_historico";
        public const string AgHistoricoCount = "sicoj_autorizaciones.fn_ag_historico_count";
       
        public const string AgAutorizacionByIdDisconnected = "sicoj_autorizaciones.fn_ag_autorizacion_by_id_disconnected";
        public const string AgDescartarByIdAsunto = "sicoj_autorizaciones.fn_ag_descartar_by_id_asunto";
        public const string AgDescartar = "sicoj_autorizaciones.fn_ag_descartar";
        public const string AgCumplimentacionDescartarPorImprocedencia = "sicoj_autorizaciones.fn_ag_cumplimentacion_descartar_por_improcedencia";
        public const string AgCumplimentacionDescartarAutorizacion = "sicoj_autorizaciones.fn_ag_cumplimentacion_descartar_autorizacion";
        public const string AgReactivar = "sicoj_autorizaciones.fn_ag_reactivar_update";

        public const string AgRequerimientoDescartarUltimo = "sicoj_autorizaciones.fn_ag_requerimiento_descartar_ultimo";
        public const string AgRequerimientoDescartar = "sicoj_autorizaciones.fn_ag_requerimiento_descartar";
        public const string AgResolucionDescartar = "sicoj_autorizaciones.fn_ag_resolucion_descartar";
       
        #endregion

        #region Administrador Unidad Central
        public const string AdminUnidadCentralHistorico = "sicoj_autorizaciones.fn_aua_historico";
        public const string AdminUnidadCentralHistoricoCount = "sicoj_autorizaciones.fn_aua_historico_count";
        #endregion

        #region Genericos
        public const string GenAvisosComunicadosById = "sicoj_autorizaciones.fn_gen_avisos_comunicados_by_id";
        public const string GenAvisosComunicadosByIdDisconnected = "sicoj_autorizaciones.fn_gen_avisos_comunicados_disconnected_by_id";
        public const string GenAvisosComunicadosDisconnected = "sicoj_autorizaciones.fn_gen_avisos_comunicados_disconnected";
        public const string GenAvisosComunicadosCreate = "sicoj_autorizaciones.fn_gen_avisos_comunicados_create";
        public const string GenAvisosComunicadosUpdate = "sicoj_autorizaciones.fn_gen_avisos_comunicados_update";

        public const string GenCumplimentacionByIdDisconnected = "sicoj_autorizaciones.fn_gen_cumplimentacion_by_id_disconnected";
        public const string GenCumplimentacionUpdate = "sicoj_autorizaciones.fn_gen_cumplimentacion_update";

        public const string GenModificacionCreate = "sicoj_autorizaciones.fn_gen_modificacion_create";
        public const string GenModificacionByIdAsunto = "sicoj_autorizaciones.fn_gen_modificacion_by_id_asunto";
        public const string GenReasignar = "sicoj_autorizaciones.fn_gen_reasignar_create";
        public const string GenConcluir = "sicoj_autorizaciones.fn_gen_concluir_update";

        public const string GenPersonasAutorizadasCreate = "sicoj_autorizaciones.fn_gen_personas_autorizadas_create";
        public const string GenPersonasAutorizadasUpdate = "sicoj_autorizaciones.fn_gen_personas_autorizadas_update";
        public const string GenPersonasAutorizadsDelete = "sicoj_autorizaciones.fn_gen_personas_autorizadas_delete";
        public const string GenPersonasAutorizadasByIds = "sicoj_autorizaciones.fn_gen_personas_autorizadas_by_ids";
        public const string GenPersonasAutorizadasByIdDisconnected = "sicoj_autorizaciones.fn_gen_personas_autorizadas_by_id_disconnected";

        public const string GenRequerimientoProdeconCreate = "sicoj_autorizaciones.fn_gen_requerimiento_prodecon_create";
        public const string GenRequerimientoProdeconUpdate = "sicoj_autorizaciones.fn_gen_requerimiento_prodecon_update";
        public const string GenRequerimientoProdeconDisconnectedList = "sicoj_autorizaciones.fn_gen_requerimiento_prodecon_disconnected";
        public const string GenRequerimientoProdeconByIdDisconnected = "sicoj_autorizaciones.fn_gen_requerimiento_prodecon_by_id_disconnected";
        public const string GenRequerimientoProdeconByIdAutorizacionCount = "sicoj_autorizaciones.fn_gen_requerimiento_prodecon_count";
        public const string GenRequerimientoProdeconById = "sicoj_autorizaciones.fn_gen_requerimiento_prodecon_by_id";

        public const string GenSolicitudOpinionInformacionCreate = "sicoj_autorizaciones.fn_gen_solicitud_opinion_informacion_create";
        public const string GenSolicitudOpinionInformacionUpdate = "sicoj_autorizaciones.fn_gen_solicitud_opinion_informacion_update";
        public const string GenSolicitudOpinionInformacionByIds = "sicoj_autorizaciones.fn_gen_solicitud_opinion_informacion_by_ids";
        public const string GenSolicitudOpinionInformacionByIdDisconnected = "sicoj_autorizaciones.fn_gen_solicitud_opinion_informacion_disconnected";
        public const string GenNoSolicitudOpinionInformacion = "sicoj_autorizaciones.fn_gen_no_solicitud_opinion_informacion";

        public const string GenAvisoConRespuetaDisconnected = "sicoj_autorizaciones.fn_gen_aviso_con_respuesta_disconnected";
        public const string GenAvisoConRespuetaById = "sicoj_autorizaciones.fn_gen_aviso_con_respuesta_by_id";
        public const string GenAvisoConRespuestaByIdAutorizacion = "sicoj_autorizaciones.fn_gen_aviso_con_respuesta_by_id_autorizacion";

        public const string GenAvisoSinRespuestaUpdate = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_update";
        public const string GenAvisoSinRespuestaCreate = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_create";
        public const string GenAvisoSinRespuestaDisconnectedById = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_disconnected_by_id";
        public const string GenAvisoSinRespuestaDisconnectedByIdAutorizacion = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_disconnected_by_id_autorizacion";
        public const string GenAvisoSinRespuestaDisconnectedByIdAsunto = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_disconnected_by_id_asunto";
        public const string GenAvisoSinRespuestaById = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_by_id";
        public const string GenAvisoSinRespuestaByIdAutorizacion = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_by_id_autorizacion";
        public const string GenAvisoSinRespuestaHistoricoDisconnected = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_historico_disconnected";
        public const string GenAvisoSinRespuestaFolios = "sicoj_autorizaciones.fn_gen_aviso_sin_respuesta_folios";
        public const string GenAutorizacionNumeroAsunto = "sicoj_autorizaciones.fn_gen_autorizacion_numero_asunto";
        public const string GenAutorizacionBuscarNumeroAsunto = "sicoj_autorizaciones.fn_gen_autorizacion_buscar_numero_asunto";

        public const string GenRequerimientoById = "sicoj_autorizaciones.fn_gen_requerimiento_by_id";
        public const string GenRequerimientoByIdAutorizacion = "sicoj_autorizaciones.fn_gen_requerimiento_by_id_autorizacion";
        public const string GenRequerimientoByIdDisconnected = "sicoj_autorizaciones.fn_gen_requerimiento_by_id_disconnected";
        public const string GenRequerimientoDisconnected = "sicoj_autorizaciones.fn_gen_requerimiento_disconnected";
        public const string GenRequerimientoCreate = "sicoj_autorizaciones.fn_gen_requerimiento_create";
        public const string GenRequerimientoUpdate = "sicoj_autorizaciones.fn_gen_requerimiento_update";
        public const string GenNoRequerimiento = "sicoj_autorizaciones.fn_gen_no_requerimiento";

        public const string GenResolucionCreate = "sicoj_autorizaciones.fn_gen_resolucion_create";
        public const string GenResolucionUpdate = "sicoj_autorizaciones.fn_gen_resolucion_update";
        public const string GenCumplimentacionImprocedenciaUpdate = "sicoj_autorizaciones.fn_gen_cumplimentacion_improcedencia_update";
        public const string GenResolucionByIdAsunto = "sicoj_autorizaciones.fn_gen_resolucion_by_id_asunto";
        public const string GenResolucionByIdAsuntoDisconnected = "sicoj_autorizaciones.fn_gen_resolucion_by_id_asunto_disconnected";

        public const string GenResolucionCumplimentacionDisconnectedById = "sicoj_autorizaciones.fn_gen_resolucion_cumplimentacion_by_id_disconnected";
        public const string GenResolucionCumplimentacionDisconnected = "sicoj_autorizaciones.fn_gen_resolucion_cumplimentacion_disconnected";

        public const string GenRemisionDisconnected = "sicoj_autorizaciones.fn_gen_remision_disconnected";
        public const string GenRemisionDisconnectedCount = "sicoj_autorizaciones.fn_gen_remision_disconnected_count";

        public const string GenMediosDefensaDisconnected = "sicoj_autorizaciones.fn_gen_medios_defensa_by_disconnected";
        #endregion
    }
}
