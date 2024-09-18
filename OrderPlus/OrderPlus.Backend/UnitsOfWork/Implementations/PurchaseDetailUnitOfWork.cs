using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class PurchaseDetailUnitOfWork :GenericUnitOfWork<PurchaseDetail>, IPurchaseDetailUnitOfWork
    {
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public PurchaseDetailUnitOfWork(IGenericRepository<PurchaseDetail> repository,
            IPurchaseDetailRepository purchaseDetailRepository) : base(repository)
        {
           _purchaseDetailRepository = purchaseDetailRepository;
        }
        public override async Task<ActionResponse<IEnumerable<PurchaseDetail>>> GetAsync(PaginationDTO pagination)
        =>await _purchaseDetailRepository.GetAsync(pagination);
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
       => await _purchaseDetailRepository.GetRecordsNumberAsync(pagination);
    }
}
