using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Interfaces
{
    public interface IInventoriesRepository
    {
        Task<IEnumerable<Inventory>> GetComboAsync();
        Task<ActionResponse<bool>> FinishCount1Async(int id);
        Task<ActionResponse<bool>> FinishCount2Async(int id);
        Task<ActionResponse<bool>> FinishCount3Async(int id);
        Task<ActionResponse<Inventory>> GetAsync(int id);
        Task<ActionResponse<Inventory>> AddAsync(Inventory inventory);
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination);
    }
}
