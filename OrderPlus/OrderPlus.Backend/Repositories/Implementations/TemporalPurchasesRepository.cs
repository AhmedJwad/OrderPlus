using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class TemporalPurchasesRepository :GenericRepository<TemporalPurchase>, ITemporalPurchasesRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public TemporalPurchasesRepository(DataContext context, IUsersRepository usersRepository ) : base(context)
        {
           _context = context;
           _usersRepository = usersRepository;
        }

        public async Task<ActionResponse<TemporalPurchaseDTO>> AddFullAsync(string email, TemporalPurchaseDTO temporalPurchaseDTO)
        {
           var product =await _context.Products.FirstOrDefaultAsync(x=>x.Id== temporalPurchaseDTO.ProductId);
            if(product == null)
            {
                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };                    
            }
            var user = await _usersRepository.GetUserAsync(email);
            if(user == null)
            {
                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = false,
                    Message = "user not exist"
                };
            }
            var temporalPurchase = new TemporalPurchase
            {
                Cost = temporalPurchaseDTO.Cost,
                Product=product,
                Quantity=temporalPurchaseDTO.Quantity,
                Remarks=temporalPurchaseDTO.RemarksDetail,
                User=user,
            };
            try
            {
                _context.Add(temporalPurchase);
                await _context.SaveChangesAsync();
                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = true,
                    Result = temporalPurchaseDTO
                };
            }
            catch (Exception ex)
            {

                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = false,
                    Message= ex.Message,

                };
            }

        }

        public async Task<ActionResponse<bool>> DeleteAsync(string email)
        {
            var temporalPurchases=await _context.TemporalPurchases.Where(x=>x.User!.UserName==email).ToListAsync();
            _context.RemoveRange(temporalPurchases);
            await _context.SaveChangesAsync();
            return new ActionResponse<bool>
            {
                WasSuccess=true,
            };

        }

        public override async Task<ActionResponse<TemporalPurchase>> GetAsync(int id)
        {
            var temporalPurchase = await _context.TemporalPurchases
               .Include(ts => ts.User!)
               .Include(ts => ts.Product!)
               .ThenInclude(p => p.ProductCategories!)
               .ThenInclude(pc => pc.Category)
               .Include(ts => ts.Product!)
               .ThenInclude(p => p.ProductImages)
               .FirstOrDefaultAsync(x => x.Id == id);

            if (temporalPurchase == null)
            {
                return new ActionResponse<TemporalPurchase>
                {
                    WasSuccess = false,
                    Message = "Record not found"
                };
            }

            return new ActionResponse<TemporalPurchase>
            {
                WasSuccess = true,
                Result = temporalPurchase
            };
        }

        public  async Task<ActionResponse<IEnumerable<TemporalPurchase>>> GetAsync(string email)
        {
            var temporalPurchases = await _context.TemporalPurchases
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .Where(x => x.User!.Email == email)
                .ToListAsync();

            return new ActionResponse<IEnumerable<TemporalPurchase>>
            {
                WasSuccess = true,
                Result = temporalPurchases
            };
        }

        public async Task<ActionResponse<int>> GetCountAsync(string email)
        {
            var count = await _context.TemporalPurchases
              .Where(x => x.User!.Email == email)
              .SumAsync(x => x.Quantity);

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public async Task<ActionResponse<TemporalPurchase>> PutFullAsync(TemporalPurchaseDTO temporalPurchaseDTO)
        {
            var currentTemporalOrder=await _context.TemporalPurchases.FirstOrDefaultAsync(x=>x.Id==temporalPurchaseDTO.Id);
            if (currentTemporalOrder!=null)
            {
                return new ActionResponse<TemporalPurchase>
                {
                    WasSuccess=false,
                    Message= "Record not found"
                };
            }
            currentTemporalOrder!.Remarks=temporalPurchaseDTO.RemarksDetail;
            currentTemporalOrder.Quantity=temporalPurchaseDTO.Quantity;
            currentTemporalOrder.Cost=temporalPurchaseDTO.Cost;
            _context.Update(currentTemporalOrder);
            await _context.SaveChangesAsync();
            return new ActionResponse<TemporalPurchase>
            {
                WasSuccess = true,
                Result = currentTemporalOrder,
            };

        }
    }
}
