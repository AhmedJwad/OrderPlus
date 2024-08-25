using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class BanksRepository : GenericRepository<Bank>, IBanksRepository
    {
        private readonly DataContext _context;

        public BanksRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Banks.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Bank>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<IEnumerable<Bank>> GetComboAsync()
        {
            return await _context.Banks
               .OrderBy(c => c.Name)
               .ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Banks.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            var totalRecords = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalRecords,
            };
        }
    }
}
