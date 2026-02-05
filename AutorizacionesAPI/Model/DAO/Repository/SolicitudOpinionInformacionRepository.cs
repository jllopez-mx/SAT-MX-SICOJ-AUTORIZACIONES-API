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
    public class SolicitudOpinionInformacionRepository : ISolicitudOpinionInformacionRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion  

        #region Constructor
        public SolicitudOpinionInformacionRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        #region Solicitud de Opinión e Información
        public async Task<ResultTransaction> AddComercioExteriorSolicitudOpinionInformacionAsync(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile, Autorizacion entityComercio)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.idAutorizacion!),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, entity.idUnidadAdministrativa!),
                new ("p_no_oficio_solicitud", NpgsqlDbType.Text, entity.noOficioSolicitud!),
                new ("p_fecha_oficio_solicitud", NpgsqlDbType.Date, entity.fechaOficioSolicitud!),
                new ("p_unidad_interna", NpgsqlDbType.Boolean, entity.unidadInterna!),
                new ("p_unidad_administrativa_externa", NpgsqlDbType.Text, entity.unidadAdministrativaExterna!),
                new ("p_id_tipo_asunto", NpgsqlDbType.Integer,  entityComercio.id_tipo_asunto),
                new ("p_usuario_creacion",NpgsqlDbType.Text,entity.usuario_creacion!),

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
            };

            var response = await _database.ExecuteFunctionFileAsync(
               EnumFunctions.GenSolicitudOpinionInformacionCreate, dataFile, parameters);

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

        public async Task<ResultTransaction> UpdateComercioExteriorSolicitudOpinioninInformacionAsync(SolicitudOpinionInformacion entity, Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, entity.id!),
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.idAutorizacion!),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, entity.idUnidadAdministrativa!),
                new ("p_no_oficio_solicitud", NpgsqlDbType.Text, entity.noOficioSolicitud!),
                new ("p_no_oficio_respuesta", NpgsqlDbType.Text, entity.noOficioRespuesta),
                new ("p_atendio_solicitud", NpgsqlDbType.Boolean, entity.atendioSolicitud),
                new ("p_fecha_oficio_respuesta", NpgsqlDbType.Date, entity.fechaOficioRespuesta),
                new ("p_fecha_oficio_solicitud", NpgsqlDbType.Date, entity.fechaOficioSolicitud!),
                new ("p_fecha_recepcion", NpgsqlDbType.Date, entity.fechaRecepcion),
                new ("p_unidad_interna", NpgsqlDbType.Boolean, entity.unidadInterna!),
                new ("p_unidad_administrativa_externa", NpgsqlDbType.Text, entity.unidadAdministrativaExterna!),
                new ("p_id_archivo", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id!),
                new ("p_archivo", NpgsqlDbType.Boolean, entityDocumento is not null),
                new ("p_usuario_modificacion", NpgsqlDbType.Text,entity.usuario_modificacion!),

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
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.GenSolicitudOpinionInformacionUpdate, dataFile, parameters);
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

        public async Task<ResponseSolicitudOpinionInformacionAbogadoById> GetComercioExteriorSolicitudOpinionInformacionByIdAsync(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenSolicitudOpinionInformacionByIdDisconnected,
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
                unidadInterna = !response.Data.Tables[0].Rows[0].IsNull(2) && response.Data.Tables[0].Rows[0].Field<bool>(2),
                idUnidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(3) ? (int?)null
                    : response.Data.Tables[0].Rows[0].Field<int?>(3),
                unidadAdministrativaExterna = response.Data.Tables[0].Rows[0].IsNull(4) ? null
                    : response.Data.Tables[0].Rows[0].Field<string>(4),
                unidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(5) ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5)!,
                noOficioSolicitud = response.Data.Tables[0].Rows[0].IsNull(6) ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,
                fechaOficioSolicitud = response.Data.Tables[0].Rows[0].IsNull(7) ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(7).ToString("dd/MM/yyyy"),
                atendioSolicitud = response.Data.Tables[0].Rows[0].IsNull(8) ? null
                    : (bool?)response.Data.Tables[0].Rows[0].Field<bool>(8),
                noOficioRespuesta = response.Data.Tables[0].Rows[0].IsNull(9) ? null
                    : response.Data.Tables[0].Rows[0].Field<string>(9),
                fechaOficioRespuesta = response.Data.Tables[0].Rows[0].IsNull(10) ? null
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(10).ToString("dd/MM/yyyy"),
                fechaRecepcion = response.Data.Tables[0].Rows[0].IsNull(11) ? null
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(11).ToString("dd/MM/yyyy"),
            };
        }

        public async Task<List<ResponseSolicitudOpinionInformacionAbogadoById>> GetComercioExteriorSolicitudOpinionInformacionByIdsAsync(int ids)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, ids),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenSolicitudOpinionInformacionByIds,
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

            List<ResponseSolicitudOpinionInformacionAbogadoById> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new ResponseSolicitudOpinionInformacionAbogadoById
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? 0 : item.Field<int>(1),
                        unidadInterna = !item.IsNull(2) && item.Field<bool>(2),
                        idUnidadAdministrativa = item.IsNull(3) ? (int?)null : item.Field<int?>(3),
                        unidadAdministrativaExterna = item.IsNull(4) ? null : item.Field<string>(4),
                        unidadAdministrativa = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        noOficioSolicitud = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        fechaOficioSolicitud = item.IsNull(7) ? null! : item.Field<DateTime>(7).ToString("dd/MM/yyyy"),
                        atendioSolicitud = item.IsNull(8) ? (bool?)null : item.Field<bool>(8),
                        noOficioRespuesta = item.IsNull(9) ? null : item.Field<string>(9),
                        fechaOficioRespuesta = item.IsNull(10) ? null : item.Field<DateTime>(10).ToString("dd/MM/yyyy"),
                        fechaRecepcion = item.IsNull(11) ? null : item.Field<DateTime>(11).ToString("dd/MM/yyyy")
                    }
                );
            }

            return resultList;
        }

        public async Task<SolicitudOpinionInformacion> GetComercioExteriorSolicitudOpinionByIdAsync(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id)
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenSolicitudOpinionInformacionByIdDisconnected,
                parameters);

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
                idAutorizacion = response.Data.Tables[0].Rows[0].IsNull(1) ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                unidadInterna = !response.Data.Tables[0].Rows[0].IsNull(2) && response.Data.Tables[0].Rows[0].Field<bool>(2),
                idUnidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(3) ? (int?)null
                    : response.Data.Tables[0].Rows[0].Field<int?>(3),
                unidadAdministrativaExterna = response.Data.Tables[0].Rows[0].IsNull(4) ? null
                    : response.Data.Tables[0].Rows[0].Field<string>(4),
                unidad_administrativa = response.Data.Tables[0].Rows[0].IsNull(5) ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5)!,
                noOficioSolicitud = response.Data.Tables[0].Rows[0].IsNull(6) ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,
                fechaOficioSolicitud = response.Data.Tables[0].Rows[0].IsNull(7) ? new()
                   : response.Data.Tables[0].Rows[0].Field<DateTime>(7),
                atendioSolicitud = response.Data.Tables[0].Rows[0].IsNull(8) ? null
                    : (bool?)response.Data.Tables[0].Rows[0].Field<bool>(8),
                noOficioRespuesta = response.Data.Tables[0].Rows[0].IsNull(9) ? null
                    : response.Data.Tables[0].Rows[0].Field<string>(9),
                fechaOficioRespuesta = response.Data.Tables[0].Rows[0].IsNull(10) ? null
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(10),
                fechaRecepcion = response.Data.Tables[0].Rows[0].IsNull(11) ? null
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(11),
            };
        }

        public async Task<ResultTransaction> UpdateComercioExteriorNoSolicitudOpinionInformacionAsync(Autorizacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.id),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, entity.usuario_modificacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenNoSolicitudOpinionInformacion,
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
        #endregion
    }
}

