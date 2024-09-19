using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InventoryDetailsController : GenericController<InventoryDetail>
    {
        private readonly IInventoryDetailsUnitOfWork _inventoryDetailsUnitOfWork;

        public InventoryDetailsController(IGenericUnitOfWork<InventoryDetail> genericUnitOfWork, 
            IInventoryDetailsUnitOfWork inventoryDetailsUnitOfWork) : base(genericUnitOfWork)
        {
           _inventoryDetailsUnitOfWork = inventoryDetailsUnitOfWork;
        }
        [HttpPut]
        public override async Task<IActionResult> PutAsync(InventoryDetail model)
        {
            var response = await _inventoryDetailsUnitOfWork.UpdateAsync(model);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("recordsNumberCount1")]
        public async Task<IActionResult> GetRecordsNumberCount1Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetRecordsNumberCount1Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("recordsNumberCount2")]
        public async Task<IActionResult> GetRecordsNumberCount2Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetRecordsNumberCount2Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("recordsNumberCount3")]
        public async Task<IActionResult> GetRecordsNumberCount3Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetRecordsNumberCount3Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("Count1")]
        public async Task<IActionResult> GetCount1Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetCount1Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("Count2")]
        public async Task<IActionResult> GetCount2Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetCount2Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("Count3")]
        public async Task<IActionResult> GetCount3Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetCount3Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}
