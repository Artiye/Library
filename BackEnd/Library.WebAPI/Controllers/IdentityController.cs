using Library.Application.DTOs.IdentityDTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _service;

        public IdentityController(IIdentityService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _service.Register(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
    }
}
