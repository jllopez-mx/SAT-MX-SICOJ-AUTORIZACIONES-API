using System.Data;
using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class AutorizacionRepository : IAutorizacionRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AutorizacionRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResponseAutorizacionGenericById> GetDisconnectedById(int id)
        {
            ParameterPGsql[] parameters = { new ("p_id", NpgsqlDbType.Integer, id), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AutorizacionByIdDisconnected,
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
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<int>(24),
                idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(25)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<int>(25),
                idEstadoTarea = response.Data.Tables[0].Rows[0].IsNull(26)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<int>(26),
                idSubadministracion = response.Data.Tables[0].Rows[0].IsNull(27)
                     ? null!
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
                idAutorizacionRelacionadoAvisoSinRespuesta = response.Data.Tables[0].Rows[0].IsNull(32)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<int>(32),
                noAsuntoExternoAvisoSinRespuesta = response.Data.Tables[0].Rows[0].IsNull(33)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(33),
                idAutorizacionRelacionadoAvisoConRespuesta = response.Data.Tables[0].Rows[0].IsNull(34)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<int>(34),
                noAsuntoExternoAvisoConRespuesta = response.Data.Tables[0].Rows[0].IsNull(35)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(35),
            };
        }

        public async Task<Autorizacion> GetByIdAsync(int id)
        {
            var resultList = await this.GetListByIdsAsync(new int[1] {id});
            if (resultList is null || !resultList.Any())
                return null!;

            return resultList.FirstOrDefault()!;
        }

        public async Task<Autorizacion> GeAutorizacionAsync(int? id)
        {
            var resultList = id == null ? null : await this.GetListByIdsAsync(new int[1] { id.Value});
            if (resultList is null || !resultList.Any())
                return null!;

            return resultList.FirstOrDefault()!;
        }

        public async Task<List<Autorizacion>> GetListByIdsAsync(int[] ids)
        {
            ParameterPGsql[] parameters = { new ("p_ids", NpgsqlDbType.Array | NpgsqlDbType.Integer, ids), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AutorizacionByIds,
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

            List<Autorizacion> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(new()
                {
                    id = item.IsNull(0)
                        ? 0
                        : item.Field<int>(0),
                    id_tipo_asunto = item.IsNull(1)
                        ? 0
                        : item.Field<int>(1),
                    id_tipo_modalidad = item.IsNull(2)
                         ? 0
                         : item.Field<int>(2),
                    id_tipo_autorizacion = item.IsNull(3)
                        ? 0
                        : item.Field<int>(3),
                    no_asunto = item.IsNull(4)
                        ? null!
                        : item.Field<string?>(4)!,
                    rfc = item.IsNull(5)
                        ? null!
                        : item.Field<string?>(5)!,
                    promovente = item.IsNull(6)
                        ? null!
                        : item.Field<string?>(6)!,
                    promovente_no_contribuyente = !item.IsNull(7)
                        && item.Field<bool>(7),
                    rfc_contribuyente = item.IsNull(8)
                        ? null!
                        : item.Field<string>(8),
                    contribuyente = item.IsNull(9)
                        ? null!
                        : item.Field<string>(9),
                    despachos_autorizados = item.IsNull(10)
                        ? null!
                        : item.Field<string>(10),
                    id_autoridad_dirigida = item.IsNull(11)
                        ? 0
                        : item.Field<int>(11),
                    otra_autoridad_dirigida = item.IsNull(12)
                        ? null!
                        : item.Field<string?>(12)!,
                    domicilio_promovente = item.IsNull(13)
                        ? null!
                        : item.Field<string?>(13)!,
                    domicilio_notificaciones = item.IsNull(14)
                        ? null!
                        : item.Field<string>(14),
                    fecha_presentacion = item.IsNull(15)
                        ? new()
                        : item.Field<DateTime>(15),
                    id_fundamento_solicitud = item.IsNull(16)
                        ? 0
                        : item.Field<int>(16),
                    otro_fundamento_solicitud = item.IsNull(17)
                        ? null!
                        : item.Field<string?>(17)!,
                    id_tema = item.IsNull(18)
                        ? 0
                        : item.Field<int>(18),
                    otro_tema = item.IsNull(19)
                        ? null!
                        : item.Field<string?>(19)!,
                    no_indica_monto = item.IsNull(20)
                        ? false
                        : item.Field<bool?>(20),
                    monto = item.IsNull(21)
                        ? 0
                        : item.Field<decimal>(21),
                    fecha_recepcion = item.IsNull(22)
                        ? new()
                        : item.Field<DateTime>(22),
                    fecha_vencimiento = item.IsNull(23)
                        ? new()
                        : item.Field<DateTime>(23),
                    id_unidad_administrativa = item.IsNull(24)
                        ? 0
                        : item.Field<int>(24),
                    id_estado_procesal = item.IsNull(25)
                        ? 0
                        : item.Field<int>(25),
                    id_estado_tarea = item.IsNull(26)
                        ? 0
                        : item.Field<int>(26),
                    activo = !item.IsNull(27)
                        && item.Field<bool>(27),
                    id_unidad_administrativa_central = item.IsNull(28)
                        ? 0
                        : item.Field<int>(28),
                    id_subadministracion = item.IsNull(29)
                     ? 0
                     : item.Field<int>(29),
                    abogado = new()
                    {
                        id = response.Data.Tables[0].Rows[0].IsNull(30)
                            ? 0
                            : response.Data.Tables[0].Rows[0].Field<int>(30),
                        id_abogado = response.Data.Tables[0].Rows[0].IsNull(31)
                            ? null!
                            : response.Data.Tables[0].Rows[0].Field<string>(31)!,
                    },
                    fecha_control_solicitudes = response.Data.Tables[0].Rows[0].IsNull(32)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<DateTime>(32),
                    requerimiento = item.IsNull(33)
                        ? null!
                        : item.Field<bool>(33),
                    solicitud_opinion = item.IsNull(34)
                        ? null!
                        : item.Field<bool>(34),
                    externo_cumplimentacion = !item.IsNull(35)
                        && item.Field<bool>(35),
                });
            }

            return resultList;
        }

        public async Task<Autorizacion> GetByNumeroAsuntoAsync(string numeroAsunto)
        {
            var resultList = await this.GetListByNumerosAsuntoAsync(new string[1] { numeroAsunto });
            if (resultList is null || !resultList.Any())
                return null!;

            return resultList.FirstOrDefault()!;
        }

        public async Task<List<Autorizacion>> GetListByNumerosAsuntoAsync(string[] numerosAsunto)
        {
            ParameterPGsql[] parameters = { new ("p_numeros_asunto", NpgsqlDbType.Array | NpgsqlDbType.Text, numerosAsunto), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AutorizacionByNumerosAsunto,
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

            List<Autorizacion> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(new()
                {
                    id = item.IsNull(0)
                     ? 0
                     : item.Field<int>(0),
                    id_tipo_asunto = item.IsNull(1)
                     ? 0
                     : item.Field<int>(1),
                    id_tipo_modalidad = item.IsNull(2)
                     ? 0
                     : item.Field<int>(2),
                    id_tipo_autorizacion = item.IsNull(3)
                     ? 0
                     : item.Field<int>(3),
                    no_asunto = item.IsNull(4)
                     ? null!
                     : item.Field<string?>(4)!,
                    rfc = item.IsNull(5)
                     ? null!
                     : item.Field<string?>(5)!,
                    promovente = item.IsNull(6)
                     ? null!
                     : item.Field<string?>(6)!,
                    promovente_no_contribuyente = !item.IsNull(7)
                    && item.Field<bool>(7),
                    rfc_contribuyente = item.IsNull(8)
                     ? null!
                     : item.Field<string>(8),
                    contribuyente = item.IsNull(9)
                     ? null!
                     : item.Field<string>(9),
                    despachos_autorizados = item.IsNull(10)
                     ? null!
                     : item.Field<string>(10),
                    id_autoridad_dirigida = item.IsNull(11)
                     ? 0
                     : item.Field<int>(11),
                    otra_autoridad_dirigida = item.IsNull(12)
                     ? null!
                     : item.Field<string?>(12)!,
                    domicilio_promovente = item.IsNull(13)
                     ? null!
                     : item.Field<string?>(13)!,
                    domicilio_notificaciones = item.IsNull(14)
                     ? null!
                     : item.Field<string>(14),
                    fecha_presentacion = item.IsNull(15)
                     ? new()
                     : item.Field<DateTime>(15),
                    id_fundamento_solicitud = item.IsNull(16)
                     ? 0
                     : item.Field<int>(16),
                    otro_fundamento_solicitud = item.IsNull(17)
                     ? null!
                     : item.Field<string?>(17)!,
                    id_tema = item.IsNull(18)
                     ? 0
                     : item.Field<int>(18),
                    otro_tema = item.IsNull(19)
                     ? null!
                     : item.Field<string?>(19)!,
                    no_indica_monto = item.IsNull(20)
                     ? false
                     : item.Field<bool?>(20),
                    monto = item.IsNull(21)
                     ? 0
                     : item.Field<decimal>(21),
                    fecha_recepcion = item.IsNull(22)
                     ? new()
                     : item.Field<DateTime>(22),
                    fecha_vencimiento = item.IsNull(23)
                     ? new()
                     : item.Field<DateTime>(23),
                    id_unidad_administrativa = item.IsNull(24)
                     ? 0
                     : item.Field<int>(24),
                    id_estado_procesal = item.IsNull(25)
                     ? 0
                     : item.Field<int>(25),
                    id_estado_tarea = item.IsNull(26)
                     ? 0
                     : item.Field<int>(26),
                    activo = !item.IsNull(27)
                    && item.Field<bool>(27),
                    id_unidad_administrativa_central = item.IsNull(28)
                     ? 0
                     : item.Field<int>(28),
                    id_subadministracion = item.IsNull(29)
                     ? 0
                     : item.Field<int>(29),
                    abogado = new()
                    {
                        id = response.Data.Tables[0].Rows[0].IsNull(30)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int>(30),
                        id_abogado = response.Data.Tables[0].Rows[0].IsNull(31)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<string>(31)!,
                    },
                    fecha_control_solicitudes = response.Data.Tables[0].Rows[0].IsNull(32)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<DateTime>(32),
                });
            }

            return resultList;
        }

        public async Task<ResultTransaction> AddAsync(Autorizacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id_tipo_asunto",
                    NpgsqlDbType.Integer,
                    entity.id_tipo_asunto!
                ),
                new (
                    "p_id_tipo_modalidad",
                    NpgsqlDbType.Integer,
                    entity.id_tipo_modalidad!
                ),
                new (
                    "p_id_tipo_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_tipo_autorizacion!
                ),
                new ("p_rfc", NpgsqlDbType.Varchar, entity.rfc!),
                new ("p_promovente", NpgsqlDbType.Varchar, entity.promovente!),
                new (
                    "p_promovente_no_contribuyente",
                    NpgsqlDbType.Boolean,
                    entity.promovente_no_contribuyente!
                ),
                new (
                    "p_rfc_contribuyente",
                    NpgsqlDbType.Varchar,
                    entity.rfc_contribuyente!
                ),
                new ("p_contribuyente", NpgsqlDbType.Varchar, entity.contribuyente!),
                new (
                    "p_despachos_autorizados",
                    NpgsqlDbType.Varchar,
                    entity.despachos_autorizados!
                ),
                new (
                    "p_id_autoridad_dirigida",
                    NpgsqlDbType.Integer,
                    entity.id_autoridad_dirigida!
                ),
                new (
                    "p_otra_autoridad_dirigida",
                    NpgsqlDbType.Text,
                    entity.otra_autoridad_dirigida!
                ),
                new (
                    "p_domicilio_promovente",
                    NpgsqlDbType.Varchar,
                    entity.domicilio_promovente!
                ),
                new (
                    "p_domicilio_notificaciones",
                    NpgsqlDbType.Varchar,
                    entity.domicilio_notificaciones!
                ),
                new (
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entity.fecha_presentacion!
                ),
                new (
                    "p_id_fundamento_solicitud",
                    NpgsqlDbType.Integer,
                    entity.id_fundamento_solicitud!
                ),
                new (
                    "p_otro_fundamento_solicitud",
                    NpgsqlDbType.Text,
                    entity.otro_fundamento_solicitud!
                ),
                new ("p_id_tema", NpgsqlDbType.Integer, entity.id_tema!),
                new ("p_otro_tema", NpgsqlDbType.Text, entity.otro_tema!),
                new (
                    "p_no_indica_monto",
                    NpgsqlDbType.Boolean,
                    entity.no_indica_monto!
                ),
                new ("p_monto", NpgsqlDbType.Numeric, entity.monto!),
                new ("p_fecha_recepcion", NpgsqlDbType.Date, entity.fecha_recepcion!),
                new (
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entity.fecha_vencimiento!
                ),
                new (
                    "p_id_unidad_administrativa_central",
                    NpgsqlDbType.Integer,
                    entity.id_unidad_administrativa_central!
                ),
                new (
                    "p_id_unidad_administrativa",
                    NpgsqlDbType.Integer,
                    entity.id_unidad_administrativa!
                ),
                new (
                    "p_id_subadministracion",
                    NpgsqlDbType.Integer,
                    entity.id_subadministracion!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new (
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entity.usuario_creacion!
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpAutorizacionCreate,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> TurnarAsync(Autorizacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, entity.id),
                new ("p_numero_empleado", NpgsqlDbType.Text, entity.no_empleado_turna!),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity.usuario_modificacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpAutorizacionTurnar,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> DeleteAsync(Autorizacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, entity.id),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity.usuario_modificacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpAutorizacionDelete,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> AsignarAsync(Autorizacion entity, Cumplimentacion cumplimentacion, Abogado entityAbogado)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity != null ? entity.id! : DBNull.Value),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, cumplimentacion != null ? cumplimentacion.id! : DBNull.Value),
                new ("p_id_abogado", NpgsqlDbType.Text, entityAbogado.id_abogado!),
                new ("p_numero_empleado", NpgsqlDbType.Text, entity != null ? entity.no_empleado_turna! : cumplimentacion!.no_empleado_turna!),
                new ("p_id_estado_procesal", NpgsqlDbType.Integer, entity != null ? entity.id_estado_procesal! : cumplimentacion!.id_estado_procesal!),
                new ("p_id_estado_tarea", NpgsqlDbType.Integer, entity != null ? entity.id_estado_tarea! : cumplimentacion!.id_estado_tarea!),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity != null ? entity.usuario_modificacion! : cumplimentacion!.usuario_modificacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AdminAsignarAbogado,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> RemitirAsync(Autorizacion entity, Remision entityRemision, Documento? entityDocumento, DataFile? dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.id),
                new (
                    "p_id_tipo_autoridad_remision",
                    NpgsqlDbType.Integer,
                    entityRemision.id_tipo_autoridad!
                ),
                new (
                    "p_id_unidad_administrativa_remite",
                    NpgsqlDbType.Integer,
                    entityRemision.id_unidad_administrativa_remite!
                ),
                new (
                    "p_id_unidad_administrativa_recibe",
                    NpgsqlDbType.Integer,
                    entityRemision.id_unidad_administrativa_recibe!
                ),
                new (
                    "p_id_unidad_administrativa_externa",
                    NpgsqlDbType.Integer,
                    entityRemision.id_unidad_administrativa_externa!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entityRemision.fecha_oficio!
                ),
                new ("p_numero_oficio", NpgsqlDbType.Text, entityRemision.numero_oficio!),
                new ("p_id_estado_tarea", NpgsqlDbType.Integer, entity.id_estado_tarea!),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new ("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_modificacion!),
                new (
                    "p_archivo",
                    NpgsqlDbType.Boolean,
                    dataFile is not null
                ),
                new ("p_id_tipo_documento", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!),
                new ("p_id_seccion", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!),
                new ("p_file_name", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.file_name!),
                new ("p_file_path", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.file_path !),
                new ("p_content_type", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.content_type!),
                new ("p_file_size", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.file_size!),
                new ("p_owner_name", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!),
                new ("p_numero_folio", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.numero_folio!),
                new ("p_permanente", NpgsqlDbType.Boolean, entityDocumento is null ? DBNull.Value : entityDocumento.permanente!),
                new ("p_id_rol", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_rol!),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_unidad_administrativa!),
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.AdminRemision,
                dataFile!,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> ReasignarAsync(List<Reasignar> listReasignacion)
        {
            var arrayObject = listReasignacion.Select(c =>
            $"{c.id_autorizacion},{c.rfc_funcionario_reasignador},{c.rfc_funcionario_reasignado},{c.rfc_funcionario_retirado},{c.id_unidad_administrativa_reasingador},{c.id_subadministracion_reasignador},{c.id_unidad_administrativa_reasignado},{c.id_subadministracion_reasignado},{c.id_estado_procesal_previo}").ToArray();
            ParameterPGsql[] parameters =
            {
                new ("p_array", NpgsqlDbType.Array |NpgsqlDbType.Text , arrayObject),
                new ("p_id_estado_tarea", NpgsqlDbType.Integer, EnumEstadoTarea.Reasingado.GetHashCode()),

            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenReasignar,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> UpdateAsync(Autorizacion entity, AvisoSinRespuesta avisoSinRespuesta , AvisoConRespuesta avisoConRespuesta, int? idSeccion =  null!)
        {
            bool isAvisoSinRespuesta = avisoSinRespuesta is not null;
            bool isAvisoConRespuesta = avisoConRespuesta is not null;
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, entity.id),
                new ("p_id_tipo_asunto", NpgsqlDbType.Integer, entity.id_tipo_asunto),
                new (
                    "p_id_tipo_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_tipo_autorizacion!
                ),
                new ("p_rfc", NpgsqlDbType.Varchar, entity.rfc!),
                new ("p_promovente", NpgsqlDbType.Varchar, entity.promovente!),
                new (
                    "p_promovente_no_contribuyente",
                    NpgsqlDbType.Boolean,
                    entity.promovente_no_contribuyente!
                ),
                new (
                    "p_rfc_contribuyente",
                    NpgsqlDbType.Varchar,
                    entity.rfc_contribuyente!
                ),
                new ("p_contribuyente", NpgsqlDbType.Varchar, entity.contribuyente!),
                new (
                    "p_despachos_autorizados",
                    NpgsqlDbType.Varchar,
                    entity.despachos_autorizados!
                ),
                new (
                    "p_id_autoridad_dirigida",
                    NpgsqlDbType.Integer,
                    entity.id_autoridad_dirigida!
                ),
                new (
                    "p_otra_autoridad_dirigida",
                    NpgsqlDbType.Text,
                    entity.otra_autoridad_dirigida!
                ),
                new (
                    "p_domicilio_notificaciones",
                    NpgsqlDbType.Varchar,
                    entity.domicilio_notificaciones!
                ),
                new (
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entity.fecha_presentacion!
                ),
                new (
                    "p_id_fundamento_solicitud",
                    NpgsqlDbType.Integer,
                    entity.id_fundamento_solicitud!
                ),
                new (
                    "p_otro_fundamento_solicitud",
                    NpgsqlDbType.Text,
                    entity.otro_fundamento_solicitud!
                ),
                new ("p_id_tema", NpgsqlDbType.Integer, entity.id_tema!),
                new ("p_otro_tema", NpgsqlDbType.Text, entity.otro_tema!),
                new (
                    "p_no_indica_monto",
                    NpgsqlDbType.Boolean,
                    entity.no_indica_monto!
                ),
                new ("p_monto", NpgsqlDbType.Numeric, entity.monto!),
                new ("p_fecha_recepcion", NpgsqlDbType.Date, entity.fecha_recepcion!),
                new (
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entity.fecha_vencimiento!
                ),
                  new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                  new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entity.usuario_modificacion!
                ),
                new ("p_id_seccion_modificacion", NpgsqlDbType.Integer, idSeccion),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, entity.id_unidad_administrativa),
                new (
                    "p_id_subadministracion",
                    NpgsqlDbType.Integer,
                    entity.id_subadministracion!
                ),
                new ("p_aviso_sin_respuesta", NpgsqlDbType.Boolean, isAvisoSinRespuesta),
                new ("p_id_aviso_sin_respuesta", NpgsqlDbType.Integer, isAvisoSinRespuesta ? avisoSinRespuesta!.id : DBNull.Value),
                new ("p_id_autorizacion_relacionado_aviso_sin_respuesta", NpgsqlDbType.Integer, isAvisoSinRespuesta ? avisoSinRespuesta!.id_autorizacion_relacionado : DBNull.Value),
                new ("p_no_asunto_externo_aviso_sin_respuesta", NpgsqlDbType.Text, isAvisoSinRespuesta ? avisoSinRespuesta!.no_asunto_externo : DBNull.Value),
                new ("p_aviso_con_respuesta", NpgsqlDbType.Boolean, isAvisoConRespuesta),
                new ("p_id_aviso_con_respuesta", NpgsqlDbType.Integer, isAvisoConRespuesta ? avisoConRespuesta!.id : DBNull.Value),
                new ("p_id_autorizacion_relacionado_aviso_con_respuesta", NpgsqlDbType.Integer, isAvisoConRespuesta ? avisoConRespuesta!.id_autorizacion_relacionado : DBNull.Value),
                new ("p_no_asunto_externo_aviso_con_respuesta", NpgsqlDbType.Text, isAvisoConRespuesta ? avisoConRespuesta!.no_asunto_externo : DBNull.Value),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AutorizacionUpdate,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> UpdateNoRequerimientoAsync(Autorizacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.id),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entity.usuario_modificacion!
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenNoRequerimiento,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> ConcluirAsync(Autorizacion entity, Cumplimentacion cumplimentacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity is null ? DBNull.Value : entity.id),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, cumplimentacion is null ? DBNull.Value : cumplimentacion.id),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity is null ? cumplimentacion!.id_estado_procesal :  entity.id_estado_procesal!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity is null ? cumplimentacion!.id_estado_tarea :  entity.id_estado_tarea!
                ),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity is null ? cumplimentacion!.usuario_modificacion :  entity.usuario_modificacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenConcluir,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> ReactivarAsync(Autorizacion autorizacion, Cumplimentacion cumplimentacion, Abogado entityAbogado, Reactivar entityReactivar)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, autorizacion != null ? autorizacion.id! : DBNull.Value),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, cumplimentacion != null ? cumplimentacion.id! : DBNull.Value),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityReactivar.id_estado_procesal!
                ),
                new (
                    "p_id_estado_procesal_nuevo",
                    NpgsqlDbType.Integer,
                    autorizacion is null ? cumplimentacion!.id_estado_procesal : autorizacion!.id_estado_procesal!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    autorizacion is null ? cumplimentacion!.id_estado_tarea :autorizacion!.id_estado_tarea!
                ),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, autorizacion is null ? cumplimentacion!.usuario_modificacion : autorizacion!.usuario_modificacion !),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AgReactivar,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        #region Aviso Sin Respuesta  II
        public async Task<ResultTransaction> RechazarAvisoSinRespuestaAsync(Autorizacion entity, int[] secciones)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.id),
                new ("p_id_tipo_asunto", NpgsqlDbType.Integer, entity.id_tipo_asunto),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity.usuario_modificacion!),
                new ("p_secciones", NpgsqlDbType.Array | NpgsqlDbType.Integer, secciones!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AdminAvisoSinRespuestaRechazar,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> AprobarAvisoSinRespuestaAsync(Autorizacion entity, AvisoSinRespuesta entityAviso)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, entityAviso.id),
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entityAviso.id_autorizacion),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity.usuario_modificacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AdminAvisoSinRespuestaAprobar,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<List<ResponseNumeroAsunto>> GetListByNoAsuntoAsync(string noAsunto, int idTipoAsunto)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ("p_id_tipo_asunto", NpgsqlDbType.Integer, idTipoAsunto),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAutorizacionNumeroAsunto,
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

            List<ResponseNumeroAsunto> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(new()
                {
                    idAsunto = item.IsNull(0)
                     ? 0
                     : item.Field<int>(0),
                    noAsunto = item.IsNull(1)
                     ? null!
                     : item.Field<string>(1)!,
                });
            }

            return resultList;
        }

        public async Task<Autorizacion> GetAvisoNoAsuntoExternoAsync(string? noAsunto)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_no_asunto", NpgsqlDbType.Text, noAsunto),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAutorizacionBuscarNumeroAsunto,
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

            var row = response.Data.Tables[0].Rows[0];

            return new Autorizacion
            {
                id = row.IsNull(0) ? 0 : row.Field<int>(0),
                no_asunto = row.IsNull(1) ? null! : row.Field<string>(1)!,
                activo = row.IsNull(2) ? false : row.Field<bool>(2),
                id_estado_procesal = row.IsNull(3) ? 0 : row.Field<int>(3),
                id_tipo_asunto = row.IsNull(4) ? 0 : row.Field<int>(4),
            };
        }

        #endregion

    }
}