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
    public class RequerimientoProdeconRepository : IRequerimientoProdeconRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public RequerimientoProdeconRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddAsync(RequerimientoProdecon entity, Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_autorizacion!
                ),
                new (
                    "p_numero_oficio",
                    NpgsqlDbType.Text,
                    entity.numero_oficio!
                ),
                new (
                    "p_numero_expediente",
                    NpgsqlDbType.Text,
                    entity.numero_expediente!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entity.fecha_oficio!
                ),
                new (
                    "p_fecha_ingreso_sat",
                    NpgsqlDbType.Date,
                    entity.fecha_ingreso_sat!),
                new (
                    "p_requiere_accion",
                    NpgsqlDbType.Boolean,
                    entity.requiere_accion!
                ),
                new (
                    "p_atencion",
                    NpgsqlDbType.Text,
                    entity.atencion!),
                new (
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entity.usuario_creacion!
                ),
                new ("p_id_tipo_documento", NpgsqlDbType.Integer, entityDocumento.id_tipo_documento!),
                new ("p_id_seccion", NpgsqlDbType.Integer, entityDocumento.id_seccion!),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, entityDocumento.id_unidad_administrativa!),
                new ("p_file_name", NpgsqlDbType.Text, entityDocumento.file_name!),
                new ("p_file_path", NpgsqlDbType.Text, entityDocumento.file_path !),
                new ("p_content_type", NpgsqlDbType.Text, entityDocumento.content_type!),
                new ("p_file_size", NpgsqlDbType.Text, entityDocumento.file_size!),
                new ("p_owner_name", NpgsqlDbType.Text, entityDocumento.owner_name!),
                new ("p_numero_folio", NpgsqlDbType.Text, entityDocumento.numero_folio!),
                new ("p_permanente", NpgsqlDbType.Boolean, entityDocumento.permanente!),
                new ("p_id_rol", NpgsqlDbType.Integer, entityDocumento.id_rol!),
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.GenRequerimientoProdeconCreate,
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

        public async Task<ResultTransaction> UpdateAsync(RequerimientoProdecon entity)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id",
                    NpgsqlDbType.Integer,
                    entity.id!
                ),
                new (
                    "p_numero_oficio",
                    NpgsqlDbType.Text,
                    entity.numero_oficio!
                ),
                new (
                    "p_numero_expediente",
                    NpgsqlDbType.Text,
                    entity.numero_expediente!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entity.fecha_oficio!
                ),
                new (
                    "p_fecha_ingreso_sat",
                    NpgsqlDbType.Date,
                    entity.fecha_ingreso_sat!),
                new (
                    "p_requiere_accion",
                    NpgsqlDbType.Boolean,
                    entity.requiere_accion!
                ),
                new (
                    "p_atencion",
                    NpgsqlDbType.Text,
                    entity.atencion!),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entity.usuario_modificacion!
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoProdeconUpdate,
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

        public async Task<List<ResponseRequerimientoProdecon>> GetRequerimientoProdecon(int idAutorizacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoProdeconDisconnectedList,
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

            List<ResponseRequerimientoProdecon> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? 0 : item.Field<int>(1),
                        numeroOficio = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        numeroExpediente = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        fechaOficio = item.IsNull(4) ? null! : item.Field<DateTime>(4)!.ToString("dd/MM/yyyy")!,
                        fechaIngreso_sat = item.IsNull(5) ? null! : item.Field<DateTime>(5).ToString("dd/MM/yyyy")!,
                        requiereAccion = !item.IsNull(6) && item.Field<bool>(6),
                        atencion = item.IsNull(7) ? null! : item.Field<string>(7),
                    }
                );
            }

            return resultList;
        }

        public async Task<ResponseRequerimientoProdecon> GetRequerimientoProdeconByIdDisconnected(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoProdeconByIdDisconnected,
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
                numeroOficio = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                numeroExpediente = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                fechaOficio = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(4)!.ToString("dd/MM/yyyy")!,
                fechaIngreso_sat = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(5).ToString("dd/MM/yyyy")!,
                requiereAccion = !response.Data.Tables[0].Rows[0].IsNull(6) && response.Data.Tables[0].Rows[0].Field<bool>(6),
                atencion = response.Data.Tables[0].Rows[0].IsNull(7) ? null! : response.Data.Tables[0].Rows[0].Field<string>(7),
            };
        }

        public async Task<RequerimientoProdecon> GetRequerimientoProdeconById(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoProdeconById,
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
                id_autorizacion = response.Data.Tables[0].Rows[0].IsNull(1) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(1),
                numero_oficio = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                numero_expediente = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                fecha_oficio = response.Data.Tables[0].Rows[0].IsNull(4) ? new() : response.Data.Tables[0].Rows[0].Field<DateTime>(4)!,
                fecha_ingreso_sat = response.Data.Tables[0].Rows[0].IsNull(5) ? new() : response.Data.Tables[0].Rows[0].Field<DateTime>(5)!,
                requiere_accion = !response.Data.Tables[0].Rows[0].IsNull(6) && response.Data.Tables[0].Rows[0].Field<bool>(6),
                atencion = response.Data.Tables[0].Rows[0].IsNull(7) ? null! : response.Data.Tables[0].Rows[0].Field<string>(7),
                activo = !response.Data.Tables[0].Rows[0].IsNull(8) && response.Data.Tables[0].Rows[0].Field<bool>(8),
            };
        }

        public async Task<int> GetByIdAutorizacionCount(int idAutorizacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoProdeconByIdAutorizacionCount,
                parameters!
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return 0;
            }

            return  response.Data.Tables[0].Rows[0].IsNull(0) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(0);
        }
    }
}
