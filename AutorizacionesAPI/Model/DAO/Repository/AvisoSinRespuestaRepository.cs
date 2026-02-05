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
    public class AvisoSinRespuestaRepository : IAvisoSinRespuestaRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AvisoSinRespuestaRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddAsync(Autorizacion entityAutorizaciones, AvisoSinRespuesta entity, Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_autorizacion!
                ),
                new (
                    "p_id_autorizacion_relacionado",
                    NpgsqlDbType.Integer,
                    entity.id_autorizacion_relacionado!
                ),
                new (
                    "p_no_asunto_externo",
                    NpgsqlDbType.Text,
                    entity.no_asunto_externo!
                ),
                new (
                    "p_id_tipo_aviso",
                    NpgsqlDbType.Integer,
                    entity.id_tipo_aviso!
                ),
                new (
                    "p_fecha_ingreso",
                    NpgsqlDbType.Date,
                    entity.fecha_ingreso!
                ),
                new (
                    "p_observaciones",
                    NpgsqlDbType.Text,
                    entity.observaciones!),
                new (
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entity.usuario_creacion!
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
                    "p_id_tipo_asunto",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones.id_tipo_asunto!
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
                EnumFunctions.GenAvisoSinRespuestaCreate,
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

        public async Task<ResultTransaction> UpdateAsync(Autorizacion entityAutorizaciones, AvisoSinRespuesta entityAviso, List<string> folios, Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, entityAviso.id!),
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entityAviso.id_autorizacion!
                ),
                new (
                    "p_id_tipo_aviso",
                    NpgsqlDbType.Integer,
                    entityAviso.id_tipo_aviso!
                ),
                new (
                    "p_fecha_ingreso",
                    NpgsqlDbType.Date,
                    entityAviso.fecha_ingreso!
                ),
                new (
                    "p_observaciones",
                    NpgsqlDbType.Text,
                    entityAviso.observaciones!),
                new (
                    "p_folio",
                    NpgsqlDbType.Array | NpgsqlDbType.Text,
                    folios),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityAviso.usuario_modificacion!
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
                    "p_id_tipo_asunto",
                    NpgsqlDbType.Integer,
                    entityAutorizaciones.id_tipo_asunto!
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
                EnumFunctions.GenAvisoSinRespuestaUpdate,
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

        public async Task<List<ResponseAvisoSinRespuestaRelacionado>> GetAutorizacionRelacionada(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoSinRespuestaHistoricoDisconnected,
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

            List<ResponseAvisoSinRespuestaRelacionado> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? 0 : item.Field<int>(1),
                        asunto = item.IsNull(2) ? null! : item.Field<string>(2),
                        idAsuntoRelacionado = item.IsNull(3) ? 0 : item.Field<int>(3),
                        idTipoAviso = item.IsNull(4) ? 0 : item.Field<int>(4),
                        tipoAviso = item.IsNull(5) ? null! : item.Field<string>(5),
                        fechaIngreso = item.IsNull(6) ? null! : item.Field<DateTime>(6).ToString("dd/MM/yyyy")!,
                        observaciones = item.IsNull(7) ? null! : item.Field<string>(7),
                        aprobado = response.Data.Tables[0].Rows[0].IsNull(8) ? false! : response.Data.Tables[0].Rows[0].Field<bool>(8),
                        idTipoAsunto = item.IsNull(9) ? 0 : item.Field<int>(9),
                        tipoAsunto = item.IsNull(10) ? null! : item.Field<string>(10),
                    }
                );
            }

            return resultList;
        }

        public async Task<AvisoSinRespuesta> GetById(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoSinRespuestaById,
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
                id_autorizacion_relacionado = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<int>(2),
                id_tipo_aviso = response.Data.Tables[0].Rows[0].IsNull(3) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(3),
                fecha_ingreso = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(4),
                observaciones = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<string>(5),
                activo = response.Data.Tables[0].Rows[0].IsNull(6) ? false : response.Data.Tables[0].Rows[0].Field<bool>(6),
                aprobado = response.Data.Tables[0].Rows[0].IsNull(7) ? false! : response.Data.Tables[0].Rows[0].Field<bool>(7),
            };
        }

        public async Task<AvisoSinRespuesta> GetByIdAutorizacion(int idAutorizacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoSinRespuestaByIdAutorizacion,
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
                id_autorizacion_relacionado = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<int>(2),
                id_tipo_aviso = response.Data.Tables[0].Rows[0].IsNull(3) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(3),
                fecha_ingreso = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(4),
                observaciones = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<string>(5),
                activo = response.Data.Tables[0].Rows[0].IsNull(6) ? false : response.Data.Tables[0].Rows[0].Field<bool>(6),
                aprobado = response.Data.Tables[0].Rows[0].IsNull(7) ? false! : response.Data.Tables[0].Rows[0].Field<bool>(7),
                no_asunto_externo = response.Data.Tables[0].Rows[0].IsNull(8) ? null! : response.Data.Tables[0].Rows[0].Field<string>(8),
            };
        }

        public async Task<ResponseAvisoSinRespuestaRelacionado> GetByIdDisconnected(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoSinRespuestaDisconnectedById,
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
                idAsuntoRelacionado = response.Data.Tables[0].Rows[0].IsNull(3) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(3),
                idTipoAviso = response.Data.Tables[0].Rows[0].IsNull(4) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(4),                
                fechaIngreso = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(5).ToString("dd/MM/yyyy")!,
                observaciones = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<string>(6),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(7) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(7),
                aprobado = response.Data.Tables[0].Rows[0].IsNull(8) ? false! : response.Data.Tables[0].Rows[0].Field<bool>(8),
            };
        }

        public async Task<ResponseAvisoSinRespuestaRelacionado> GetByIdAutorizacionDisconnected(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoSinRespuestaDisconnectedByIdAutorizacion,
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
                idAsuntoRelacionado = response.Data.Tables[0].Rows[0].IsNull(3) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(3),
                idTipoAviso = response.Data.Tables[0].Rows[0].IsNull(4) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(4),
                fechaIngreso = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(5).ToString("dd/MM/yyyy")!,
                observaciones = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<string>(6),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(7) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(7),
                aprobado = response.Data.Tables[0].Rows[0].IsNull(8) ? false! : response.Data.Tables[0].Rows[0].Field<bool>(8),
            };
        }

        public async Task<ResponseAvisoSinRespuestaRelacionado> GetByIdAsuntoDisconnected(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoSinRespuestaDisconnectedByIdAsunto,
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
                idAsuntoRelacionado = response.Data.Tables[0].Rows[0].IsNull(3) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(3),
                idTipoAviso = response.Data.Tables[0].Rows[0].IsNull(4) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(4),
                fechaIngreso = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(5).ToString("dd/MM/yyyy")!,
                observaciones = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<string>(6),
                idTipoAsunto = response.Data.Tables[0].Rows[0].IsNull(7) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(7),
                aprobado = response.Data.Tables[0].Rows[0].IsNull(8) ? false! : response.Data.Tables[0].Rows[0].Field<bool>(8),
            };
        }

        public async Task<List<AvisoSinRespuestaFolio>> GetFolios(int[] idAvisoSinRespuesta)
        {
            ParameterPGsql[] parameters =
            {
               new ("p_id_aviso_sin_respuesta", NpgsqlDbType.Array | NpgsqlDbType.Integer, idAvisoSinRespuesta),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoSinRespuestaFolios,
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

            List<AvisoSinRespuestaFolio> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_aviso_sin_respuesta = item.IsNull(1) ? 0 : item.Field<int>(1),
                        folio = item.IsNull(2) ? null! : item.Field<string>(2)!,
                    }
                );
            }

            return resultList;
        }
    }
}
