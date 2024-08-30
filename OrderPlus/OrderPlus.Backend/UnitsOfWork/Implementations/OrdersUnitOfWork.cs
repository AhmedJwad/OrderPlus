using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class OrdersUnitOfWork : GenericUnitOfWork<Order>, IOrdersUnitOfWork
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrdersUnitOfWork(IGenericRepository<Order> genericRepository , IOrdersRepository ordersRepository):base(genericRepository) 
        {
           _ordersRepository = ordersRepository;
        }
        public async Task<ActionResponse<IEnumerable<Order>>> GetReportAsync(DatesDTO datesDTO) => await _ordersRepository.GetReportAsync(datesDTO);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _ordersRepository.GetRecordsNumberAsync(pagination);

        public async Task<ActionResponse<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination) => await _ordersRepository.GetAsync(email, pagination);

        public async Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination) => await _ordersRepository.GetTotalPagesAsync(email, pagination);

        public async Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO) => await _ordersRepository.UpdateFullAsync(email, orderDTO);

        public override async Task<ActionResponse<Order>> GetAsync(int id) => await _ordersRepository.GetAsync(id);
    }
}
