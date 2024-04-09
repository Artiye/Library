using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var result = await _memberService.GetAllMembers();
            return StatusCode(result.Status, result);
        }
        [HttpGet("{memberId}")]

        public async Task<IActionResult> GetMemberById(string memberId)
        {
            var result = await _memberService.GetMemberById(memberId);
            return StatusCode(result.Status, result);
        }
        [HttpGet("{memberId}/readlist")]
        public async Task<IActionResult> GetAMembersReadList(string memberId)
        {
            var result = await _memberService.GetAMembersReadList(memberId);
            return StatusCode(result.Status, result);
        }
        [HttpGet("{memberId}/favouriteAuthors")]

        public async Task<IActionResult> GetAMembersFavouriteAuthors(string memberId)
        {
            var result = await _memberService.GetAMembersFavouriteAuthors(memberId);
            return StatusCode(result.Status, result);
        }
    }
}
