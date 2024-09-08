using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class SuppliersUnitOfWork :GenericUnitOfWork<Supplier> ,ISuppliersUnitOfWork
    {
        private readonly ISuppliersRepository _suppliersRepository;

        public SuppliersUnitOfWork(IGenericRepository<Supplier> genericRepository,ISuppliersRepository suppliersRepository ):base(genericRepository)
        {
           _suppliersRepository = suppliersRepository;
        }
        public override async Task<ActionResponse<Supplier>> GetAsync(int id)
        =>await _suppliersRepository.GetAsync(id);

        public override async Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination)
       => await _suppliersRepository.GetAsync(pagination);

        public async Task<IEnumerable<Supplier>> GetComboAsync()
        => await _suppliersRepository.GetComboAsync();

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _suppliersRepository.GetRecordsNumberAsync(pagination);
    }
}
