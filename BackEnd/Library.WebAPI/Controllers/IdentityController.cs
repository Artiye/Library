using Library.Application.DTOs.IdentityDTOs;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityController(IIdentityService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _service.Register(dto);
             return StatusCode(result.Status, result); ;
        }
        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<ApiResponse>> ConfirmEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if ((_userManager.ConfirmEmailAsync(user, code).Result.Succeeded == true))
            {
                return new ApiResponse(200, "User email has been confirmed!");
            };
            return StatusCode(400, "Confirmation failed!");
        }
        [HttpPost("changerole")]
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> ChangeUserRole(RoleChangeDTO dto)
        {
            var result = await _service.EditRole(dto);
            return StatusCode(result.Status, result);
        }
    }
}
