using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class KardexRepository :GenericRepository<Kardex>, IKardexRepository
    {
        private readonly DataContext _context;

        public KardexRepository(DataContext context ):base(context)
        {
           _context = context;
        }
        public async Task<ActionResponse<bool>> AddAsync( KardexDTO kardexDTO)
        {
            var product=await _context.Products.FindAsync(kardexDTO.ProductId);
            if(product==null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message=$"Kardex - Product with Id: {kardexDTO.ProductId}, not found."
                };
            }
            var kardex = new Kardex
            {
                Date=kardexDTO.Date,
                ProductId=kardexDTO.ProductId,
                KardexType=kardexDTO.KardexType,
                Cost=kardexDTO.Cost,
                Quantity=kardexDTO.Quantity,
            };
            _context.Add(kardex);
            await _context.SaveChangesAsync();
            var kardexForProdcut = await _context.Kardex
                .Where(x => x.ProductId == kardexDTO.ProductId)
                .OrderBy(x => x.Date)
                .ToListAsync();
            await ReKardexAsync(kardexForProdcut);

            return new ActionResponse<bool>
            {
                WasSuccess = true,
            };
        }

        private async Task ReKardexAsync(List<Kardex> kardexForProdcut)
        {
            Kardex? previousKardex = null;
            foreach (var kardex in kardexForProdcut)
            {
                switch (kardex.KardexType)
                {
                    case KardexType.Purchase:
                        if (previousKardex == null)
                        {
                            kardex.Balance = kardex.Quantity;
                            kardex.AverageCost = kardex.Cost;
                        }
                        else
                        {
                            kardex.Balance = kardex.Quantity + previousKardex.Balance;
                            kardex.AverageCost = ((decimal)kardex.Quantity * kardex.Cost + (decimal)previousKardex.Balance * previousKardex.AverageCost) / (decimal)kardex.Balance;
                        }
                        break;

                    case KardexType.Order:
                        if (previousKardex == null)
                        {
                            kardex.Balance -= kardex.Quantity;
                            kardex.AverageCost = 0;
                        }
                        else
                        {
                            kardex.Balance = previousKardex.Balance - kardex.Quantity;
                            kardex.AverageCost = previousKardex.AverageCost;
                        }
                        break;

                    case KardexType.CancelOrder:
                        if (previousKardex == null)
                        {
                            kardex.Balance += kardex.Quantity;
                            kardex.AverageCost = 0;
                        }
                        else
                        {
                            kardex.Balance = previousKardex.Balance + kardex.Quantity;
                            kardex.AverageCost = previousKardex.AverageCost;
                        }
                        break;

                    case KardexType.Inventory:
                        if (previousKardex == null)
                        {
                            kardex.Balance += kardex.Quantity;
                            kardex.AverageCost = kardex.Cost;
                        }
                        else
                        {
                            kardex.Balance = previousKardex.Balance + kardex.Quantity;
                            kardex.AverageCost = kardex.Cost;
                        }
                        break;
                }
                previousKardex = kardex;
            }

            var product = await _context.Products.FindAsync(previousKardex!.ProductId);
            if (product != null)
            {
                product.Cost = previousKardex.AverageCost;
                product.Stock = previousKardex.Balance;
            }
            await _context.SaveChangesAsync();
        }

        public override async Task<ActionResponse<IEnumerable<Kardex>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Kardex.AsQueryable();

            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.ProductId == pagination.Id);
            }

            return new ActionResponse<IEnumerable<Kardex>>
            {
                WasSuccess = true,
                Result = await queryable
                    .Include(x => x.Product)
                    .OrderByDescending(x => x.Date)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Kardex.AsQueryable();

            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.ProductId == pagination.Id);
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
