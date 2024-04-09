using Azure;
using Library.Application.DTOs.BookClubDTOs;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookClubController : ControllerBase
    {
        private readonly IBookClubService _bookClubService;

        public BookClubController(IBookClubService bookClubService)
        {
           _bookClubService = bookClubService;
        }
        
        [HttpPost]
        

        public async Task<IActionResult> AddBookClub([FromBody] AddBookClubDTOs dto)
        {
            var result = await _bookClubService.AddBookClub(dto);
            return StatusCode(result.Status, result);
        }
        
        [HttpPost("AddBookToBookClub")]
        
        public async Task<IActionResult> AddBookToBookClub(int bookClubId, int bookId)
        {
            var result = await _bookClubService.AddBookToBookClub(bookClubId, bookId);
            return StatusCode(result.Status, result);
        }
        
        [HttpPost("AddAuthorToBookClub")]
      
        public async Task<IActionResult> AddAuthorToBookClub(int bookClubId, int authorId)
        {
            var result = await _bookClubService.AddAuthorToBookClub(bookClubId, authorId);
            return StatusCode(result.Status, result);
        }
        
        [HttpPut]
       

        public async Task<IActionResult> EditBookClub([FromBody] EditBookClubDTO dto)
        {
            var result = await _bookClubService.EditBookClub(dto);
            return StatusCode(result.Status, result);
        }
       
        [HttpPut("RemoveBookFromBookClub")]
        
        public async Task<IActionResult> RemoveBookFromBookClub(int bookClubId, int bookId)
        {
            var result = await _bookClubService.RemoveBookFromBookClub(bookClubId, bookId);
            return StatusCode(result.Status, result);

        }
       
        [HttpPut("RemoveAuthorFromBookClub")]
        
        public async Task<IActionResult> RemoveAuthorToBookClub(int bookClubId, int authorId)
        {
            var result = await _bookClubService.RemoveAuthorFromBookClub(bookClubId, authorId);
            return StatusCode(result.Status, result);
        }
        [HttpPut("RemoveMemberFromBookClub")]

        public async Task<IActionResult> RemoveMemberFromBookClub(int bookClubId, string memberId, string reason)
        {
            var result = await _bookClubService.RemoveMemberFromBookClub(bookClubId, memberId, reason);
            return StatusCode(result.Status, result);
        }
        
        [HttpDelete]
        
        public async Task<IActionResult> DeleteBookClub(int id)
        {
            var result = await _bookClubService.DeleteBookClub(id);
            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpGet("{bookClubId}")]

        public async Task<IActionResult> GetBookClubById(int bookClubId)
        {
            var result = await _bookClubService.GetBookClubById(bookClubId);
            return StatusCode(result.Status, result);
        }
        [AllowAnonymous]
        [HttpGet("byname/{bookClubName}")]

        public async Task<IActionResult> GetBookClubByName(string bookClubName)
        {
            var result = await _bookClubService.GetBookClubByName(bookClubName);
            return StatusCode(result.Status, result);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBookClubs()
        {
            var result = await _bookClubService.GetBookClubs();
            return StatusCode(result.Status, result);
        }
        [AllowAnonymous]
        [HttpGet("bylanguage/{language}")]

        public async Task<IActionResult> GetBookClubByLanguage(string language)
        {
            var result = await _bookClubService.GetBookClubByLanguage(language);
            return StatusCode(result.Status, result);
        }
        [AllowAnonymous]
        [HttpGet("bygenre/{genre}")]
        public async Task<IActionResult> GetBookClubByGenre(string genre)
        {
            var result = await _bookClubService.GetBookClubByGenre(genre);
            return StatusCode(result.Status, result);
        }

        [HttpGet("joinrequests-for-bookclub")]
        public async Task<IActionResult> GetJoinRequestsForOwner(int bookClubId)
        {
            var result = await _bookClubService.GetJoinRequestsForOwner(bookClubId);
            return StatusCode(result.Status, result);
        }

        [HttpPost("request-to-join")]
        public async Task<IActionResult> RequestToJoinBookClub([FromQuery] int bookClubId, string reason)
        {
            var result = await _bookClubService.RequestToJoinBookClub(bookClubId, reason);
            return StatusCode(result.Status, result);
        }
        
        [HttpPost("accept-join-request")]
        public async Task<IActionResult> AcceptJoinRequest([FromQuery] int joinRequestId)
        {
            var result = await _bookClubService.AcceptJoinRequest(joinRequestId);
            return StatusCode(result.Status, result);
        }

        [HttpPost("deny-join-request")]
        public async Task<IActionResult> DenyJoinRequest([FromQuery] int joinRequestId, string reason)
        {
            var result= await _bookClubService.DenyJoinRequest(joinRequestId, reason);
            return StatusCode(result.Status, result);
        }
        
        
    }
}
