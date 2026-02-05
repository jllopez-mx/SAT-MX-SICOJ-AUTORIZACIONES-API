using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IDescartarRepository
    {
        Task<ResultTransaction> DescartarAsync(Autorizacion autorizacion, Cumplimentacion cumplimentacion, Descartar entity, 
            List<int>? listaSecciones);
        Task<List<Descartar>> GetByIdAsuntoAsync(int? idAutorizacion, int? idCumplimentacion);
        Task<ResultTransaction> DescartarCumplimentacionPorImprocedenciaAsync(Cumplimentacion cumplimentacion, Descartar descartar);
        Task<ResultTransaction> DescartarCumplimentacionAutorizacionAsync(Cumplimentacion cumplimentacion, Descartar descartar);
    }
}
