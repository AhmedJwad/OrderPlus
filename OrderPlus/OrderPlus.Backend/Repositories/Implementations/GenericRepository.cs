using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Data;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.Repositories.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _entity;
        public GenericRepository(DataContext context )
        {
           _context = context;
           _entity = _context.Set<T>();
        }
        public virtual async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO)
        {
            var queryable = _entity.AsQueryable();
            int recordsNumber=await queryable.CountAsync();

            return new ActionResponse<int>()
            {
                WasSuccess=true,
                Result=recordsNumber,
            };
        }

        public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO paginationDTO)
        {
           var queryable= _entity.AsQueryable();
            return new ActionResponse<IEnumerable<T>>
            {
                WasSuccess = true,
                Result = await queryable
                    .Paginate(paginationDTO)
                    .ToListAsync()
            };
        }
        public virtual async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO)
        {
            var queryable = _entity.AsQueryable();
            var count=await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / paginationDTO.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess=true,
                Result=totalPages,
            };

        }
        public virtual async Task<ActionResponse<T>> AddAsync(T entity)
        {
            _context.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }
        }

        

        public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "Record not found"
                };
            }

            try
            {
                _entity.Remove(row);
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true
                };
            }
            catch
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "It cannot be deleted, because it has related records."
                };
            }
        }

        public virtual async Task<ActionResponse<T>> GetAsync(int Id)
        {
            var row = await _entity.FindAsync(Id);
            if (row == null)
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "Record not found"
                };
            }

            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = row
            };
        }

         

        

        public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
        {
            _context.Update(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }
        }

        private ActionResponse<T> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
        private ActionResponse<T> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "The record you are trying to create already exists."
            };
        }

        public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<T>>
            {
                WasSuccess = true,
                Result = await _entity.ToListAsync()
            };
        }
    }
}
