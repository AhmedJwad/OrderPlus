using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Interfaces
{
    public interface ISuppliersUnitOfWork
    {
        Task<ActionResponse<Supplier>> GetAsync(int id);
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination);
        Task<IEnumerable<Supplier>> GetComboAsync();
    }
}
