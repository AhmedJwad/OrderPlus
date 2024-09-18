using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<ActionResponse<Purchase>> AddAsync(Purchase purchase);
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<Purchase>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination);
    }
}
