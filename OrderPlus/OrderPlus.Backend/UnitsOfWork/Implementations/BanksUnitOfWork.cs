using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class BanksUnitOfWork :GenericUnitOfWork<Bank>, IBanksUnitOfWork
    {
        private readonly IBanksRepository _repository;

        public BanksUnitOfWork(IGenericRepository<Bank> genericRepository, 
            IBanksRepository repository):base(genericRepository)
        {
           _repository = repository;
        }
        public override async Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination)
       => await _repository.GetAsync(pagination);

        public  async Task<IEnumerable<Bank>> GetComboAsync()
        =>await _repository.GetComboAsync();

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _repository.GetRecordsNumberAsync(pagination);
    }
}
