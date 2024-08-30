using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Helpers
{
    public interface IOrdersHelper
    {
        Task<ActionResponse<bool>> ProcessOrderAsync(string email, OrderDTO orderDTO);
    }
}
