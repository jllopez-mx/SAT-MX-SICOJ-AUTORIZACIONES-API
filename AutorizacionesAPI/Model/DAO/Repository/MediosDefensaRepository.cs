using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class MediosDefensaRepository : IMediosDefensaRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public MediosDefensaRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<List<ResponseMediosDefensa>> GetMediosDefensaDisconnectedAsync(List<string> noAsunto)
        {

            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_no_asunto", NpgsqlDbType.Array | NpgsqlDbType.Text, (noAsunto is null || !noAsunto.Any()) ? DBNull.Value :  noAsunto.ToArray()),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenMediosDefensaDisconnected, parameters!);

            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseMediosDefensa> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        noAsunto = response.Data.Tables[0].Rows[0].IsNull(0) ? null! : response.Data.Tables[0].Rows[0].Field<string>(0),
                        idUnidadAdministrativa = response.Data.Tables[0].Rows[0].IsNull(1) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(1),
                        idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(2) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(2),
                    }
                );
            }

            return resultList;

        }
    }
}
