using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class CountriesUnitOfWork : GenericUnitOfWork<Country>, ICountriesUnitOfWork
    {
        private readonly ICountriesRepository _countriesUnitOfWork;

        public CountriesUnitOfWork(IGenericRepository<Country> repository, 
           ICountriesRepository countriesUnitOfWork ) : base(repository)
        {
            _countriesUnitOfWork = countriesUnitOfWork;
        }

        public override async Task<ActionResponse<Country>> GetAsync(int id)
        => await _countriesUnitOfWork.GetAsync(id);
         
        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync()
        => await _countriesUnitOfWork.GetAsync();

        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination)
        => await _countriesUnitOfWork.GetAsync(pagination);

        public async Task<IEnumerable<Country>> GetComboAsync()
        =>await _countriesUnitOfWork.GetComboAsync();

        public override async  Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _countriesUnitOfWork.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _countriesUnitOfWork.GetTotalPagesAsync(pagination);
    }
}
