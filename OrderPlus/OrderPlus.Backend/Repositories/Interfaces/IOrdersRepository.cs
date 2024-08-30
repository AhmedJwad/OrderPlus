using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Interfaces
{
    public interface IOrdersRepository
    {
        Task<ActionResponse<IEnumerable<Order>>> GetReportAsync(DatesDTO datesDTO);
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination);
        Task<ActionResponse<Order>> GetAsync(int id);
        Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO);
    }
}
