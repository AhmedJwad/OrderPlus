using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class PurchaseRepository :GenericRepository<Purchase> ,IPurchaseRepository
    {
        private readonly DataContext _context;

        public PurchaseRepository( DataContext context):base(context)
        {
          _context = context;
        }
       

        public override async Task<ActionResponse<Purchase>> GetAsync(int id)
        {
           var purchase = await _context.Purchases.Include(x=>x.Supplier!)
                .Include(x=>x.PurchaseDetails!).FirstOrDefaultAsync(x=>x.Id == id);
            if(purchase == null)
            {
                return new ActionResponse<Purchase>
                {
                    Message = "Purchase does not exist."
                };
            }
            return new ActionResponse<Purchase>
            {
                WasSuccess = true,
                Result=purchase,
            };
        }

        public override async Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination)
        { 
           var queryable=_context.Purchases.Include(x=>x.Supplier).Include(x=>x.PurchaseDetails).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Supplier!.SupplierName.Contains(pagination.Filter!, StringComparison.CurrentCultureIgnoreCase));    
            }
            return new ActionResponse<IEnumerable<Purchase>>
            {
                WasSuccess = true,
                Result =await queryable.OrderByDescending(x => x.Date).Paginate(pagination).ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Purchases
                 .Include(x => x.Supplier)
                 .AsQueryable();
            int recordsNumber = await queryable.CountAsync();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Supplier!.SupplierName.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
            }

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }
    }
}
