using AutorizacionesAPI.Model.DTO;
using AutorizacionesAPI.Model.Entities;
using Sicoj.Utils.Models;


namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IPersonasAutorizadasRepository
    {
        Task<ResultTransaction> AddPersonasAutorizadasAsync(PersonasAutorizadas entity);
        Task<ResponseComercioExteriorPersonasAutorizadasAbogadoById> GetPersonasAutorizadasByIdAsync(int id);
        Task<List<ResponseComercioExteriorPersonasAutorizadasAbogadoById>> GetByIdAutorizacionAsync(int ids);
        Task<PersonasAutorizadas> GetByIdAsync(int id);
        Task<ResultTransaction> UpdatePersonasAutorizadasAsync(PersonasAutorizadas entity);
        Task<ResultTransaction> DeleteAsync(int id);       
    }
}
