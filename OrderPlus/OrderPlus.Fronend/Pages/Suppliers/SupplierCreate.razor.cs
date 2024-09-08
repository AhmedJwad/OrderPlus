using Microsoft.AspNetCore.Authorization;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Suppliers
{
    [Authorize(Roles = "Admin")]
    public partial class SupplierCreate
    {
        private Supplier supplier = new();

    }
}
