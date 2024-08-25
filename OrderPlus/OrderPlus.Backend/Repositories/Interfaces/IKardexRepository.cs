using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Interfaces
{
    public interface IKardexRepository
    {
        Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO);
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Kardex>>> GetAsync(PaginationDTO pagination);
    }
}
