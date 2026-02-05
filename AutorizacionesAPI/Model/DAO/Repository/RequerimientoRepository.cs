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
    public class RequerimientoRepository : IRequerimientoRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public RequerimientoRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddAsync(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_autorizacion!
                ),
                new (
                    "p_oficio_requerimiento",
                    NpgsqlDbType.Text,
                    entity.oficio_requerimiento!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entity.fecha_oficio!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones.id_estado_tarea!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones.id_estado_procesal!
                ),
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
                new ("p_content_type", NpgsqlDbType.Text,entityDocumento.content_type!),
                new ("p_file_size", NpgsqlDbType.Text, entityDocumento.file_size!),
                new ("p_owner_name", NpgsqlDbType.Text, entityDocumento.owner_name!),
                new ("p_numero_folio", NpgsqlDbType.Text, entityDocumento.numero_folio!),
                new ("p_permanente", NpgsqlDbType.Boolean, entityDocumento.permanente!),
                new ("p_id_rol", NpgsqlDbType.Integer, entityDocumento.id_rol!),
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.GenRequerimientoCreate,
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

        public async Task<ResultTransaction> UpdateAsync(Autorizacion entityAutorizaciones, Requerimiento entity, Documento entityDocumento, DataFile dataFile, int idSeccion)
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
                    entity.id_autorizacion!
                ),
                new (
                    "p_oficio_requerimiento",
                    NpgsqlDbType.Text,
                    entity.oficio_requerimiento!
                ),
                new (
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entity.fecha_oficio!
                ),
                new (
                    "p_fecha_notificacion",
                    NpgsqlDbType.Date,
                    entity.fecha_notificacion!
                ),
                new (
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entity.fecha_vencimiento!
                ),
                new (
                    "p_primera_prorroga",
                    NpgsqlDbType.Boolean,
                    entity.primer_prorroga!
                ),
                new (
                    "p_segunda_prorroga",
                    NpgsqlDbType.Boolean,
                    entity.segunda_prorroga!
                ),
                new (
                    "p_atendio_requerimiento",
                    NpgsqlDbType.Boolean,
                    entity.atendio_requerimiento!
                ),
                new (
                    "p_fecha_atencion",
                    NpgsqlDbType.Date,
                    entity.fecha_atencion!
                ),
                new (
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones.id_estado_tarea!
                ),
                new (
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones.id_estado_procesal!
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
                EnumFunctions.GenRequerimientoUpdate,
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

        public async Task<Requerimiento> GetById(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoById,
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
                oficio_requerimiento = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                fecha_oficio = response.Data.Tables[0].Rows[0].IsNull(3) ? new()! : response.Data.Tables[0].Rows[0].Field<DateTime>(3),
                fecha_notificacion = response.Data.Tables[0].Rows[0].IsNull(4) ? new()! : response.Data.Tables[0].Rows[0].Field<DateTime>(4),
                fecha_vencimiento = response.Data.Tables[0].Rows[0].IsNull(5) ? new()! : response.Data.Tables[0].Rows[0].Field<DateTime>(5),
                atendio_requerimiento = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<bool>(6),
                fecha_atencion = response.Data.Tables[0].Rows[0].IsNull(7) ? new()! : response.Data.Tables[0].Rows[0].Field<DateTime>(7),
                activo = !response.Data.Tables[0].Rows[0].IsNull(8) && response.Data.Tables[0].Rows[0].Field<bool>(8),
                primer_prorroga = response.Data.Tables[0].Rows[0].IsNull(9) ? null! : response.Data.Tables[0].Rows[0].Field<bool>(9),
                segunda_prorroga = !response.Data.Tables[0].Rows[0].IsNull(10) && response.Data.Tables[0].Rows[0].Field<bool>(10),
            };
        }

        public async Task<ResponseRequerimiento> GetByIdDisconnected(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoByIdDisconnected,
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
                oficioRequerimiento = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                fechaOficio = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(3).ToString("dd/MM/yyyy"),
                fechaNotificacion = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(4).ToString("dd/MM/yyyy"),
                fechaVencimiento = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(5).ToString("dd/MM/yyyy"),
                atendioRequerimiento = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<bool>(6),
                fechaAtencion = response.Data.Tables[0].Rows[0].IsNull(7) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(7).ToString("dd/MM/yyyy"),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(8) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(8),
                primeraProrroga = response.Data.Tables[0].Rows[0].IsNull(9) ? null! : response.Data.Tables[0].Rows[0].Field<bool>(9),
                segundaProrroga = response.Data.Tables[0].Rows[0].IsNull(10) ? null! : response.Data.Tables[0].Rows[0].Field<bool>(10),
            };
        }

        public async Task<List<ResponseRequerimiento>> GetByIdAutorizacionDisconnected(int idAutorizacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoDisconnected,
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

            List<ResponseRequerimiento> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? 0 : item.Field<int>(1),
                        oficioRequerimiento = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        fechaOficio = item.IsNull(3) ? null! : item.Field<DateTime>(3).ToString("dd/MM/yyyy"),
                        fechaNotificacion = item.IsNull(4) ? null! : item.Field<DateTime>(4).ToString("dd/MM/yyyy"),
                        fechaVencimiento = item.IsNull(5) ? null! : item.Field<DateTime>(5).ToString("dd/MM/yyyy"),
                        atendioRequerimiento = item.IsNull(6) ? null! : item.Field<bool>(6),
                        fechaAtencion = item.IsNull(7) ? null! : item.Field<DateTime>(7).ToString("dd/MM/yyyy"),
                        idTipoAsunto = item.IsNull(8) ? 0 : item.Field<int>(8),
                        tipoAsunto = item.IsNull(9) ? null! : item.Field<string>(9),
                        primeraProrroga = response.Data.Tables[0].Rows[0].IsNull(10) ? null! : response.Data.Tables[0].Rows[0].Field<bool>(10),
                        segundaProrroga = response.Data.Tables[0].Rows[0].IsNull(11) ? null! : response.Data.Tables[0].Rows[0].Field<bool>(11),
                    });
            }

            return resultList;
        }

        public async Task<List<Requerimiento>> GetByIdAutorizacion(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRequerimientoByIdAutorizacion,
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

            List<Requerimiento> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_autorizacion = item.IsNull(1) ? 0 : item.Field<int>(1),
                        oficio_requerimiento = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        fecha_oficio = item.IsNull(3) ? new()! : item.Field<DateTime>(3),
                        fecha_notificacion = item.IsNull(4) ? new()! : item.Field<DateTime>(4),
                        fecha_vencimiento = item.IsNull(5) ? new()! : item.Field<DateTime>(5),
                        atendio_requerimiento = item.IsNull(6) ? null! : item.Field<bool>(6),
                        fecha_atencion = item.IsNull(7) ? new()! : item.Field<DateTime>(7),
                        activo = !item.IsNull(8) && item.Field<bool>(8),
                    });
            }
            return resultList;
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
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entityAutorizaciones.usuario_modificacion!),
                new ("p_fecha_vencimiento", NpgsqlDbType.Date, entityAutorizaciones.fecha_vencimiento),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AgRequerimientoDescartar,
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

        public async Task<ResultTransaction> DescartarUltimoAsync(Requerimiento entity, int idSeccionRequerimiento)
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
                    entity.id_autorizacion!
                ),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entity.usuario_modificacion!
                ),
                new ("p_id_seccion", NpgsqlDbType.Integer,idSeccionRequerimiento),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AgRequerimientoDescartarUltimo,
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
