using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class TemporalOrdersUnitOfWork :GenericUnitOfWork<TemporalOrder>, ITemporalOrdersUnitOfWork
    {
        private readonly ITemporalOrdersRepository _temporalOrdersRepository;

        public TemporalOrdersUnitOfWork(IGenericRepository<TemporalOrder> genericRepository,
            ITemporalOrdersRepository temporalOrdersRepository):base(genericRepository)
        {
           _temporalOrdersRepository = temporalOrdersRepository;
        }
        public async Task<ActionResponse<TemporalOrderDTO>> AddFullAsync(string email, TemporalOrderDTO temporalOrderDTO)
        => await _temporalOrdersRepository.AddFullAsync(email, temporalOrderDTO);
               
        public override async Task<ActionResponse<TemporalOrder>> GetAsync(int id)
        =>await _temporalOrdersRepository.GetAsync(id);

        public async Task<ActionResponse<IEnumerable<TemporalOrder>>> GetAsync(string email)
        => await _temporalOrdersRepository.GetAsync(email);

        public async Task<ActionResponse<int>> GetCountAsync(string email)
        =>await _temporalOrdersRepository.GetCountAsync(email);

        public async Task<ActionResponse<TemporalOrder>> PutFullAsync(TemporalOrderDTO temporalOrderDTO)
        =>await _temporalOrdersRepository.PutFullAsync(temporalOrderDTO);
    }
}
