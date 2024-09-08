using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Interfaces
{
    public interface ISuppliersRepository
    {
        Task<ActionResponse<Supplier>> GetAsync(int id);
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination);
        Task<IEnumerable<Supplier>> GetComboAsync();
    }
}
