using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Interfaces
{
    public interface ITemporalOrdersUnitOfWork
    {
        
        Task<ActionResponse<TemporalOrder>> GetAsync(int id);
        Task<ActionResponse<TemporalOrder>> PutFullAsync(TemporalOrderDTO temporalOrderDTO);
        Task<ActionResponse<TemporalOrderDTO>> AddFullAsync(string email, TemporalOrderDTO temporalOrderDTO);
        Task<ActionResponse<IEnumerable<TemporalOrder>>> GetAsync(string email);
        Task<ActionResponse<int>> GetCountAsync(string email);
    }
}
