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
    public class PurchaseDetailsController : GenericController<PurchaseDetail>
    {
        private readonly IPurchaseDetailUnitOfWork _purchaseDetailUnitOfWork;

        public PurchaseDetailsController(IGenericUnitOfWork<PurchaseDetail> genericUnitOfWork , 
            IPurchaseDetailUnitOfWork purchaseDetailUnitOfWork ) : base(genericUnitOfWork)
        {
            _purchaseDetailUnitOfWork = purchaseDetailUnitOfWork;
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _purchaseDetailUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _purchaseDetailUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}
