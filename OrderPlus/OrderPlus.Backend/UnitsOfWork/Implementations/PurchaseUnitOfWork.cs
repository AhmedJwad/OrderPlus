using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class PurchaseUnitOfWork : GenericUnitOfWork<Purchase>, IPurchaseUnitOfWork
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseUnitOfWork(IGenericRepository<Purchase> genericRepository, IPurchaseRepository purchaseRepository)
            :base(genericRepository)
        {
           _purchaseRepository = purchaseRepository;
        }
       
        public override async Task<ActionResponse<Purchase>> GetAsync(int id)
         => await _purchaseRepository.GetAsync(id);

        public override async Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination)
         => await _purchaseRepository.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _purchaseRepository.GetRecordsNumberAsync(pagination);
    }
}
