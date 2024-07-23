using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;

namespace OrderPlus.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> :Controller where T :class
    {
        private readonly IGenericUnitOfWork<T> _genericUnitOfWork;

        public GenericController(IGenericUnitOfWork<T> genericUnitOfWork)
        {
            _genericUnitOfWork = genericUnitOfWork;
        }
         [HttpGet("recordsNumber")]  
        public virtual async Task<IActionResult> GetRecordsNumberAsync([FromQuery]PaginationDTO pagination)
        {
            var response=await _genericUnitOfWork.GetRecordsNumberAsync(pagination);
            if(response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("full")]
        public virtual async Task<IActionResult> GetAsync()
        {
            var response=await _genericUnitOfWork.GetAsync();
            if(response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [HttpGet]
        public virtual async Task<IActionResult> GetAsync([FromQuery] PaginationDTO paginationDTO)
        {
            var action=await _genericUnitOfWork.GetAsync(paginationDTO);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }
        [HttpGet("totalPages")]
        public virtual async Task<IActionResult> GetPagesAsync([FromQuery]PaginationDTO pagination)
        {
            var response=await _genericUnitOfWork.GetTotalPagesAsync(pagination);
            if(response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetAsync(int id)
        {
            var action=await _genericUnitOfWork.GetAsync(id);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostAsync(T model)
        {
            var action=await _genericUnitOfWork.AddAsync(model);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }
        [HttpPut]
        public virtual async Task<IActionResult> PutAsync(T model)
        {
            var action=await _genericUnitOfWork.UpdateAsync(model);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(int id)
        {
            var action =await _genericUnitOfWork.DeleteAsync(id);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }
    }
}
