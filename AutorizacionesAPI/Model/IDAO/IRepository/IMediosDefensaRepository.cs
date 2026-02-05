using AutorizacionesAPI.Model.DTO;

namespace AutorizacionesAPI.Model.IDAO.IRepository
{
    public interface IMediosDefensaRepository
    {
        Task<List<ResponseMediosDefensa>> GetMediosDefensaDisconnectedAsync(List<string> dato);
    }
}
