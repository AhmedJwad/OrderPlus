using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class NewsRepository :GenericRepository<NewsArticle> ,INewsRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public NewsRepository(DataContext context, IFileStorage fileStorage) : base(context)
        {
           _context = context;
           _fileStorage = fileStorage;
        }

        public override  async Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle newsArticle)
        {
            var photoProduct = Convert.FromBase64String(newsArticle.ImageUrl);
            newsArticle.ImageUrl = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "news");
            return await base.AddAsync(newsArticle);
        }

        public override async Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.NewsArticles.AsQueryable();
            if(pagination.Id!=0)
            {
                queryable = queryable.Where(x => x.Active);
            }
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Title.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<NewsArticle>>
            {
                WasSuccess=true,
                Result = await queryable
                    .OrderBy(x => x.Title)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.NewsArticles.AsQueryable();

            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.Active);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Title.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle newsArticle)
        {
           if(!newsArticle.ImageUrl.StartsWith("images/products"))
            {
                var photoProduct = Convert.FromBase64String(newsArticle.ImageUrl);
                newsArticle.ImageUrl = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "news");
               
            }
            return await base.UpdateAsync(newsArticle);
        }
    }
}
