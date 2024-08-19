using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;
using Org.BouncyCastle.Crypto.Operators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderPlus.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly IMailHelper _mailHelper;

        public AccountsController(IUsersUnitOfWork usersUnitOfWork, IConfiguration configuration,
           IFileStorage fileStorage, IMailHelper mailHelper)
        {
            _usersUnitOfWork = usersUnitOfWork;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _mailHelper = mailHelper;
        }
        [HttpGet("recordsNumber")]
        public async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _usersUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _usersUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync(PaginationDTO pagination)
        {
            var response = await _usersUnitOfWork.GetTotalPagesAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            var myToken = await _usersUnitOfWork.GeneratePasswordResetTokenAsync(user);
            var tokenLink = Url.Action("ResetPassword", "accounts",
            new
            {
                userid = user.Id,
                token = myToken,
            }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);
            var response = _mailHelper.SendMail(user.FullName, user.Email!,
               $"Order - Password Recovery",
               $"<h1>Order - Password Recovery</h1>" +
               $"<p>To recover your password, please click 'Recover Password'':</p>" +
               $"<b><a href ={tokenLink}>Recover Password</a></b>");

            if (response.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _usersUnitOfWork.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }
        [HttpPost("ResedToken")]
        public async Task<IActionResult> ResedTokenAsync([FromBody] EmailDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var response = await SendConfirmationEmailAsync(user);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        private async Task<ActionResponse<string>> SendConfirmationEmailAsync(User user)
        {
            var myToken = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "accounts", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

            return _mailHelper.SendMail(user.FullName, user.Email!,
                $"Order - Account Confirmation",
                $"<h1>Order - Account Confirmation</h1>" +
                $"<p>To enable the user, please click 'Confirm Email':</p>" +
                $"<b><a href ={tokenLink}>Confirm Email</a></b>");

        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            token = token.Replace(" ", "+");
            var user = await _usersUnitOfWork.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }
            var result = await _usersUnitOfWork.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }
            return NoContent();
        }
        //TODO:
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;
            if(!string.IsNullOrEmpty(model.Photo))
            {
                var photoUser=Convert.FromBase64String(model.Photo);
                model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", "users");
            };
            var result = await _usersUnitOfWork.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _usersUnitOfWork.AddUserToRoleAsync(user, user.UserType.ToString());
                var response=await SendConfirmationEmailAsync(user);
                if(response.WasSuccess)
                {
                    return NoContent();
                }
                return BadRequest(response.Message);
            }
            return BadRequest(result.Errors.FirstOrDefault());
        }
        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _usersUnitOfWork.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }

            return NoContent();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
        {
            var result = await _usersUnitOfWork.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _usersUnitOfWork.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }

            if (result.IsLockedOut)
            {
                return BadRequest("You have exceeded the maximum number of attempts, your account is blocked, please try again in 5 minutes.");
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("The user has not been enabled, you must follow the instructions in the email sent to enable the user.");
            }

            return BadRequest("Incorrect email or password.");
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.CountryCode + user.PhoneNumber),                
                new("FirstName", user.FirstName),
                new("LastName", user.LastName),
                new("Address", user.Address),
                new("Photo", user.Photo ?? string.Empty),
                new("CityId", user.CityId.ToString())

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials=new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddYears(1);
            var token = new JwtSecurityToken
            (
               issuer:null,
               audience:null,
               claims:claims,
               expires:expiration,
               signingCredentials:credentials
            );
            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,

            };
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!));
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutAsync(User user)
        {
            try
            {
                var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
                if (currentUser == null)
                {
                    return NotFound();
                }
                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser = Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", "users");
                }
                currentUser.PhoneNumber = user.PhoneNumber;
                currentUser.Email = user.Email;
                currentUser.UserName = user.UserName;
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Address = user.Address;
                currentUser.CountryCode = user.CountryCode;
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo :
                    currentUser.Photo;
                currentUser.CityId = user.CityId;

                var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    return Ok(BuildToken(currentUser));
                }

                return BadRequest(result.Errors.FirstOrDefault());
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
