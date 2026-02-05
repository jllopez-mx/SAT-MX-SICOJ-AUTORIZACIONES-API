using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;
using Sicoj.Utils.ViewModels;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class PerfilAdministradorGlobalDisconnectedRepository : IPerfilAdministradorGlobalDisconnectedRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion  

        #region Constructor
        public PerfilAdministradorGlobalDisconnectedRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        #region Historico
        public async Task<int?> GetHistoricoCountAsync(
            string? noAsunto,
            DateTime? fechaPresentacion,
            DateTime? fechaVencimiento,
            string? rfc,
            string? promovente,
            List<int>? alerta,
            List<int>? TipoAsunto,
            List<int>? EstadoTarea,
            List<int>? TipoModalidad,
            List<int>? EstadoProcesal,
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
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoAsunto is null || !TipoAsunto.Any()) ? DBNull.Value :  TipoAsunto.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoTarea is null || !EstadoTarea.Any()) ? DBNull.Value :  EstadoTarea.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoModalidad is null || !TipoModalidad.Any()) ? DBNull.Value :  TipoModalidad.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoProcesal is null || !EstadoProcesal.Any()) ? DBNull.Value :  EstadoProcesal.ToArray()),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, idUnidadAdmistrativa)
            };

            var response = await _database.ExecuteFunctionAsync(
                    EnumFunctions.AgHistoricoCount,
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

        public async Task<List<ResponseAutorizacionesAdministradorGlobalHistorico>> GetHistoricoAsync(
           int pageSize,
           int page,
           string? OrderByColumn,
           bool OrderDesc,
           string? noAsunto,
           DateTime? fechaPresentacion,
           DateTime? fechaVencimiento,
           string? rfc,
           string? promovente,
           List<int>? alerta,
           List<int>? TipoAsunto,
           List<int>? EstadoTarea,
           List<int>? TipoModalidad,
           List<int>? EstadoProcesal,
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
                new ("p_id_tipo_asunto", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoAsunto is null || !TipoAsunto.Any()) ? DBNull.Value :  TipoAsunto.ToArray()),
                new ("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoTarea is null || !EstadoTarea.Any()) ? DBNull.Value :  EstadoTarea.ToArray()),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoModalidad is null || !TipoModalidad.Any()) ? DBNull.Value :  TipoModalidad.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoProcesal is null || !EstadoProcesal.Any()) ? DBNull.Value :  EstadoProcesal.ToArray()),
                new ("p_order_column", NpgsqlDbType.Varchar, OrderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, OrderDesc),
                new ("p_activo", NpgsqlDbType.Boolean, true),
                new ("p_id_unidad_administrativa_central", NpgsqlDbType.Integer, idUnidadAdmistrativaCentral),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, idUnidadAdmistrativa),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AgHistorico,
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

            List<ResponseAutorizacionesAdministradorGlobalHistorico> resultList = new();
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
                        idUnidadAdministrativaCentral = item.IsNull(25) ? 0 : item.Field<int>(25),
                        unidadAdministrativaCentral = item.IsNull(26) ? null! : item.Field<string>(26),
                        fechaControlSolicitudes = item.IsNull(27) ? null! : item.Field<DateTime>(27).ToString("dd/MM/yyyy"),
                        tieneCumplimentacion = item.IsNull(28) ? false : item.Field<bool>(28)
                    }
                );
            }

            return resultList;
        }        
        #endregion

        public async Task<ResponseComercioExteriorAdministradorGlobalById> GetComercioExteriorByIdAsync(int id)
        {
            ParameterPGsql[] parameters = { new ("p_id", NpgsqlDbType.Integer, id), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AgAutorizacionByIdDisconnected,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(0),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(1)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(1),
                idTipoModalidad = response.Data.Tables[0].Rows[0].IsNull(2)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(2),
                idTipoAutorizacion = response.Data.Tables[0].Rows[0].IsNull(3)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(3),
                noAsunto = response.Data.Tables[0].Rows[0].IsNull(4)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(4)!,
                rfc = response.Data.Tables[0].Rows[0].IsNull(5)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(5)!,
                promovente = response.Data.Tables[0].Rows[0].IsNull(6)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(6)!,
                promoventeNoContribuyente = !response.Data.Tables[0].Rows[0].IsNull(7)
                    && response.Data.Tables[0].Rows[0].Field<bool>(7),
                rfcContribuyente = response.Data.Tables[0].Rows[0].IsNull(8)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(8),
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(9)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(9),
                despachosAutorizados = response.Data.Tables[0].Rows[0].IsNull(10)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(10),
                idAutoridadDirigida = response.Data.Tables[0].Rows[0].IsNull(11)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(11),
                otroAutoridadDirigida = response.Data.Tables[0].Rows[0].IsNull(12)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(12)!,
                domicilioPromovente = response.Data.Tables[0].Rows[0].IsNull(13)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(13)!,
                domicilioNotificaciones = response.Data.Tables[0].Rows[0].IsNull(14)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(14),
                fechaPresentacion = response.Data.Tables[0].Rows[0].IsNull(15)
                            ? null!
                            : response.Data.Tables[0].Rows[0].Field<DateTime>(15).ToString("dd/MM/yyyy"),
                idFundamentoSolicitud = response.Data.Tables[0].Rows[0].IsNull(16)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(16),
                otroFundamentoSolicitud = response.Data.Tables[0].Rows[0].IsNull(17)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(17)!,
                idTema = response.Data.Tables[0].Rows[0].IsNull(18)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(18),
                otroTema = response.Data.Tables[0].Rows[0].IsNull(19)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(19)!,
                noIndicaMonto = response.Data.Tables[0].Rows[0].IsNull(20)
                     ? false
                     : response.Data.Tables[0].Rows[0].Field<bool?>(20),
                monto = response.Data.Tables[0].Rows[0].IsNull(21)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<decimal>(21),
                fechaRecepcion = response.Data.Tables[0].Rows[0].IsNull(22)
                            ? null!
                            : response.Data.Tables[0].Rows[0].Field<DateTime>(22).ToString("dd/MM/yyyy"),
                fechaVencimiento = response.Data.Tables[0].Rows[0].IsNull(23)
                            ? null!
                            : response.Data.Tables[0].Rows[0].Field<DateTime>(23).ToString("dd/MM/yyyy"),
                idUnidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(24)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(24),
                idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(25)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(25),
                idEstadoTarea = response.Data.Tables[0].Rows[0].IsNull(26)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(26),
                idSubadministracion = response.Data.Tables[0].Rows[0].IsNull(27)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(27),
                idAbogado = response.Data.Tables[0].Rows[0].IsNull(28)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(28),
                requerimiento = response.Data.Tables[0].Rows[0].IsNull(29)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<bool>(29),

                solicitudOpinion = response.Data.Tables[0].Rows[0].IsNull(30)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<bool>(30),
                solicitudOpinionCompleto = !response.Data.Tables[0].Rows[0].IsNull(31)
                    && response.Data.Tables[0].Rows[0].Field<bool>(31),
            };
        }

        public async Task<ResponseImpuestosInternosAdministradorGlobalById> GetImpuestosInternosByIdAsync(int id)
        {
            ParameterPGsql[] parameters = { new ("p_id", NpgsqlDbType.Integer, id), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AgAutorizacionByIdDisconnected,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(0),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(1)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(1),
                idTipoModalidad = response.Data.Tables[0].Rows[0].IsNull(2)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(2),
                noAsunto = response.Data.Tables[0].Rows[0].IsNull(3)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                rfc = response.Data.Tables[0].Rows[0].IsNull(4)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(4)!,
                promovente = response.Data.Tables[0].Rows[0].IsNull(5)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(5)!,
                promoventeNoContribuyente = !response.Data.Tables[0].Rows[0].IsNull(6)
                    && response.Data.Tables[0].Rows[0].Field<bool>(6),
                rfcContribuyente = response.Data.Tables[0].Rows[0].IsNull(7)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(7),
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(8)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(8),
                domicilioPromovente = response.Data.Tables[0].Rows[0].IsNull(9)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(9)!,
                fechaPresentacion = response.Data.Tables[0].Rows[0].IsNull(10)
                            ? null!
                            : response.Data.Tables[0].Rows[0].Field<DateTime>(10).ToString("dd/MM/yyyy"),
                fechaRecepcion = response.Data.Tables[0].Rows[0].IsNull(11)
                            ? null!
                            : response.Data.Tables[0].Rows[0].Field<DateTime>(11).ToString("dd/MM/yyyy"),
                fechaVencimiento = response.Data.Tables[0].Rows[0].IsNull(12)
                            ? null!
                            : response.Data.Tables[0].Rows[0].Field<DateTime>(12).ToString("dd/MM/yyyy"),
                idUnidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(13)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(13),
                idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(14)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(14),
                idEstadoTarea = response.Data.Tables[0].Rows[0].IsNull(15)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(15),
                idSubadministracion = response.Data.Tables[0].Rows[0].IsNull(16)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(16),
                idAbogado = response.Data.Tables[0].Rows[0].IsNull(17)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(17),
                despachosAutorizados = response.Data.Tables[0].Rows[0].IsNull(18)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(18),
                domicilioNotificaciones = response.Data.Tables[0].Rows[0].IsNull(19)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(19),
                idTema = response.Data.Tables[0].Rows[0].IsNull(20)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(20),
                otroTema = response.Data.Tables[0].Rows[0].IsNull(21)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string?>(21)!,
                noIndicaMonto = response.Data.Tables[0].Rows[0].IsNull(22)
                     ? false
                     : response.Data.Tables[0].Rows[0].Field<bool?>(22),
                monto = response.Data.Tables[0].Rows[0].IsNull(23)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<decimal>(23),
                requerimiento = response.Data.Tables[0].Rows[0].IsNull(24)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<bool>(24),
                solicitudOpinion = response.Data.Tables[0].Rows[0].IsNull(25)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<bool>(25),
                solicitudOpinionCompleto = !response.Data.Tables[0].Rows[0].IsNull(26)
                    && response.Data.Tables[0].Rows[0].Field<bool>(26),
            };
        }
    }
}
