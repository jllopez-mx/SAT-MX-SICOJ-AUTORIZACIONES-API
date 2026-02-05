using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;
using System;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class DocumentoRepository : IDocumentoRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public DocumentoRepository(ISqlTools database)
        {
            _database =
                database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddAsync(Documento entity, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.id_autorizacion!),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, entity.id_cumplimentacion!),
                new ("p_id_tipo_documento", NpgsqlDbType.Integer, entity.id_tipo_documento!),
                new ("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion!),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, entity.id_renglon_seccion!),
                new ("p_id_unidad_administrativa", NpgsqlDbType.Integer, entity.id_unidad_administrativa!),
                new ("p_file_name", NpgsqlDbType.Text, entity.file_name!),
                new ("p_file_path", NpgsqlDbType.Text, entity.file_path!),
                new ("p_content_type", NpgsqlDbType.Text, entity.content_type!),
                new ("p_file_size", NpgsqlDbType.Text, entity.file_size!),
                new ("p_owner_name", NpgsqlDbType.Text, entity.owner_name!),
                new ("p_numero_folio", NpgsqlDbType.Text, entity.numero_folio!),
                new ("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
                new ("p_permanente", NpgsqlDbType.Boolean, entity.permanente!),
                new ("p_id_rol", NpgsqlDbType.Integer, entity.id_rol!),
            };

            var response = await _database.ExecuteFunctionFileAsync(EnumFunctions.DocumentosCreate, dataFile, parameters);
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

        public async Task<ResultTransaction> UpdateAsync(Documento entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id",
                    NpgsqlDbType.Integer,
                    entityDocumento.id!
                ),
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entityDocumento.id_autorizacion!
                ),
                new (
                    "p_id_cumplimentacion",
                    NpgsqlDbType.Integer,
                    entityDocumento.id_cumplimentacion!
                ),
                new (
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityDocumento.usuario_modificacion!
                ),
                new (
                    "p_archivo",
                    NpgsqlDbType.Boolean,
                    dataFile is not null
                ),
                new ("p_id_tipo_documento", NpgsqlDbType.Integer, entityDocumento.id_tipo_documento!),
                new ("p_id_seccion", NpgsqlDbType.Integer,entityDocumento.id_seccion!),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, entityDocumento.id_renglon_seccion!),
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
                EnumFunctions.DocumentosUpdate,
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

        public async Task<List<Documento>> GetByIdsAsync(int[] id)
        {
            ParameterPGsql[] parameters = 
            { 
                new ("p_ids", NpgsqlDbType.Array | NpgsqlDbType.Integer, id)
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.DocumentosByIds,
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

            List<Documento> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0)
                            ? 0
                            : item.Field<int>(0),
                        id_autorizacion = item.IsNull(1)
                            ? null!
                            : item.Field<int>(1),
                        id_tipo_documento = item.IsNull(2)
                            ? 0
                            : item.Field<int>(2),
                        file_name = item.IsNull(3)
                             ? null!
                            : item.Field<string>(3)!,
                        file_path = item.IsNull(4)
                            ? null!
                            : item.Field<string>(4)!,
                        content_type = item.IsNull(5)
                            ? null!
                            : item.Field<string?>(5)!,
                        owner_name = item.IsNull(6)
                            ? null!
                            : item.Field<string>(6)!,
                        numero_folio = item.IsNull(7)
                            ? null!
                            : item.Field<string>(7),
                        fecha_creacion = item.IsNull(8)
                            ? new()
                            : item.Field<DateTime>(8),
                        usuario_creacion = item.IsNull(9)
                            ? null!
                            : item.Field<string>(9),
                        fecha_modificacion = item.IsNull(10)
                            ? null!
                            : item.Field<DateTime?>(10)!,
                        usuario_modificacion = item.IsNull(11)
                            ? null!
                            : item.Field<string>(11),
                        activo = !item.IsNull(12)
                            && item.Field<bool>(12),
                        id_seccion = item.IsNull(13)
                            ? 0
                            : item.Field<int>(13),
                        file_size = item.IsNull(14)
                            ? null!
                            : item.Field<string>(14)!,
                        permanente = !item.IsNull(15)
                            && item.Field<bool>(15)!,
                        id_unidad_administrativa = item.IsNull(16)
                            ? 0
                            : item.Field<int>(16),
                        id_renglon_seccion = item.IsNull(17)
                            ? 0
                            : item.Field<int>(17)!,
                        id_rol = item.IsNull(18)
                            ? 0
                            : item.Field<int>(18)!,
                        id_cumplimentacion = item.IsNull(19)
                            ? null!
                            : item.Field<int>(19)!,
                    }
                );
            }
            return resultList;
        }

        public async Task<List<ResponseDocumentosConFolio>> GetDocumentosFolioDisconnected(bool paginado, int? idAutorizacion, int? idCumplimentacion,  List<int>? idSeccion, int? idRenglonSeccion, bool activo, int? Fetch = null, int? Page = null!, string? OrderByColumn = null!, bool? OrderDesc = null!)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_paginado", NpgsqlDbType.Boolean, paginado),
                new ("p_page_size", NpgsqlDbType.Integer, Fetch),
                new ("p_page", NpgsqlDbType.Integer, Page),
                new ("p_order_column", NpgsqlDbType.Varchar, OrderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, OrderDesc),
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
                new ("p_id_seccion", NpgsqlDbType.Array | NpgsqlDbType.Integer, (idSeccion is null || !idSeccion.Any()) ? DBNull.Value :  idSeccion.ToArray()),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, idRenglonSeccion),
                new ("p_activo", NpgsqlDbType.Boolean, activo),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.DocumentosFolio,
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

            List<ResponseDocumentosConFolio> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? item.IsNull(11) ? null! : item.Field<int>(11) : item.Field<int>(1),
                        IdTipoDocumento = item.IsNull(2) ? 0 : item.Field<int>(2),
                        TipoDocumento = item.IsNull(3) ? null! : item.Field<string>(3),
                        NombreDocumento = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        TamanoDocumento = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        IdSeccion = item.IsNull(6) ? 0 : item.Field<int>(6),
                        Seccion = item.IsNull(7) ? null! : item.Field<string>(7),
                        NumeroFolio = item.IsNull(8) ? null! : item.Field<string>(8)!,
                        IdRenglonSeccion = item.IsNull(9) ? 0 : item.Field<int>(9),
                        Permanente = !item.IsNull(10) && item.Field<bool>(10),
                    }
                );
            }

            return resultList;
        }

        public async Task<int?> GetDocumentosFolioDisconnectedCount(int? idAutorizacion, int? idCumplimentacion, List<int>? idSeccion, int? idRenglonSeccion, bool activo)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
                new ("p_id_seccion", NpgsqlDbType.Array | NpgsqlDbType.Integer, (idSeccion is null || !idSeccion.Any()) ? DBNull.Value :  idSeccion.ToArray()),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, idRenglonSeccion),
                new ("p_activo", NpgsqlDbType.Boolean, activo),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.DocumentosFolioCount,
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

        public async Task<List<ResponseDocumentoHistorico>> GetDocumentosHistoricoDisconnected(bool paginado, int? idAutorizacion, int? idCumplimentacion, List<int>? idSeccion, int? idRenglonSeccion, bool activo, int? Fetch = null, int? Page = null!, string? OrderByColumn = null!, bool? OrderDesc = null!)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_paginado", NpgsqlDbType.Boolean, paginado),
                new ("p_page_size", NpgsqlDbType.Integer, Fetch),
                new ("p_page", NpgsqlDbType.Integer, Page),
                new ("p_order_column", NpgsqlDbType.Varchar, OrderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, OrderDesc),
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
                new ("p_id_seccion", NpgsqlDbType.Array | NpgsqlDbType.Integer, (idSeccion is null || !idSeccion.Any()) ? DBNull.Value :  idSeccion.ToArray()),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, idRenglonSeccion),
                new ("p_activo", NpgsqlDbType.Boolean, activo),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.DocumentosFolio,
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

            List<ResponseDocumentoHistorico> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? item.IsNull(11) ? null! : item.Field<int>(11) : item.Field<int>(1),
                        IdTipoDocumento = item.IsNull(2) ? 0 : item.Field<int>(2),
                        TipoDocumento = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        NombreDocumento = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        TamanoDocumento = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        IdSeccion = item.IsNull(6) ? 0 : item.Field<int>(6),
                        Seccion = item.IsNull(7) ? null! : item.Field<string>(7)!,
                        Folio = item.IsNull(8) ? null! : item.Field<string>(8)!,
                        IdRenglonSeccion = item.IsNull(9) ? 0 : item.Field<int>(9),
                        Permanente = !item.IsNull(10) && item.Field<bool>(10),
                    }
                );
            }

            return resultList;
        }

        public async Task<ResultTransaction> DeleteAsync(int[] ids, string usuarioModificacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_ids", NpgsqlDbType.Array | NpgsqlDbType.Integer, ids!),
                new ("p_usuario_modificacion", NpgsqlDbType.Text, usuarioModificacion!),
            };
            var response = await _database.ExecuteFunctionAsync(EnumFunctions.DocumentosDelete, parameters);
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

        public async Task<List<Documento>> GetDocumentosByRenglonTipoAsync(int? idAutorizacion, int? idCumplimentacion, int idSeccion, int? idRenglonSeccion, int? idTipoDocumento)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
                new ("p_id_seccion", NpgsqlDbType.Integer, idSeccion),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, idRenglonSeccion),
                new ("p_id_tipo_documento", NpgsqlDbType.Integer, idTipoDocumento),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.DocumentosByRenglonTipo,
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

            List<Documento> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_autorizacion = item.IsNull(1) ? null! : item.Field<int>(1),
                        id_tipo_documento = item.IsNull(2) ? 0 : item.Field<int>(2),
                        id_seccion = item.IsNull(3) ? 0 : item.Field<int>(3),
                        id_renglon_seccion = item.IsNull(4) ? 0 : item.Field<int>(4),
                        id_unidad_administrativa = item.IsNull(5) ? 0 : item.Field<int>(5),
                        numero_folio = item.IsNull(6) ? null! : item.Field<string>(6),
                        file_name = item.IsNull(7) ? null! : item.Field<string>(7)!,
                        file_path = item.IsNull(8) ? null! : item.Field<string>(8)!,
                        content_type = item.IsNull(9) ? null! : item.Field<string>(9)!,
                        file_size = item.IsNull(10) ? null! : item.Field<string>(10)!,
                        owner_name = item.IsNull(11) ? null! : item.Field<string>(11)!,
                        activo = !item.IsNull(12) && item.Field<bool>(12)!,
                        permanente = !item.IsNull(13) && item.Field<bool>(13)!,
                        usuario_creacion = item.IsNull(14) ? null! : item.Field<string>(14)!,
                        fecha_creacion = item.IsNull(15) ? new() : item.Field<DateTime>(15)!,
                        usuario_modificacion = item.IsNull(16) ? null! : item.Field<string>(16)!,
                        fecha_modificacion = item.IsNull(17) ? new() : item.Field<DateTime>(17)!,
                        id_cumplimentacion = item.IsNull(18) ? null! : item.Field<int>(18),
                    }
                );
            }

            return resultList;
        }
    }
}