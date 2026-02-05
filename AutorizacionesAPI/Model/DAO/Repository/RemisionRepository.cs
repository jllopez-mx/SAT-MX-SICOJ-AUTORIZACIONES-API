using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.IDAO.IRepository;
using AutorizacionesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;
using System.Data;

namespace AutorizacionesAPI.Model.DAO.Repository
{
    public class RemisionRepository : IRemisionRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public RemisionRepository(ISqlTools database)
        {
            _database =
                database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<int?> GetRemisionesDisconnectedCount(int idAutorizacionesComercioExterior)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacionesComercioExterior),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRemisionDisconnectedCount,
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

        public async Task<List<ResponseRemision>> GetRemisionesDisconnected(
           int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int idAutorizacionesComercioExterior)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_page_size", NpgsqlDbType.Integer, Fetch),
                new ("p_page", NpgsqlDbType.Integer, Page),
                new ("p_order_column", NpgsqlDbType.Varchar, OrderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, OrderDesc),
                new ("p_id_autorizacion", NpgsqlDbType.Integer, idAutorizacionesComercioExterior),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenRemisionDisconnected,
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

            List<ResponseRemision> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsunto = item.IsNull(1) ? 0 : item.Field<int>(1),
                        idTipoAutoridadRemision = item.IsNull(2) ? 0 : item.Field<int>(2),
                        TipoAutoridadRemision = item.IsNull(3) ? null! : item.Field<string>(3),
                        idUnidadAdministrativaRemite = item.IsNull(4) ? 0 : item.Field<int>(4),
                        UnidadAdministrativaRemite = item.IsNull(5) ? null! : item.Field<string>(5),
                        idUnidadAdministrativaRecibe = item.IsNull(6) ? 0 : item.Field<int>(6),
                        UnidadAdministrativaRecibe = item.IsNull(7) ? null! : item.Field<string>(7),
                        idUnidadAdministrativaExterna = item.IsNull(8) ? 0 : item.Field<int>(8),
                        UnidadAdministrativaExterna = item.IsNull(9) ? null! : item.Field<string>(9),
                        FechaOficio = item.IsNull(10) ? null! : item.Field<DateTime>(10)!.ToString("dd/MM/yyyy"),
                        NumeroOficio = item.IsNull(11) ? null! : item.Field<string>(11)!,
                        FechaRemision = item.IsNull(12) ? null! : item.Field<DateTime>(12)!.ToString("dd/MM/yyyy"),
                    }
                );
            }

            return resultList;
        }
    }
}
