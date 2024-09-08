using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class SuppliersRepository : GenericRepository<Supplier>, ISuppliersRepository
    {
        private readonly DataContext _context;

        public SuppliersRepository(DataContext context):base(context)
        {
            _context = context;
        }
        public override async Task<ActionResponse<Supplier>> GetAsync(int id)
        {
            var supplier = await _context.Suppliers.Include(x => x.City)
                .ThenInclude(x => x.State).ThenInclude(x => x.Country).FirstOrDefaultAsync(x=>x.Id== id);   
            if(supplier == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "supplier not found"
                };
            }
            return new ActionResponse<Supplier>
            {
                WasSuccess = true,
                Result = supplier
            };
        }

        public override async Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Suppliers.AsQueryable();

            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.SupplierName.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Supplier>>
            {
                WasSuccess = true,
                Result = await queryable
                 .Include(s => s.City!)
                 .ThenInclude(c => c.State!)
                 .ThenInclude(s => s.Country)
                 .OrderBy(x => x.SupplierName)
                 .Paginate(pagination)
                 .ToListAsync()
            };
        }

        public async Task<IEnumerable<Supplier>> GetComboAsync()
        {
            return await _context.Suppliers.OrderBy(x=>x.SupplierName).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.SupplierName.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }
    }
}
