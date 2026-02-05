using AutorizacionesAPI.Model.Entities;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class AbogadoRepository : IAbogadoRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AbogadoRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<Abogado> GetAbogadoAsignadoAsync(int? idAutorizacion, int? idCumplimentacion)
        {
            ParameterPGsql[] parameters = 
                { 
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacion), 
                new ("p_id_cumplimentacion", NpgsqlDbType.Integer, idCumplimentacion),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AdminAbogadoAsignado,
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
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<int>(1),
                id_abogado = response.Data.Tables[0].Rows[0].IsNull(2)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                fecha_asignacion = response.Data.Tables[0].Rows[0].IsNull(3)
                     ? new()
                     : response.Data.Tables[0].Rows[0].Field<DateTime>(3)!,
                remitido = !response.Data.Tables[0].Rows[0].IsNull(4)
                    && response.Data.Tables[0].Rows[0].Field<bool>(4)!,
                reasingado = !response.Data.Tables[0].Rows[0].IsNull(5)
                    && response.Data.Tables[0].Rows[0].Field<bool>(5)!,
                id_cumplimentacion = response.Data.Tables[0].Rows[0].IsNull(6)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<int>(6),
            };
        }
    }
}
