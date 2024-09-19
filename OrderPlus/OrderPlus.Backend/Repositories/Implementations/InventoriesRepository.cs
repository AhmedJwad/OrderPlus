using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class InventoriesRepository : GenericRepository<Inventory>, IInventoriesRepository
    {
        private readonly DataContext _context;
        private readonly IKardexUnitOfWork _kardexUnitOfWork;

        public InventoriesRepository(DataContext context, IKardexUnitOfWork kardexUnitOfWork ) : base(context)
        {
            _context = context;
           _kardexUnitOfWork = kardexUnitOfWork;
        }

        public override async Task<ActionResponse<Inventory>> AddAsync(Inventory inventory)
        {
            inventory.InventoryDetails=new List<InventoryDetail>();
            inventory.Date = inventory.Date.ToUniversalTime();
            var products = await _context.Products.ToListAsync();
            foreach (var item in products)
            {
                inventory.InventoryDetails.Add(new InventoryDetail
                {
                    Cost = item.Cost,
                    ProductId=item.Id,
                    Stock=item.Stock,
                });
            }
            await base.AddAsync(inventory);
            return new ActionResponse<Inventory>
            {
                WasSuccess = true,
                Result = inventory,
            };
        }

        public async Task<ActionResponse<bool>> FinishCount1Async(int id)
        {
            var inventory=await _context.Inventories.FindAsync(id);
            if(inventory==null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = true,
                    Message = "Inventory does not exist"
                };
            }
            inventory.Count1Finish = true;
            _context.Update(inventory);
            await _context.SaveChangesAsync();
            return new ActionResponse<bool>
            {
                WasSuccess = true,
            };
        }

        public async Task<ActionResponse<bool>> FinishCount2Async(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = true,
                    Message = "Inventory does not exist"
                };
            }
            inventory.Count2Finish = true;
            _context.Update(inventory);
            await _context.SaveChangesAsync();
            return new ActionResponse<bool>
            {
                WasSuccess = true,
            };
        }

        public async Task<ActionResponse<bool>> FinishCount3Async(int id)
        {
            var inventory = await _context.Inventories.Include(x => x.InventoryDetails)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (inventory == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Inventory does not exist"
                };
            }
            foreach (var item in inventory.InventoryDetails!)
            {
                if (!(item.Stock == item.Count1 || item.Stock==item.Count2))
                {
                    if(item.Count1==item.Count2)
                    {
                        if(item.Stock > item.Count1)
                        {
                            item.Adjustment = (item.Stock - item.Count1) * -1;
                        }
                        else
                        {
                            item.Adjustment=item.Count1 - item.Stock;
                        }
                    }
                    else
                    {
                        if(item.Stock > item.Count3)
                        {
                            item.Adjustment = (item.Stock - item.Count3) * -1;
                        }
                        else
                        {
                            item.Adjustment=item.Count3 - item.Stock;
                        }
                    }
                    var kardexDTO = new KardexDTO
                    {
                        Date = inventory.Date,
                        ProductId = item.ProductId,
                        KardexType = KardexType.Inventory,
                        Cost = item.Cost,
                        Quantity = item.Adjustment
                    };

                    await _kardexUnitOfWork.AddAsync(kardexDTO);
                }
            }
            inventory.Count3Finish = true;
            await _context.SaveChangesAsync();
            return new ActionResponse<bool> { WasSuccess = true };
        }

        public override async Task<ActionResponse<Inventory>> GetAsync(int id)
        {
            var inventory = await _context.Inventories.Include(x => x.InventoryDetails!)
                 .ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
            if(inventory == null)
            {
                return new ActionResponse<Inventory>
                {
                    WasSuccess=false,
                    Message = "Inventory does not exist"
                };
            }
            return new ActionResponse<Inventory>
            {
                WasSuccess=true,
                Result= inventory
            };
        }

        public override async Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination)
        {
           var queryable=_context.Inventories.AsQueryable();
            return new ActionResponse<IEnumerable<Inventory>>
            {
                WasSuccess = true,
                Result = await queryable
                   .OrderByDescending(x => x.Date)
                   .Paginate(pagination)
                   .ToListAsync()
            };
        }

        public async Task<IEnumerable<Inventory>> GetComboAsync()
        {
            return await _context.Inventories
               .OrderBy(c => c.Name)
               .ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Inventories.AsQueryable();
            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }
    }
}
