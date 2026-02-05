using AutorizacionesAPI.Model.Entities;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IAbogadoRepository
    {
        Task<Abogado> GetAbogadoAsignadoAsync(int? idAutorizacion, int? idCumplimentacion);
    }
}
