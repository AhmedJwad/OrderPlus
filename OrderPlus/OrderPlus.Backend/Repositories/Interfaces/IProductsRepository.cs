using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;
using System.Threading.Tasks;

namespace OrderPlus.Backend.Repositories.Interfaces
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetComboAsync();
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<Product>> DeleteAsync(int id);
        Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO);
        Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO);
        Task <ActionResponse<Product>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO);
        Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO);
        Task<IEnumerable<CategoryProductDTO>> GetProductCountByCategoryAsync();
    }
}
