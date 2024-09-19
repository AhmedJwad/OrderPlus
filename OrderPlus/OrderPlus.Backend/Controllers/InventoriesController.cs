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
    public class InventoriesController : GenericController<Inventory>
    {
        private readonly IInventoriesUnitOfWork _inventoriesUnitOfWork;

        public InventoriesController(IGenericUnitOfWork<Inventory> genericUnitOfWork,
            IInventoriesUnitOfWork inventoriesUnitOfWork) : base(genericUnitOfWork)
        {
            _inventoriesUnitOfWork = inventoriesUnitOfWork;
        }
        [HttpGet("combo")]
        public async Task<IActionResult> GetComboAsync()
        {
            return Ok(await _inventoriesUnitOfWork.GetComboAsync());
        }
        [HttpGet("finishCount1/{id}")]
        public async Task<IActionResult> FinishCount1Async(int id)
        {
            var action=await _inventoriesUnitOfWork.FinishCount1Async(id);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound();            
        }
        [HttpGet("finishCount2/{id}")]
        public async Task<IActionResult> FinishCount2Async(int id)
        {
            var action=await _inventoriesUnitOfWork.FinishCount2Async(id);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound();
        }
        [HttpGet("finishCount3/{id}")]
        public async Task<IActionResult> FinishCount3Async(int id)
        {
            var action=await _inventoriesUnitOfWork.FinishCount3Async(id);
            if(action.WasSuccess)
            {
                return Ok(action);
            }
            return NotFound();
        }
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var response=await _inventoriesUnitOfWork.GetAsync(id);
            if(response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }
        [HttpPost]
        public override async Task<IActionResult> PostAsync(Inventory inventory)
        {
            var action = await _inventoriesUnitOfWork.AddAsync(inventory);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound();
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoriesUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoriesUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}
