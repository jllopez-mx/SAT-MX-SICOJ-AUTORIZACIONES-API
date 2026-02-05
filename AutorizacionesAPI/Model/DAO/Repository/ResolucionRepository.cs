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
    public class ResolucionRepository : IResolucionRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public ResolucionRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddAsync(Autorizacion entityAutorizaciones, Cumplimentacion entityCumplimentacion, Resolucion entity, Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_autorizacion!
                ),
                new (
                    "p_id_cumplimentacion",
                    NpgsqlDbType.Integer,
                    entity.id_cumplimentacion!
                ),
                new (
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entity.fecha_vencimiento!
                ),
                new (
                    "p_oficio_resolucion",
                    NpgsqlDbType.Text,
                    entity.oficio_resolucion!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entity.fecha_oficio!
                ),
                new (
                    "p_id_sentido",
                    NpgsqlDbType.Integer,
                    entity.id_sentido!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones is null ? entityCumplimentacion.id_estado_tarea :entityAutorizaciones.id_estado_tarea!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones is null ? entityCumplimentacion.id_estado_procesal :entityAutorizaciones.id_estado_procesal!
                ),
                new (
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entity.usuario_creacion!
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
                EnumFunctions.GenResolucionCreate,
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

        public async Task<ResultTransaction> UpdateAsync(Autorizacion entityAutorizaciones, Cumplimentacion entityCumplimentacion, Resolucion entity, Documento entityDocumento, DataFile dataFile, int idSeccion)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id",
                    NpgsqlDbType.Integer,
                    entity.id!
                ),
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_autorizacion
                ),
                new (
                    "p_id_cumplimentacion",
                    NpgsqlDbType.Integer,
                    entity.id_cumplimentacion
                ),
                new (
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entity.fecha_vencimiento!
                ),
                new (
                    "p_oficio_resolucion",
                    NpgsqlDbType.Text,
                    entity.oficio_resolucion!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entity.fecha_oficio!
                ),
                new (
                    "p_id_sentido",
                    NpgsqlDbType.Integer,
                    entity.id_sentido!
                ),
                new (
                    "p_fecha_notificacion",
                    NpgsqlDbType.Date,
                    entity.fecha_notificacion!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones is null ? entityCumplimentacion.id_estado_tarea :entityAutorizaciones.id_estado_tarea!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones is null ? entityCumplimentacion.id_estado_procesal :entityAutorizaciones.id_estado_procesal!
                ),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entity.usuario_modificacion!
                ),
                new (
                    "p_archivo",
                    NpgsqlDbType.Boolean,
                    entityDocumento is not null
                ),
                new ("p_id_tipo_documento", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!),
                new ("p_id_seccion", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_unidad_administrativa!),
                new ("p_file_name", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.file_name!),
                new ("p_file_path", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.file_path !),
                new ("p_content_type", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.content_type!),
                new ("p_file_size", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.file_size!),
                new ("p_owner_name", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!),
                new ("p_numero_folio", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.numero_folio!),
                new ("p_permanente", NpgsqlDbType.Boolean, entityDocumento is null ? DBNull.Value : entityDocumento.permanente!),
                new ("p_id_rol", NpgsqlDbType.Integer,entityDocumento is null ? DBNull.Value :  entityDocumento.id_rol!),
                new ("p_id_seccion_modificacion", NpgsqlDbType.Integer, idSeccion),
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.GenResolucionUpdate,
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

        public async Task<List<Resolucion>> GetByIdAusunto(int? idAutorizacion, int? idCumplimentacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenResolucionByIdAsunto,
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

            List<Resolucion> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_autorizacion = item.IsNull(1) ? null! : item.Field<int>(1),
                        id_cumplimentacion = item.IsNull(2) ? null! : item.Field<int>(2),
                        fecha_vencimiento = item.IsNull(3) ? new() : item.Field<DateTime>(3)!,
                        oficio_resolucion = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        fecha_oficio = item.IsNull(5) ? new()! : item.Field<DateTime>(5),
                        id_sentido = item.IsNull(6) ? 0 : item.Field<int>(6),
                        fecha_notificacion = item.IsNull(7) ? null! : item.Field<DateTime>(7),
                        activo = !item.IsNull(8) && item.Field<bool>(8),
                    });
            }

            return resultList;
        }

        public async Task<List<ResponseResolucion>> GetByIdAsuntoDisconnected(int? idAutorizacion, int? idCumplimentacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenResolucionByIdAsuntoDisconnected,
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

            List<ResponseResolucion> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ?
                    item.IsNull(7) ? 0 : item.Field<int>(7)
                    : item.Field<int>(1),
                        fechaVencimiento = item.IsNull(2) ? null! : item.Field<DateTime>(2).ToString("dd/MM/yyyy")!,
                        oficioResolucion = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        fechaOficio = item.IsNull(4) ? null! : item.Field<DateTime>(4).ToString("dd/MM/yyyy"),
                        idSentido = item.IsNull(5) ? 0 : item.Field<int>(5),
                        fechaNotificacion = item.IsNull(6) ? null! : item.Field<DateTime>(6).ToString("dd/MM/yyyy"),
                    });
            }

            return resultList;
        }

        public async Task<List<ResponseAvisoConRespuesta>> GetAvisoConRespuestaDisconnected(int idAutorizacionRelacionada)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion_relacionada", NpgsqlDbType.Integer, idAutorizacionRelacionada),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoConRespuetaDisconnected,
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

            List<ResponseAvisoConRespuesta> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? 0 : item.Field<int>(1),
                        asunto = item.IsNull(2) ? null! : item.Field<string>(2),
                        fechaVencimiento = item.IsNull(3) ? null! : item.Field<DateTime>(3).ToString("dd/MM/yyyy"),
                        oficioResolucion = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        fechaOficio = item.IsNull(5) ? null! : item.Field<DateTime>(5).ToString("dd/MM/yyyy"),
                        fechaNotificacion = item.IsNull(6) ? null! : item.Field<DateTime>(6).ToString("dd/MM/yyyy"),
                        idAsuntoRelacionado = item.IsNull(7) ? 0 : item.Field<int>(7),
                        idTipoSentido = item.IsNull(8) ? 0 : item.Field<int>(8),
                        tipoSentido = item.IsNull(9) ? null! : item.Field<string>(9),
                    });
            }

            return resultList;
        }

        public async Task<ResponseAvisoConRespuesta> GetAvisoConRespuestaByIdDisconnected(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoConRespuetaById,
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
                asunto = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2),
                fechaVencimiento = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(3).ToString("dd/MM/yyyy"),
                oficioResolucion = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                fechaOficio = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(5).ToString("dd/MM/yyyy"),
                fechaNotificacion = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(6).ToString("dd/MM/yyyy"),
                idAsuntoRelacionado = response.Data.Tables[0].Rows[0].IsNull(7) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(7),
                idTipoSentido = response.Data.Tables[0].Rows[0].IsNull(8) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(8),
                tipoSentido = response.Data.Tables[0].Rows[0].IsNull(9) ? null! : response.Data.Tables[0].Rows[0].Field<string>(9),
            };
        }

        public async Task<ResultTransaction> DescartarAsync(Autorizacion entityAutorizaciones, Descartar entity, List<int>? listaSecciones)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones.id!
                ),
                new ("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion),
                new ("p_id_seccion_archivo", NpgsqlDbType.Array | NpgsqlDbType.Integer, (listaSecciones is null || !listaSecciones.Any()) ? DBNull.Value :  listaSecciones.ToArray()),
                new ("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAutorizaciones.id_estado_procesal!),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityAutorizaciones.usuario_modificacion!
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AgResolucionDescartar, parameters!
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
