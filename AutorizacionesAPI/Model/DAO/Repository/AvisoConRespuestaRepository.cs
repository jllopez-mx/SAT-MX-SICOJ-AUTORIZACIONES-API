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
    public class AvisoConRespuestaRepository : IAvisoConRespuestaRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AvisoConRespuestaRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion


        public async Task<AvisoConRespuesta> GetByIdAutorizacion(int idAutorizacion)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAvisoConRespuestaByIdAutorizacion,
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
                id_autorizacion_relacionado = response.Data.Tables[0].Rows[0].IsNull(2) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(2),
                activo = response.Data.Tables[0].Rows[0].IsNull(3) ? false : response.Data.Tables[0].Rows[0].Field<bool>(3),
                no_asunto_externo = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<string>(4),
            };
        }
    }
}
