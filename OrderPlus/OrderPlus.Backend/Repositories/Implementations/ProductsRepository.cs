using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class ProductsRepository : GenericRepository<Product>, IProductsRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public ProductsRepository(DataContext context, IFileStorage fileStorage) : base(context)
        {
            _context = context;
            _fileStorage = fileStorage;
        }
        public async Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO)
        {
            try
            {
                var newproduct = new Product
                {
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    DesiredProfit = productDTO.DesiredProfit / 100,
                    Stock = productDTO.Stock,
                    Cost = productDTO.Cost,
                    Price = productDTO.Price,
                    ProductCategories = new List<ProductCategory>(),
                    ProductImages = new List<ProductImage>(),
                };
                foreach (var item in productDTO.ProductImages!)
                {
                    var photoProduct = Convert.FromBase64String(item);
                    newproduct.ProductImages.Add(new ProductImage
                    {
                        Image = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products")
                    });
                }
                foreach (var item in productDTO.ProductCategoryIds!)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == item);
                    if (category != null)
                    {
                        newproduct.ProductCategories.Add(new ProductCategory { Category = category });
                    }

                }
                _context.Add(newproduct);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
                {
                    WasSuccess = true,
                    Result = newproduct
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "A product with the same name already exists."
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        public async Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products.Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };
            }
            for (int i = 0; i < imageDTO.Images.Count; i++)
            {
                if (!imageDTO.Images[i].StartsWith("images/products"))
                {                   
                    var photoProduct = Convert.FromBase64String(imageDTO.Images[i]);
                    imageDTO.Images[i] = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products");
                    product.ProductImages!.Add(new ProductImage { Image = imageDTO.Images[i] });
                }

            }
            _context.Update(product);
            await _context.SaveChangesAsync();
            return new ActionResponse<ImageDTO>
            {
                WasSuccess = true,
                Result = imageDTO
            };
        }

        public override async Task<ActionResponse<Product>> DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(x => x.ProductCategories)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "Product not found"
                };
            }

            foreach (var productImage in product.ProductImages!)
            {
                await _fileStorage.RemoveFileAsync(productImage.Image, "products");
            }

            try
            {
                _context.ProductCategories.RemoveRange(product.ProductCategories!);
                _context.ProductImages.RemoveRange(product.ProductImages!);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
                {
                    WasSuccess = true,
                };
            }
            catch
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "The product cannot be deleted because it has related records."
                };
            }
        }

        public override async Task<ActionResponse<Product>> GetAsync(int id)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductCategories!)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };
            }

            return new ActionResponse<Product>
            {
                WasSuccess = true,
                Result = product
            };
        }

        public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products
               .Include(x => x.ProductImages)
               .Include(x => x.ProductCategories)
               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(pagination.CategoryFilter))
            {
                queryable = queryable.Where(x => x.ProductCategories!.Any(y => y.Category!.Name == pagination.CategoryFilter));
            }

            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<IEnumerable<Product>> GetComboAsync()
        {
            return await _context.Products
               .OrderBy(x => x.Name)
               .ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(pagination.CategoryFilter))
            {
                queryable = queryable.Where(x => x.ProductCategories!.Any(y => y.Category.Name == pagination.CategoryFilter));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products
                   .Include(x => x.ProductImages)
                   .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };
            }
            if (product.ProductImages is null || product.ProductImages.Count == 0)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = true,
                    Result = imageDTO
                };
            }
            var lastImage = product.ProductImages.LastOrDefault();
            await _fileStorage.RemoveFileAsync(lastImage!.Image, "products");
            _context.ProductImages.Remove(lastImage);
            await _context.SaveChangesAsync();
            imageDTO.Images = product.ProductImages.Select(x => x.Image).ToList();
            return new ActionResponse<ImageDTO>
            {
                WasSuccess = true,
                Result = imageDTO
            };
        }

        public async Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO)
        {
            try
            {
                var product = await _context.Products.Include(x => x.ProductCategories!).
                    ThenInclude(x => x.Category).FirstOrDefaultAsync(x => x.Id == productDTO.Id);
                if (product == null)
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "Product does not exist",
                    };
                }
                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                product.Stock = productDTO.Stock;
                product.Cost = productDTO.Cost;
                product.DesiredProfit = productDTO.DesiredProfit / 100;
                _context.ProductCategories.RemoveRange(product.ProductCategories!);
                product.ProductCategories = new List<ProductCategory>();

                foreach (var item in productDTO.ProductCategoryIds!)
                {
                    var category = await _context.Categories.FindAsync(item);
                    if (category != null)
                    {
                        _context.ProductCategories.Add(new ProductCategory { CategoryId = category.Id, ProductId = product.Id });
                    }
                }
                _context.Update(product);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
                {
                    WasSuccess = true,
                    Result = product
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "A product with the same name already exists."
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }
    }
}