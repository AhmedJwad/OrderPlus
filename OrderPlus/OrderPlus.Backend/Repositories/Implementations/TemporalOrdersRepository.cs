using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class TemporalOrdersRepository :GenericRepository<TemporalOrder>, ITemporalOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public TemporalOrdersRepository(DataContext context , IUsersRepository usersRepository):base(context)
        {
           _context = context;
           _usersRepository = usersRepository;
        }
        public async Task<ActionResponse<TemporalOrderDTO>> AddFullAsync(string email, TemporalOrderDTO temporalOrderDTO)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalOrderDTO.ProductId);
            if(product ==null)
            {
                return new ActionResponse<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message= "Product does not exist",
                };
            }
            var user=await _usersRepository.GetUserAsync(email);
            if(user == null)
            {
                return new ActionResponse<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message= "User does not exist",
                };
            }
            var temporalorder = new TemporalOrder
            {
                Product=product,
                User=user,
                Quantity=temporalOrderDTO.Quantity,
                Remarks=temporalOrderDTO.Remarks,
            };
            try
            {
                _context.Add(temporalorder);
                await _context.SaveChangesAsync();
                return new ActionResponse<TemporalOrderDTO>
                {
                    WasSuccess = true,
                    Result = temporalOrderDTO
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
                
            }
        }
        
        public override async Task<ActionResponse<TemporalOrder>> GetAsync(int id)
        {
            var temporalOrder = await _context.TemporalOrders.Include(x => x.User)
                .Include(x => x.Product).ThenInclude(x => x.ProductCategories!).ThenInclude(x => x.Category)
                .Include(x => x.Product).ThenInclude(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if(temporalOrder==null)
            {
                return new ActionResponse<TemporalOrder>
                {
                    WasSuccess = false,
                    Message= "Order not found"
                };
            }
            return new ActionResponse<TemporalOrder>
            {
                WasSuccess = true,
                Result = temporalOrder
            };
        }

        public async Task<ActionResponse<IEnumerable<TemporalOrder>>> GetAsync(string email)
        {
            var temporalOrders = await _context.TemporalOrders.Include(X => X.User)
                 .Include(X => X.Product).ThenInclude(X => X.ProductCategories!).ThenInclude(X => X.Category)
                 .Include(X => X.Product).ThenInclude(X => X.ProductImages).Where(X => X.User!.Email == email)
                 .ToListAsync();
            return new ActionResponse<IEnumerable<TemporalOrder>>
            {
                WasSuccess = true,
                Result = temporalOrders,
            };
        }

        public async Task<ActionResponse<int>> GetCountAsync(string email)
        {
            var count = await _context.TemporalOrders.Where(x => x.User!.Email == email)
                 .SumAsync(x => x.Quantity);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result =(int)count,
            };
        }

        public async Task<ActionResponse<TemporalOrder>> PutFullAsync(TemporalOrderDTO temporalOrderDTO)
        {
            var currentTemporalOrder=await _context.TemporalOrders.FirstOrDefaultAsync(x=>x.Id==temporalOrderDTO.Id);
            if(currentTemporalOrder==null)
            {
                return new ActionResponse<TemporalOrder>
                {
                    WasSuccess = false,
                    Message = "Order not found"
                };
            }
            currentTemporalOrder.Quantity= temporalOrderDTO.Quantity;
            currentTemporalOrder.Remarks=temporalOrderDTO.Remarks;
            _context.Update(currentTemporalOrder);
            await _context.SaveChangesAsync();
            return new ActionResponse<TemporalOrder>
            {
                WasSuccess = true,
                Result = currentTemporalOrder,
            };
        }
    }
}
