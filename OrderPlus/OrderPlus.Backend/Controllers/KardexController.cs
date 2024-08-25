using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using System;

namespace OrderPlus.Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class KardexController : GenericController<Kardex>
    {
        private readonly IKardexUnitOfWork _kardexUnitOfWork;

        public KardexController(IGenericUnitOfWork<Kardex> genericUnitOfWork , IKardexUnitOfWork kardexUnitOfWork)
            :base(genericUnitOfWork)
        {
            _kardexUnitOfWork = kardexUnitOfWork;
        }

        [HttpPost("Add")]
        public async Task<IActionResult>AddAsync(KardexDTO kardexDTO)
        {
            var response=await _kardexUnitOfWork.AddAsync(kardexDTO);
            if(response.WasSuccess)
            {
                return Ok();
            }
            return NotFound(response.Message);
        }
        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _kardexUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

      
        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _kardexUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        
    }
}
