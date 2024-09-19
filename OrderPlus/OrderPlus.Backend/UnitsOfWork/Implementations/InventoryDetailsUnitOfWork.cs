using OrderPlus.Backend.Repositories.Implementations;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class InventoryDetailsUnitOfWork :GenericUnitOfWork<InventoryDetail>, IInventoryDetailsUnitOfWork
    {
        private readonly IInventoryDetailsRepository _inventoryDetailsRepository;

        public InventoryDetailsUnitOfWork(IGenericRepository<InventoryDetail> repository , 
            IInventoryDetailsRepository inventoryDetailsRepository) : base(repository)
        {
            _inventoryDetailsRepository = inventoryDetailsRepository;
        }

        public async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount1Async(PaginationDTO pagination)
         => await _inventoryDetailsRepository.GetCount1Async(pagination);

        public async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount2Async(PaginationDTO pagination)
         => await _inventoryDetailsRepository.GetCount2Async(pagination);

        public async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount3Async(PaginationDTO pagination)
        => await _inventoryDetailsRepository.GetCount3Async(pagination);

        public async Task<ActionResponse<int>> GetRecordsNumberCount1Async(PaginationDTO pagination)
        => await _inventoryDetailsRepository.GetRecordsNumberCount2Async(pagination);

        public async Task<ActionResponse<int>> GetRecordsNumberCount2Async(PaginationDTO pagination)
        => await _inventoryDetailsRepository.GetRecordsNumberCount2Async(pagination);

        public async Task<ActionResponse<int>> GetRecordsNumberCount3Async(PaginationDTO pagination)
        => await _inventoryDetailsRepository.GetRecordsNumberCount3Async(@pagination);

        public override async Task<ActionResponse<InventoryDetail>> UpdateAsync(InventoryDetail inventoryDetail)
        => await _inventoryDetailsRepository.UpdateAsync(inventoryDetail);
    }
}
