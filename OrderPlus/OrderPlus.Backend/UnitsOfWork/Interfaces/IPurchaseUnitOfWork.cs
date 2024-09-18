using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Interfaces
{
    public interface IPurchaseUnitOfWork
    {
        Task<ActionResponse<Purchase>> AddAsync(Purchase entity);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<Purchase>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination);
    }
}
