using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Interfaces
{
    public interface ITemporalPurchasesRepository
    {
        Task<ActionResponse<TemporalPurchase>> GetAsync(int id);
        Task<ActionResponse<TemporalPurchase>> PutFullAsync(TemporalPurchaseDTO temporalPurchaseDTO);
        Task<ActionResponse<TemporalPurchaseDTO>> AddFullAsync(string email, TemporalPurchaseDTO temporalPurchaseDTO);
        Task<ActionResponse<IEnumerable<TemporalPurchase>>> GetAsync(string email);
        Task<ActionResponse<int>> GetCountAsync(string email);
        Task<ActionResponse<bool>> DeleteAsync(string email);
    }
}
