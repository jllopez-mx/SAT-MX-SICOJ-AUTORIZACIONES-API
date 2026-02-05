using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class ModificacionRepository : IModificacionRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public ModificacionRepository(ISqlTools database)
        {
            _database =
                database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddAsync(Autorizacion entityAutorizacion, Modificacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, entity.id_autorizacion!),
                new ("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion!),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, entity.id_renglon_seccion!),
                new ("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAutorizacion.id_estado_procesal!),
                new ("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAutorizacion.fecha_control_solicitudes!),
                new ("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };
            var response = await _database.ExecuteFunctionAsync(EnumFunctions.GenModificacionCreate, parameters);
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

        public async Task<List<Modificacion>> GetByIdAsuntoAsync(int? idAutorizacion, int? idCumplimentacion)
        {
            ParameterPGsql[] parameters = 
            { 
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion), 
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenModificacionByIdAsunto,
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

            List<Modificacion> resultList = new();
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
                        id_seccion = item.IsNull(2)
                            ? 0
                            : item.Field<int>(2),
                        id_renglon_seccion = item.IsNull(3)
                            ? 0
                            : item.Field<int>(3)!,
                        fecha_creacion = item.IsNull(4)
                            ? new()
                            : item.Field<DateTime>(4),
                        usuario_creacion = item.IsNull(5)
                            ? null!
                            : item.Field<string>(5),
                        fecha_modificacion = item.IsNull(6)
                            ? null!
                            : item.Field<DateTime?>(6)!,
                        usuario_modificacion = item.IsNull(7)
                            ? null!
                            : item.Field<string>(7),
                        activo = !item.IsNull(8)
                            && item.Field<bool>(8),   
                        id_estado_procesal = item.IsNull(9)
                            ? 0
                            : item.Field<int>(9),
                        id_cumplimentacion = item.IsNull(10)
                            ? null!
                            : item.Field<int>(10),
                    }
                );
            }
            return resultList;
        }
    }
}
