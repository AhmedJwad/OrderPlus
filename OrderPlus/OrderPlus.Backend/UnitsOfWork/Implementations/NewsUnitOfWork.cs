using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.UnitsOfWork.Implementations
{
    public class NewsUnitOfWork :GenericUnitOfWork<NewsArticle> ,INewsUnitOfWork
    {
        private readonly INewsRepository _newsRepository;

        public NewsUnitOfWork(IGenericRepository<NewsArticle> repository, INewsRepository newsRepository) : base(repository)
        {
           _newsRepository = newsRepository;
        }

        public override async Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle newsArticle)
        =>await _newsRepository.AddAsync(newsArticle);

        public override async Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination)
        => await _newsRepository.GetAsync(pagination);  

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _newsRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle newsArticle)
        => await _newsRepository.UpdateAsync(newsArticle);
    }
}
