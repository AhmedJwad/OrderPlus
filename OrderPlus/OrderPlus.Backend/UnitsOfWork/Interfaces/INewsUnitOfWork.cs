using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Interfaces
{
    public interface INewsUnitOfWork
    {
        Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle newsArticle);
        Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle newsArticle);
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination);
    }
}
