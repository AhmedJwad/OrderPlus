using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class PurchaseDetailRepository :GenericRepository<PurchaseDetail>, IPurchaseDetailRepository
    {
        private readonly DataContext _context;

        public PurchaseDetailRepository(DataContext context) : base(context)
        {
           _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<PurchaseDetail>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.PurchaseDetails.AsQueryable();
            if(pagination.Id!=0)
            {
                queryable = queryable.Where(x => x.PurchaseId == pagination.Id);
            }
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x=>x.Name.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));

            }           
            return new ActionResponse<IEnumerable<PurchaseDetail>>
            {
                WasSuccess=true,
                Result=await queryable.OrderBy(x=>x.Name).Paginate(pagination).ToListAsync(),
            };
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.PurchaseDetails.AsQueryable();
            if(pagination.Id!=0)
            {
                queryable=queryable.Where(x=>x.PurchaseId== pagination.Id);
            }
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
            }
            var recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess=true,
                Result=recordsNumber,

            };

        }
    }
}
