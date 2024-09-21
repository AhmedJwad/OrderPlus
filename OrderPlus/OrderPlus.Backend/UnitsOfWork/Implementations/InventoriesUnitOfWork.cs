using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class InventoriesUnitOfWork :GenericUnitOfWork<Inventory> ,IInventoriesUnitOfWork
    {
        private readonly IInventoriesRepository _inventoriesRepository;

        public InventoriesUnitOfWork(IGenericRepository<Inventory> repository , IInventoriesRepository inventoriesRepository) : base(repository)
        {
            _inventoriesRepository = inventoriesRepository;
        }

        public override async Task<ActionResponse<Inventory>> AddAsync(Inventory inventory)
        =>await _inventoriesRepository.AddAsync(inventory);

        public async Task<ActionResponse<bool>> FinishCount1Async(int id)
        =>await _inventoriesRepository.FinishCount1Async(id);

        public async Task<ActionResponse<bool>> FinishCount2Async(int id)
        => await _inventoriesRepository.FinishCount2Async(id);

        public async Task<ActionResponse<bool>> FinishCount3Async(int id)
        => await _inventoriesRepository.FinishCount3Async(id);  

        public override async Task<ActionResponse<Inventory>> GetAsync(int id)
        =>await _inventoriesRepository.GetAsync(id);

        public override async Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination)
        => await _inventoriesRepository.GetAsync(pagination);

        public async Task<IEnumerable<Inventory>> GetComboAsync()
        =>await _inventoriesRepository.GetComboAsync();

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _inventoriesRepository.GetRecordsNumberAsync(pagination);
    }
}
