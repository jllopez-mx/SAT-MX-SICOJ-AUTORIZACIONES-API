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
    public class PersonasAutorizadasRepository : IPersonasAutorizadasRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion  

        #region Constructor
        public PersonasAutorizadasRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        #region Personas Autorizadas
        public async Task<ResultTransaction> AddPersonasAutorizadasAsync(PersonasAutorizadas entity)
        {
            ParameterPGsql[] parameters =
            {
                new (
                "p_nombre",
                NpgsqlDbType.Text,
                entity.Nombre!
                ),

                new (
                "p_rfc",
                NpgsqlDbType.Varchar,
                entity.Rfc
                ),

                new (
                "p_telefono",
                NpgsqlDbType.Varchar,
                entity.Telefono
                ),

                new (
                "p_email",
                NpgsqlDbType.Text,
                entity.Email
                ),

                new (
                "p_id_autorizacion",
                NpgsqlDbType.Integer,
                entity.id_autorizacion
                )
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenPersonasAutorizadasCreate, parameters);

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

        public async Task<ResponseComercioExteriorPersonasAutorizadasAbogadoById> GetPersonasAutorizadasByIdAsync(int id)
        {
            ParameterPGsql[] parameters = { new ("p_id", NpgsqlDbType.Integer, id), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenPersonasAutorizadasByIdDisconnected,
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
                nombre = response.Data.Tables[0].Rows[0].IsNull(1) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(1),
                rfc = response.Data.Tables[0].Rows[0].IsNull(2) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(2),
                telefono = response.Data.Tables[0].Rows[0].IsNull(3) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(3),
                email = response.Data.Tables[0].Rows[0].IsNull(4) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(4),
                idAsunto = response.Data.Tables[0].Rows[0].IsNull(5) ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(5),
            };
        }

        public async Task<List<ResponseComercioExteriorPersonasAutorizadasAbogadoById>> GetByIdAutorizacionAsync(int ids)
        {
            ParameterPGsql[] parameters = { new ("p_id_autorizacion", NpgsqlDbType.Integer, ids) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenPersonasAutorizadasByIds,
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

            List<ResponseComercioExteriorPersonasAutorizadasAbogadoById> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new ResponseComercioExteriorPersonasAutorizadasAbogadoById
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        nombre = item.IsNull(1) ? null! : item.Field<string>(1),
                        rfc = item.IsNull(2) ? null! : item.Field<string>(2),
                        telefono = item.IsNull(3) ? null! : item.Field<string>(3),
                        email = item.IsNull(4) ? null! : item.Field<string>(4),
                        idAsunto = response.Data.Tables[0].Rows[0].IsNull(5) ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(5),
                    }
                );
            }
            return resultList;
        }

        public async Task<PersonasAutorizadas> GetByIdAsync(int id)
        {
            ParameterPGsql[] parameters = { new ("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenPersonasAutorizadasByIdDisconnected,
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
                Id = response.Data.Tables[0].Rows[0].IsNull(0) ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(0),
                Nombre = response.Data.Tables[0].Rows[0].IsNull(1) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(1),
                Rfc = response.Data.Tables[0].Rows[0].IsNull(2) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(2),
                Telefono = response.Data.Tables[0].Rows[0].IsNull(3) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(3),
                Email = response.Data.Tables[0].Rows[0].IsNull(4) ? string.Empty
                     : response.Data.Tables[0].Rows[0].Field<string>(4),
                id_autorizacion = response.Data.Tables[0].Rows[0].IsNull(5) ? 0
                     : response.Data.Tables[0].Rows[0].Field<int>(5)
            };
        }

        public async Task<ResultTransaction> UpdatePersonasAutorizadasAsync(PersonasAutorizadas entity)
        {
            ParameterPGsql[] parameters =
            {
                new (
                    "p_id",
                    NpgsqlDbType.Integer,
                    entity.Id
                ),
                new (
                    "p_nombre",
                    NpgsqlDbType.Text,
                    entity.Nombre!
                ),
                new (
                    "p_rfc",
                    NpgsqlDbType.Varchar,
                    entity.Rfc!
                ),
                new (
                    "p_telefono",
                    NpgsqlDbType.Varchar,
                    entity.Telefono
                ),
                new (
                    "p_email",
                    NpgsqlDbType.Text,
                    entity.Email
                ),
                new (
                    "p_id_autorizacion",
                    NpgsqlDbType.Integer,
                    entity.id_autorizacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenPersonasAutorizadasUpdate,
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

        public async Task<ResultTransaction> DeleteAsync(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id", NpgsqlDbType.Integer, id)
            };
            var response = await _database.ExecuteFunctionAsync(EnumFunctions.GenPersonasAutorizadsDelete, parameters);
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

