using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Helpers
{
    public interface IPurchaseHelper
    {
        Task<ActionResponse<bool>> ProcessPurchaseAsync(PurchaseDTO purchaseDTO, string email);
    }
}
