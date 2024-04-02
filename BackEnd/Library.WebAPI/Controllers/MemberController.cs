using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
            var members = await _memberService.GetAllMembers();
            return Ok(members);
        }
        [HttpGet("{memberId}")]

        public async Task<IActionResult> GetMemberById(string memberId)
        {
            var member = await _memberService.GetMemberById(memberId);
            return Ok(member);
        }
        [HttpGet("{memberId}/readlist")]
        public async Task<IActionResult> GetAMembersReadList(string memberId)
        {
            var result = await _memberService.GetAMembersReadList(memberId);
            return Ok(result);  
        }
    }
}
