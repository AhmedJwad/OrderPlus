using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PurchasesController : GenericController<Purchase>
    {
        private readonly IPurchaseUnitOfWork _purchaseUnitOfWork;
        private readonly IPurchaseHelper _purchaseHelper;

        public PurchasesController(IGenericUnitOfWork<Purchase> genericUnitOfWork,
            IPurchaseUnitOfWork purchaseUnitOfWork , IPurchaseHelper purchaseHelper) : base(genericUnitOfWork)
        {
           _purchaseUnitOfWork = purchaseUnitOfWork;
           _purchaseHelper = purchaseHelper;
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response= await _purchaseUnitOfWork.GetRecordsNumberAsync(pagination);
            if(response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response=await _purchaseUnitOfWork.GetAsync(pagination);
            if(response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var action=await _purchaseUnitOfWork.GetAsync(id);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }
        [HttpPost("full")]
        public async Task<IActionResult>PosyAsync(PurchaseDTO purchaseDTO)
        {
            var response=await _purchaseHelper.ProcessPurchaseAsync(purchaseDTO, User.Identity!.Name!);
            if(!response.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }
    }
}
