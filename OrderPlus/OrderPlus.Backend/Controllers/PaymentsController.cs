using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly Random random = new();
        [HttpPost]
        public async Task <IActionResult> PostAsync(PaymentDTO paymentDTO)
        {
            await Task.Delay(3000);
            var number = random.Next(100);
            if(number <70)
            {
                return Ok(new ActionResponse<string>
                {
                    WasSuccess=true,
                    Result=Guid.NewGuid().ToString(),
                    Message= "Transaction approved.",
                });
            }
            return Ok(new ActionResponse<string>
            {
                WasSuccess = false,
                Message = "Transaction not approved."
            });
        }
    }
}
