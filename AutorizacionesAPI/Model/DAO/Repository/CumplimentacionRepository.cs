using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;
using System.Data;


namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class CumplimentacionRepository : ICumplimentacionRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public CumplimentacionRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddCumplimentacionAsync(Autorizacion entity, Cumplimentacion entityCumplimentacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_externo_cumplimentacion",NpgsqlDbType.Boolean, entity.externo_cumplimentacion),
                new ("p_id_autorizacion",NpgsqlDbType.Integer, entity.id!),
                new ("p_no_asunto", NpgsqlDbType.Text, entity.no_asunto!),
                new ("p_id_tipo_asunto", NpgsqlDbType.Integer, entityCumplimentacion.id_tipo_asunto!),
                new ("p_id_tipo_modalidad", NpgsqlDbType.Integer, entityCumplimentacion.id_tipo_modalidad!),
                new ("p_id_estado_procesal",NpgsqlDbType.Integer,entityCumplimentacion.id_estado_procesal!),
                new ("p_id_estado_tarea",NpgsqlDbType.Integer,entityCumplimentacion.id_estado_tarea!),
                new ("p_id_unidad_administrativa_central",NpgsqlDbType.Integer,entityCumplimentacion.id_unidad_administrativa_central!),
                new ("p_usuario_creacion",NpgsqlDbType.Text,entityCumplimentacion.usuario_creacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpCumplimentacionCreate,
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

        public async Task<ResultTransaction> UpdateAsync(Autorizacion entityAutorizacion, Cumplimentacion entityCumplimentacion)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_externo_cumplimentacion",
                    NpgsqlDbType.Boolean,
                    entityAutorizacion.externo_cumplimentacion!
                ),
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entityAutorizacion.id!
                ),
                new (
                    "p_id_tipo_asunto",
                    NpgsqlDbType.Integer,
                    entityAutorizacion.id_tipo_asunto!
                ),                
                new (
                    "p_rfc",
                    NpgsqlDbType.Text,
                    entityAutorizacion.rfc!
                ),
                new (
                    "p_promovente",
                    NpgsqlDbType.Text,
                    entityAutorizacion.promovente!
                ),
                new (
                    "p_promovente_no_contribuyente",
                    NpgsqlDbType.Boolean,
                    entityAutorizacion.promovente_no_contribuyente!
                ),
                new (
                    "p_rfc_contribuyente",
                    NpgsqlDbType.Text,
                    entityAutorizacion.rfc_contribuyente!
                ),
                new (
                    "p_contribuyente",
                    NpgsqlDbType.Text,
                    entityAutorizacion.contribuyente!
                ),
                new (
                    "p_id",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id!
                ),
                new (
                    "p_numero_juicio",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.no_juicio_recuro_amparo!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_recepcion_solicitud!
                ),
                new (
                    "p_fecha_firmeza",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_firmeza!
                ),
                new (
                    "p_id_plaza_cumplimento",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_plazo_cumplimiento!
                ),
                new (
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_vencimiento!
                ),
                new (
                    "p_id_organo_jurisdiccional",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_organo_jurisdiccional!
                ),
                new (
                    "p_organo_jurisdiccional",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.organo_jurisdiccional!
                ),
                new (
                    "p_id_unidad_administrativa_cumplimiento",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_unidad_administrativa_cumplimiento!
                ),
                new (
                    "p_unidad_administrativa_cumplimiento",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.unidad_administrativa_cumplimiento!
                ),
                new (
                    "p_oficio_resolucion_impugnada",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.oficio_resolucion_impugnada!
                ),
                new (
                    "p_fecha_oficio_resolucion_impugnada",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_oficio_resolucion_impugnada!
                ),
                new (
                    "p_id_unidad_administrativa",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_unidad_administrativa!
                ),
                new (
                    "p_id_subadministracion",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_subadministracion!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_estado_tarea!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_estado_procesal!
                ),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.usuario_modificacion!
                ),                
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpCumplimentacionUpdate,
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

        public async Task<ResponseCumplimentacionOficialPartesById> GetCumplimentacionByIdDisconnectedAsync(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpCumplimentacionByIdDisconnected,
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
                id = response.Data.Tables[0].Rows[0].IsNull(0) ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsunto = response.Data.Tables[0].Rows[0].IsNull(1) ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(2)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(2),
                noAsunto = response.Data.Tables[0].Rows[0].IsNull(3)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                idTipoModalidad = response.Data.Tables[0].Rows[0].IsNull(4)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(4),
                rfc = response.Data.Tables[0].Rows[0].IsNull(5)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(5)!,
                promovente = response.Data.Tables[0].Rows[0].IsNull(6)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(6)!,
                promoventeNoContribuyente = response.Data.Tables[0].Rows[0].IsNull(7)
                     ? false : response.Data.Tables[0].Rows[0].Field<bool>(7),
                rfcContribuyente = response.Data.Tables[0].Rows[0].IsNull(8)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string>(8),
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(9)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string>(9),
                noAsuntoCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(10)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(10)!,
                noJuicioRecuroAmparo = response.Data.Tables[0].Rows[0].IsNull(11)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(11)!,
                fechaRecepcionSolicitud = response.Data.Tables[0].Rows[0].IsNull(12) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(12).ToString("dd/MM/yyyy"),
                fechaFirmeza = response.Data.Tables[0].Rows[0].IsNull(13) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(13).ToString("dd/MM/yyyy"),
                idPlazoCumplimiento = response.Data.Tables[0].Rows[0].IsNull(14)
                     ? null! : response.Data.Tables[0].Rows[0].Field<int?>(14)!,
                fechaVencimiento = response.Data.Tables[0].Rows[0].IsNull(15) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(15).ToString("dd/MM/yyyy"),
                idOrganoJurisdiccional = response.Data.Tables[0].Rows[0].IsNull(16)
                     ? null! : response.Data.Tables[0].Rows[0].Field<int?>(16)!,
                organoJurisdiccional = response.Data.Tables[0].Rows[0].IsNull(17)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(17)!,
                idUnidadAdministrativaCumplimiento = response.Data.Tables[0].Rows[0].IsNull(18)
                     ? null! : response.Data.Tables[0].Rows[0].Field<int?>(18)!,
                unidadAdministrativaCumplimiento = response.Data.Tables[0].Rows[0].IsNull(19)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(19)!,
                oficioResolucionImpugnada = response.Data.Tables[0].Rows[0].IsNull(20)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(20)!,
                fechaOficioResolucionImpugnada = response.Data.Tables[0].Rows[0].IsNull(21) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(21).ToString("dd/MM/yyyy"),
                idUnidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(22)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(22),
                idTipoAsuntoCumplimentar = response.Data.Tables[0].Rows[0].IsNull(23)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(23),
                externoCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(24)
                     ? false : response.Data.Tables[0].Rows[0].Field<bool>(24),
            };
        }

        public async Task<Cumplimentacion> GetCumplimentacionByIdAsync(int? id, int? idAutorizacion)
        {
            ParameterPGsql[] parameters =
            {
                    new ("p_id", NpgsqlDbType.Integer, id),
                    new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion)
                };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpCumplimentacionById,
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
                id_autorizacion = response.Data.Tables[0].Rows[0].IsNull(1) 
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),                
                id_tipo_asunto = response.Data.Tables[0].Rows[0].IsNull(2)
                     ? 0 : 
                     response.Data.Tables[0].Rows[0].Field<int>(2),
                no_asunto_cumplimentacion = response.Data.Tables[0].Rows[0].IsNull(3)
                     ? null! 
                     : response.Data.Tables[0].Rows[0].Field<string?>(3)!,                
                no_juicio_recuro_amparo = response.Data.Tables[0].Rows[0].IsNull(4)
                     ? null! 
                     : response.Data.Tables[0].Rows[0].Field<string?>(4)!,
                fecha_recepcion_solicitud = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? new() 
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(5),
                fecha_firmeza = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? new() 
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(6),
                id_plazo_cumplimiento = response.Data.Tables[0].Rows[0].IsNull(7)
                     ? null! 
                     : response.Data.Tables[0].Rows[0].Field<int?>(7)!,
                fecha_vencimiento = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? new() 
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(8),
                id_organo_jurisdiccional = response.Data.Tables[0].Rows[0].IsNull(9)
                     ? null! 
                     : response.Data.Tables[0].Rows[0].Field<int?>(9)!,
                id_unidad_administrativa_cumplimiento = response.Data.Tables[0].Rows[0].IsNull(10)
                     ? null! 
                     : response.Data.Tables[0].Rows[0].Field<int?>(10)!,
                unidad_administrativa_cumplimiento = response.Data.Tables[0].Rows[0].IsNull(11)
                     ? null! 
                     : response.Data.Tables[0].Rows[0].Field<string?>(11)!,
                oficio_resolucion_impugnada = response.Data.Tables[0].Rows[0].IsNull(12)
                     ? null! 
                     : response.Data.Tables[0].Rows[0].Field<string?>(12)!,
                fecha_oficio_resolucion_impugnada = response.Data.Tables[0].Rows[0].IsNull(13)
                    ? new() 
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(13),
                id_estado_procesal = response.Data.Tables[0].Rows[0].IsNull(14) 
                    ? 0 
                    : response.Data.Tables[0].Rows[0].Field<int>(14),
                id_estado_tarea = response.Data.Tables[0].Rows[0].IsNull(15) 
                    ? 0 
                    : response.Data.Tables[0].Rows[0].Field<int>(15),
                id_tipo_modalidad = response.Data.Tables[0].Rows[0].IsNull(16) 
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(16),
                id_unidad_administrativa = response.Data.Tables[0].Rows[0].IsNull(17) 
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(17),
                id_subadministracion = response.Data.Tables[0].Rows[0].IsNull(18) 
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(18),
                id_unidad_administrativa_central = response.Data.Tables[0].Rows[0].IsNull(19) 
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(19),
                activo = response.Data.Tables[0].Rows[0].IsNull(20) 
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(20),
            };
        }
                
        public async Task<ResultTransaction> TurnarCumplimentacionAsync(Cumplimentacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, entity.id),
                new ("p_numero_empleado", NpgsqlDbType.Text, entity.no_empleado_turna!),
                new ("p_id_estado_procesal",NpgsqlDbType.Integer,entity.id_estado_procesal!),
                new ("p_id_estado_tarea",NpgsqlDbType.Integer, entity.id_estado_tarea! ),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity.usuario_modificacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OpTurnarCumplimentacion,
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

        public async Task<ResponseCumplimentacionById> GetCumplimentacionDisconnectedAsync(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenCumplimentacionByIdDisconnected,
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
                id = response.Data.Tables[0].Rows[0].IsNull(0) ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsunto = response.Data.Tables[0].Rows[0].IsNull(1) ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(2)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(2),
                noAsunto = response.Data.Tables[0].Rows[0].IsNull(3)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                idTipoModalidad = response.Data.Tables[0].Rows[0].IsNull(4)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(4),
                rfc = response.Data.Tables[0].Rows[0].IsNull(5)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(5)!,
                promovente = response.Data.Tables[0].Rows[0].IsNull(6)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(6)!,
                promoventeNoContribuyente = response.Data.Tables[0].Rows[0].IsNull(7)
                     ? false : response.Data.Tables[0].Rows[0].Field<bool>(7),
                rfcContribuyente = response.Data.Tables[0].Rows[0].IsNull(8)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string>(8),
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(9)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string>(9),
                noAsuntoCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(10)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(10)!,
                noJuicioRecuroAmparo = response.Data.Tables[0].Rows[0].IsNull(11)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(11)!,
                fechaRecepcionSolicitud = response.Data.Tables[0].Rows[0].IsNull(12) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(12).ToString("dd/MM/yyyy"),
                fechaFirmeza = response.Data.Tables[0].Rows[0].IsNull(13) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(13).ToString("dd/MM/yyyy"),
                idPlazoCumplimiento = response.Data.Tables[0].Rows[0].IsNull(14)
                     ? null! : response.Data.Tables[0].Rows[0].Field<int?>(14)!,
                fechaVencimiento = response.Data.Tables[0].Rows[0].IsNull(15) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(15).ToString("dd/MM/yyyy"),
                idOrganoJurisdiccional = response.Data.Tables[0].Rows[0].IsNull(16)
                     ? null! : response.Data.Tables[0].Rows[0].Field<int?>(16)!,
                organoJurisdiccional = response.Data.Tables[0].Rows[0].IsNull(17)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(17)!,
                idUnidadAdministrativaCumplimiento = response.Data.Tables[0].Rows[0].IsNull(18)
                     ? null! : response.Data.Tables[0].Rows[0].Field<int?>(18)!,
                unidadAdministrativaCumplimiento = response.Data.Tables[0].Rows[0].IsNull(19)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(19)!,
                oficioResolucionImpugnada = response.Data.Tables[0].Rows[0].IsNull(20)
                     ? null! : response.Data.Tables[0].Rows[0].Field<string?>(20)!,
                fechaOficioResolucionImpugnada = response.Data.Tables[0].Rows[0].IsNull(21) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(21).ToString("dd/MM/yyyy"),
                idUnidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(22)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(22),
                idSubadministracion = response.Data.Tables[0].Rows[0].IsNull(23)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(23),
                idTipoAsuntoCumplimentar = response.Data.Tables[0].Rows[0].IsNull(24)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(24),
                idAbogado = response.Data.Tables[0].Rows[0].IsNull(25)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(25),
                idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(26)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(26),
                idEstadoTarea = response.Data.Tables[0].Rows[0].IsNull(27)
                     ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(27),
                externoCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(28)
                     ? false : response.Data.Tables[0].Rows[0].Field<bool>(28),

            };
        }

        public async Task<ResultTransaction> UpdateCumplimentacionAsync(Autorizacion entityAutorizacion, Cumplimentacion entityCumplimentacion)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_externo_cumplimentacion",
                    NpgsqlDbType.Boolean,
                    entityAutorizacion.externo_cumplimentacion!
                ),
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entityAutorizacion.id!
                ),
                new (
                    "p_id_tipo_asunto",
                    NpgsqlDbType.Integer,
                    entityAutorizacion.id_tipo_asunto!
                ),
                new (
                    "p_rfc",
                    NpgsqlDbType.Text,
                    entityAutorizacion.rfc!
                ),
                new (
                    "p_promovente",
                    NpgsqlDbType.Text,
                    entityAutorizacion.promovente!
                ),
                new (
                    "p_promovente_no_contribuyente",
                    NpgsqlDbType.Boolean,
                    entityAutorizacion.promovente_no_contribuyente!
                ),
                new (
                    "p_rfc_contribuyente",
                    NpgsqlDbType.Text,
                    entityAutorizacion.rfc_contribuyente!
                ),
                new (
                    "p_contribuyente",
                    NpgsqlDbType.Text,
                    entityAutorizacion.contribuyente!
                ),
                new (
                    "p_id",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id!
                ),
                new (
                    "p_numero_juicio",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.no_juicio_recuro_amparo!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_recepcion_solicitud!
                ),
                new (
                    "p_fecha_firmeza",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_firmeza!
                ),
                new (
                    "p_id_plaza_cumplimento",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_plazo_cumplimiento!
                ),
                new (
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_vencimiento!
                ),
                new (
                    "p_id_organo_jurisdiccional",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_organo_jurisdiccional!
                ),
                new (
                    "p_organo_jurisdiccional",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.organo_jurisdiccional!
                ),
                new (
                    "p_id_unidad_administrativa_cumplimiento",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_unidad_administrativa_cumplimiento!
                ),
                new (
                    "p_unidad_administrativa_cumplimiento",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.unidad_administrativa_cumplimiento!
                ),
                new (
                    "p_oficio_resolucion_impugnada",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.oficio_resolucion_impugnada!
                ),
                new (
                    "p_fecha_oficio_resolucion_impugnada",
                    NpgsqlDbType.Date,
                    entityCumplimentacion.fecha_oficio_resolucion_impugnada!
                ),
                new (
                    "p_id_subadministracion",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_subadministracion!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_estado_tarea!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_estado_procesal!
                ),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.usuario_modificacion!
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenCumplimentacionUpdate,
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

        public async Task<ResponseResolucionCumplimentacion> GetResolucionCumplimentacionDisconnectedAsync(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenResolucionCumplimentacionDisconnectedById,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsunto = response.Data.Tables[0].Rows[0].IsNull(1) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(1),
                fechaVencimiento = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(2).ToString("dd/MM/yyyy")!,
                oficioResolucionCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                fechaOficioResolucionCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(4).ToString("dd/MM/yyyy"),
                idSentidoCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(5) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(5),
                fechaNotificacionCumplimentacion = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(6).ToString("dd/MM/yyyy"),
            };
        }

        public async Task<List<ResponseResolucionCumplimentacion>> GetResolucionCumplimentacions(int idCumplimentacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenResolucionCumplimentacionDisconnected,
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

            List<ResponseResolucionCumplimentacion> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? 0 : item.Field<int>(1),
                        fechaVencimiento = item.IsNull(2) ? null! : item.Field<DateTime>(2).ToString("dd/MM/yyyy"),
                        oficioResolucionCumplimentacion = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        fechaOficioResolucionCumplimentacion = item.IsNull(4) ? null! : item.Field<DateTime>(4).ToString("dd/MM/yyyy"),
                        idSentidoCumplimentacion = item.IsNull(5) ? 0 : item.Field<int>(5),
                        //sentidoCumplimentacion = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        fechaNotificacionCumplimentacion = item.IsNull(6) ? null! : item.Field<DateTime>(6).ToString("dd/MM/yyyy"),
                    });
            }
            return resultList;
        }

        public async Task<ResultTransaction> UpdateImprocedenciaAsync(Cumplimentacion entityCumplimentacion, Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_estado_tarea!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityCumplimentacion.id_estado_procesal!
                ),
                new (
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityCumplimentacion.usuario_modificacion!
                ),
                new ("p_id_tipo_documento_oficio", NpgsqlDbType.Integer, entityDocumento.id_tipo_documento!),
                new ("p_id_seccion_oficio", NpgsqlDbType.Integer, entityDocumento.id_seccion!),
                new ("p_id_unidad_administrativa_oficio", NpgsqlDbType.Integer, entityDocumento.id_unidad_administrativa!),
                new ("p_file_name_oficio", NpgsqlDbType.Text, entityDocumento.file_name!),
                new ("p_file_path_oficio", NpgsqlDbType.Text, entityDocumento.file_path !),
                new ("p_content_type_oficio", NpgsqlDbType.Text,entityDocumento.content_type!),
                new ("p_file_size_oficio", NpgsqlDbType.Text, entityDocumento.file_size!),
                new ("p_owner_name_oficio", NpgsqlDbType.Text, entityDocumento.owner_name!),
                new ("p_numero_folio_oficio", NpgsqlDbType.Text, entityDocumento.numero_folio!),
                new ("p_permanente_oficio", NpgsqlDbType.Boolean, entityDocumento.permanente!),
                new ("p_id_rol_oficio", NpgsqlDbType.Integer, entityDocumento.id_rol!),
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.GenCumplimentacionImprocedenciaUpdate,
                dataFile,
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
    }
}