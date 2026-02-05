using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class PerfilOficialPartesDisconnectedRepository : IPerfilOficialPartesDisconnectedRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public PerfilOficialPartesDisconnectedRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        #region Métodos

        #region Bandeja Pendientes
        public async Task<List<ResponseAutorizacionesOficialPartesBandeja>> GetBandejaPendientesAsync(
            int pageSize, int page, string? orderByColumn, bool orderDesc,
            List<int>? TipoAsunto, int? idUnidadAdmistrativaCentral = null!
        )
        {
            ParameterPGsql[] parameters =
            {
                new ("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ("p_page", NpgsqlDbType.Integer, page),
                new ("p_order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoAsunto is null || !TipoAsunto.Any()) ? DBNull.Value :  TipoAsunto.ToArray()),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpBandejaPendientes,
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

            List<ResponseAutorizacionesOficialPartesBandeja> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        noAsunto = item.IsNull(1) ? null! : item.Field<string?>(1),
                        idTipoAsunto = item.IsNull(2) ? 0 : item.Field<int>(2),
                        tipoAsunto = item.IsNull(3) ? null! : item.Field<string>(3),
                        idTipoAsuntoCumplimentacion = item.IsNull(4) ? 0 : item.Field<int>(4),
                        tipoAsuntoCumplimentacion = item.IsNull(5) ? null! : item.Field<string>(5), // Mapear tipo asunto de cumplimentación
                        idTipoEntrada = item.IsNull(6) ? 0 : item.Field<int>(6),
                        tipoEntrada = item.IsNull(7) ? null! : item.Field<string>(7),
                        rfc = item.IsNull(8) ? null! : item.Field<string>(8)!,
                        promovente = item.IsNull(9) ? null! : item.Field<string>(9)!,
                        fechaRecepcion = item.IsNull(10) ? null!: item.Field<DateTime>(10).ToString("dd/MM/yyyy"),
                        idEstadoProcesal = item.IsNull(11) ? 0 : item.Field<int>(11),
                        estadoProcesal = item.IsNull(12) ? null! : item.Field<string>(12),
                        idEstadoTarea = item.IsNull(13) ? 0 : item.Field<int>(13),
                        estadoTarea = item.IsNull(14) ? null! : item.Field<string>(14),
                    }
                );
            }

            return resultList;
        }

        public async Task<int?> GetBandejaPendientesCountAsync(List<int>? TipoAsunto, int? idUnidadAdmistrativaCentral = null!)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoAsunto is null || !TipoAsunto.Any()) ? DBNull.Value :  TipoAsunto.ToArray()),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpBandejaPendientesCount,
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
        #endregion

        #region Historico
        public async Task<List<ResponseAutorizacionesOficialPartesHistorico>> GetHistoricoAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string? noAsunto,
            DateTime? desdeRecepcion,
            DateTime? hastaRecepcion,
            string? rfc,
            string? promovente,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativaCentral = null!
        )
        {
            ParameterPGsql[] parameters =
            {
                new ("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ("p_page", NpgsqlDbType.Integer, page),
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ("p_fecha_recepcion_desde", NpgsqlDbType.Date, desdeRecepcion),
                new ("p_fecha_recepcion_hasta", NpgsqlDbType.Date, hastaRecepcion),
                new ("p_rfc", NpgsqlDbType.Varchar, rfc),
                new ("p_promovente", NpgsqlDbType.Text, promovente),
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoAsunto is null || !TipoAsunto.Any()) ? DBNull.Value :  TipoAsunto.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoTarea is null || !EstadoTarea.Any()) ? DBNull.Value :  EstadoTarea.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoModalidad is null || !TipoModalidad.Any()) ? DBNull.Value :  TipoModalidad.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoProcesal is null || !EstadoProcesal.Any()) ? DBNull.Value :  EstadoProcesal.ToArray()),
                new ("p_order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpHistorico,
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

            List<ResponseAutorizacionesOficialPartesHistorico> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        noAsunto = item.IsNull(1) ? null! : item.Field<string?>(1),
                        idTipoAsunto = item.IsNull(2) ? 0 : item.Field<int>(2),
                        tipoAsunto = item.IsNull(3) ? null! : item.Field<string>(3),
                        idTipoEntrada = item.IsNull(4) ? 0 : item.Field<int>(4),
                        tipoEntrada = item.IsNull(5) ? null! : item.Field<string>(5),
                        rfc = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        promovente = item.IsNull(7) ? null! : item.Field<string>(7)!,
                        fecha_recepcion = item.IsNull(8)
                            ? null!
                            : item.Field<DateTime>(8).ToString("dd/MM/yyyy"),
                        idEstadoProcesal = item.IsNull(9) ? 0 : item.Field<int>(9),
                        estadoProcesal = item.IsNull(10) ? null! : item.Field<string>(10),
                        idEstadoTarea = item.IsNull(11) ? 0 : item.Field<int>(11),
                        estadoTarea = item.IsNull(12) ? null! : item.Field<string>(12),
                    }
                );
            }

            return resultList;
        }

        public async Task<int?> GetHistoricoCountAsync(
            string? noAsunto,
            DateTime? desdeRecepcion,
            DateTime? hastaRecepcion,
            string? rfc,
            string? promovente,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
            int? idUnidadAdmistrativaCentral = null!
        )
        {
            ParameterPGsql[] parameters =
            {
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ("p_fecha_recepcion_desde", NpgsqlDbType.Date, desdeRecepcion),
                new ("p_fecha_recepcion_hasta", NpgsqlDbType.Date, hastaRecepcion),
                new ("p_rfc", NpgsqlDbType.Varchar, rfc),
                new ("p_promovente", NpgsqlDbType.Text, promovente),
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoAsunto is null || !TipoAsunto.Any()) ? DBNull.Value :  TipoAsunto.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoTarea is null || !EstadoTarea.Any()) ? DBNull.Value :  EstadoTarea.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoModalidad is null || !TipoModalidad.Any()) ? DBNull.Value :  TipoModalidad.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoProcesal is null || !EstadoProcesal.Any()) ? DBNull.Value :  EstadoProcesal.ToArray()),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpHistoricoCount,
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
        #endregion

        #endregion
    }
}
