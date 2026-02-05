using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;
using Sicoj.Utils.ViewModels;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class PerfilAbogadoDisconnectedRepository : IPerfilAbogadoDisconnectedRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion  

        #region Constructor
        public PerfilAbogadoDisconnectedRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        #region Bandeja de Pendientes
        public async Task<int?> GetBandejaPendientesCountAsync(
            string? noAsunto,
            DateTime? fechaPresentacionDesde,
            DateTime? fechaPresentacionHasta,
            DateTime? fechaVencimientoDesde,
            DateTime? fechaVencimientoHasta,
            string? rfc,
            string? promovente,
            List<int>? temaList,
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            int? idUnidadAdmistrativaCentral = null!,
            int? idUnidadAdmistrativa = null!,
            int? idSubadministracion = null!,
            string? idAbogado = null!)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ("p_fecha_presentacion_desde", NpgsqlDbType.Date, fechaPresentacionDesde),
                new ("p_fecha_presentacion_hasta", NpgsqlDbType.Date, fechaPresentacionHasta),
                new ("p_fecha_vencimiento_desde", NpgsqlDbType.Date, fechaVencimientoDesde),
                new ("p_fecha_vencimiento_hasta", NpgsqlDbType.Date, fechaVencimientoHasta),
                new ("p_rfc", NpgsqlDbType.Varchar, rfc),
                new ("p_promovente", NpgsqlDbType.Text, promovente),
                new ("p_id_tema", NpgsqlDbType.Array | NpgsqlDbType.Integer, (temaList is null || !temaList.Any()) ? DBNull.Value :  temaList.ToArray()),
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoAsuntoList is null || !tipoAsuntoList.Any()) ? DBNull.Value :  tipoAsuntoList.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoTareaList is null || !estadoTareaList.Any()) ? DBNull.Value :  estadoTareaList.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoModalidadList is null || !tipoModalidadList.Any()) ? DBNull.Value :  tipoModalidadList.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoProcesalList is null || !estadoProcesalList.Any()) ? DBNull.Value :  estadoProcesalList.ToArray()),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, idUnidadAdmistrativa),
                new ("p_id_subadministracion", NpgsqlDbType.Integer, idSubadministracion),
                new ("p_id_abogado", NpgsqlDbType.Varchar, idAbogado),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AbogadoBandejaPendientesCount,
                parameters
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return response.Data.Tables[0].Rows[0].IsNull(0)
                ? null!
                : response.Data.Tables[0].Rows[0].Field<int>(0);
        }

        public async Task<List<ResponseAutorizacionesAbogadoBandeja>> GetBandejaPendientesAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string? noAsunto,
            DateTime? fechaPresentacionDesde,
            DateTime? fechaPresentacionHasta,
            DateTime? fechaVencimientoDesde,
            DateTime? fechaVencimientoHasta,
            string? rfc,
            string? promovente,
            List<int>? temaList,
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            int? idUnidadAdmistrativaCentral = null!,
            int? idUnidadAdmistrativa = null!,
            int? idSubadministracion = null!,
            string? idAbogado = null!
        )
        {
            ParameterPGsql[] parameters =
            {
                new ("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ("p_page", NpgsqlDbType.Integer, page),
                new ("p_order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ("p_fecha_presentacion_desde", NpgsqlDbType.Date, fechaPresentacionDesde),
                new ("p_fecha_presentacion_hasta", NpgsqlDbType.Date, fechaPresentacionHasta),
                new ("p_fecha_vencimiento_desde", NpgsqlDbType.Date, fechaVencimientoDesde),
                new ("p_fecha_vencimiento_hasta", NpgsqlDbType.Date, fechaVencimientoHasta),
                new ("p_rfc", NpgsqlDbType.Varchar, rfc),
                new ("p_promovente", NpgsqlDbType.Text, promovente),
                new ("p_id_tema", NpgsqlDbType.Array | NpgsqlDbType.Integer, (temaList is null || !temaList.Any()) ? DBNull.Value :  temaList.ToArray()),
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoAsuntoList is null || !tipoAsuntoList.Any()) ? DBNull.Value :  tipoAsuntoList.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoTareaList is null || !estadoTareaList.Any()) ? DBNull.Value :  estadoTareaList.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoModalidadList is null || !tipoModalidadList.Any()) ? DBNull.Value :  tipoModalidadList.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoProcesalList is null || !estadoProcesalList.Any()) ? DBNull.Value :  estadoProcesalList.ToArray()),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, idUnidadAdmistrativa),
                new ("p_id_subadministracion", NpgsqlDbType.Integer, idSubadministracion),
                new ("p_id_abogado", NpgsqlDbType.Varchar, idAbogado),
                new ("p_activo", NpgsqlDbType.Boolean, true),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AbogadoBandejaPendientes, parameters
            );

            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseAutorizacionesAbogadoBandeja> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        noAsunto = item.IsNull(1) ? null! : item.Field<string>(1)!,
                        fechaPresentacion = item.IsNull(2) ? null! : item.Field<DateTime>(2).ToString("dd/MM/yyyy"),
                        fechaVencimiento = item.IsNull(3) ? null! : item.Field<DateTime>(3).ToString("dd/MM/yyyy"),
                        rfc = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        promovente = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        idTipoAsunto = item.IsNull(6) ? 0 : item.Field<int>(6),
                        tipoAsunto = item.IsNull(7) ? null! : item.Field<string>(7),
                        idTipoAsuntoCumplimentacion = item.IsNull(8) ? 0 : item.Field<int>(8),
                        tipoAsuntoCumplimentacion = item.IsNull(9) ? null! : item.Field<string>(9),
                        idTipoModalidad = item.IsNull(10) ? 0 : item.Field<int>(10),
                        tipoModalidad = item.IsNull(11) ? null! : item.Field<string>(11),
                        idTema = item.IsNull(12) ? 0 : item.Field<int>(12),
                        tema = item.IsNull(13) ? null! : item.Field<string>(13),
                        idEstadoProcesal = item.IsNull(14) ? 0 : item.Field<int>(14),
                        estadoProcesal = item.IsNull(15) ? null! : item.Field<string>(15),
                        idEstadoTarea = item.IsNull(16) ? 0 : item.Field<int>(16),
                        estadoTarea = item.IsNull(17) ? null! : item.Field<string>(17),
                        idUnidadAdministrativa = item.IsNull(18) ? 0 : item.Field<int>(18),
                        unidadAdministrativa = item.IsNull(19) ? null! : item.Field<string>(19),
                        idSubadministracion = item.IsNull(20) ? 0 : item.Field<int>(20),
                        subadministracion = item.IsNull(21) ? null! : item.Field<string>(21),
                        idAbogado = item.IsNull(22) ? null! : item.Field<string>(22),
                        abogado = item.IsNull(23) ? null! : item.Field<string>(23),
                        idAlerta = item.IsNull(24) ? 0 : item.Field<int>(24),
                        alerta = item.IsNull(24) ? null : ((EnumAlerta)item.Field<int>(24)).ToString(),
                    }
                );
            }
            return resultList;
        }
        #endregion

        #region Historico
        public async Task<int?> GetHistoricoCountAsync(
          string? noAsunto,
          DateTime? fechaPresentacion,
          DateTime? fechaVencimiento,
          string? rfc,
          string? promovente,
          List<int>? tipoAsuntoList,
          List<int>? estadoTareaList,
          List<int>? tipoModalidadList,
          List<int>? estadoProcesalList,
          List<int>? alertaList,
          int? idUnidadAdmistrativaCentral = null!,
          int? idUnidadAdmistrativa = null!
      )
        {
            ParameterPGsql[] parameters =
            {
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ("p_fecha_recepcion_desde", NpgsqlDbType.Date, fechaPresentacion),
                new ("p_fecha_recepcion_hasta", NpgsqlDbType.Date, fechaVencimiento),
                new ("p_rfc", NpgsqlDbType.Varchar, rfc),
                new ("p_promovente", NpgsqlDbType.Text, promovente),
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoAsuntoList is null || !tipoAsuntoList.Any()) ? DBNull.Value :  tipoAsuntoList.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoTareaList is null || !estadoTareaList.Any()) ? DBNull.Value :  estadoTareaList.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoModalidadList is null || !tipoModalidadList.Any()) ? DBNull.Value :  tipoModalidadList.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoProcesalList is null || !estadoProcesalList.Any()) ? DBNull.Value :  estadoProcesalList.ToArray()),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, idUnidadAdmistrativa),
            };

            var response = await _database.ExecuteFunctionAsync(
                    EnumFunctions.AbogadoHistoricoCount,
                    parameters!
                );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return response.Data.Tables[0].Rows[0].IsNull(0)
                ? null!
                : response.Data.Tables[0].Rows[0].Field<int>(0);
        }

        public async Task<List<ResponseAutorizacionesAbogadoHistorico>> GetHistoricoAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string? noAsunto,
            DateTime? fechaPresentacion,
            DateTime? fechaVencimiento,
            string? rfc,
            string? promovente,
            List<int>? tipoAsuntoList,
            List<int>? estadoTareaList,
            List<int>? tipoModalidadList,
            List<int>? estadoProcesalList,
            List<int>? alertaList,
            int? idUnidadAdmistrativaCentral = null!,
            int? idUnidadAdmistrativa = null!
        )
        {
            ParameterPGsql[] parameters =
            {
                new ("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ("p_page", NpgsqlDbType.Integer, page),
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ("p_fecha_recepcion_desde", NpgsqlDbType.Date, fechaPresentacion),
                new ("p_fecha_recepcion_hasta", NpgsqlDbType.Date, fechaVencimiento),
                new ("p_rfc", NpgsqlDbType.Varchar, rfc),
                new ("p_promovente", NpgsqlDbType.Text, promovente),
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoAsuntoList is null || !tipoAsuntoList.Any()) ? DBNull.Value :  tipoAsuntoList.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoTareaList is null || !estadoTareaList.Any()) ? DBNull.Value :  estadoTareaList.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (tipoModalidadList is null || !tipoModalidadList.Any()) ? DBNull.Value :  tipoModalidadList.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (estadoProcesalList is null || !estadoProcesalList.Any()) ? DBNull.Value :  estadoProcesalList.ToArray()),
                new ("p_order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, idUnidadAdmistrativa),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AbogadoHistorico,
                parameters!
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseAutorizacionesAbogadoHistorico> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        noAsunto = item.IsNull(1) ? null! : item.Field<string>(1)!,
                        fechaPresentacion = item.IsNull(2) ? null! : item.Field<DateTime>(2).ToString("dd/MM/yyyy"),
                        fechaVencimiento = item.IsNull(3) ? null! : item.Field<DateTime>(3).ToString("dd/MM/yyyy"),
                        rfc = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        promovente = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        idTipoAsunto = item.IsNull(6) ? 0 : item.Field<int>(6),
                        tipoAsunto = item.IsNull(7) ? null! : item.Field<string>(7),
                        idTipoAsuntoCumplimentacion = item.IsNull(8) ? 0 : item.Field<int>(8),
                        tipoAsuntoCumplimentacion = item.IsNull(9) ? null! : item.Field<string>(9),
                        idTipoModalidad = item.IsNull(10) ? 0 : item.Field<int>(10),
                        tipoModalidad = item.IsNull(11) ? null! : item.Field<string>(11),
                        idTema = item.IsNull(12) ? 0 : item.Field<int>(12),
                        tema = item.IsNull(13) ? null! : item.Field<string>(13),
                        idEstadoProcesal = item.IsNull(14) ? 0 : item.Field<int>(14),
                        estadoProcesal = item.IsNull(15) ? null! : item.Field<string>(15),
                        idEstadoTarea = item.IsNull(16) ? 0 : item.Field<int>(16),
                        estadoTarea = item.IsNull(17) ? null! : item.Field<string>(17),
                        idUnidadAdministrativa = item.IsNull(18) ? 0 : item.Field<int>(18),
                        unidadAdministrativa = item.IsNull(19) ? null! : item.Field<string>(19),
                        idSubadministracion = item.IsNull(20) ? 0 : item.Field<int>(20),
                        subadministracion = item.IsNull(21) ? null! : item.Field<string>(21),
                        idAbogado = item.IsNull(22) ? null! : item.Field<string>(22),
                        abogado = item.IsNull(23) ? null! : item.Field<string>(23),
                        idAlerta = item.IsNull(24) ? 0 : item.Field<int>(24),
                        alerta = item.IsNull(24) ? null : ((EnumAlerta)item.Field<int>(24)).ToString(),
                    }
                );
            }
            return resultList;
        }
        #endregion
    }
}
