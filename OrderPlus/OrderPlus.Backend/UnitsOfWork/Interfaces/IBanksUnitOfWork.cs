using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Interfaces
{
    public interface IBanksUnitOfWork
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination);
        Task<IEnumerable<Bank>> GetComboAsync();
    }
}
