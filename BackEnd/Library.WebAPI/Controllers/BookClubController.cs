using Library.Application.DTOs.BookClubDTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        

        public async Task<IActionResult> AddBookClub([FromBody] AddBookClubDTOs dto)
        {
            var result = await _bookClubService.AddBookClub(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddBookToBookClub")]
        
        public async Task<IActionResult> AddBookToBookClub(int bookClubId, int bookId)
        {
            var result = await _bookClubService.AddBookToBookClub(bookClubId, bookId);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddAuthorToBookClub")]
      
        public async Task<IActionResult> AddAuthorToBookClub(int bookClubId, int authorId)
        {
            var result = await _bookClubService.AddAuthorToBookClub(bookClubId, authorId);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
       

        public async Task<IActionResult> EditBookClub([FromBody] EditBookClubDTO dto)
        {
            var result = await _bookClubService.EditBookClub(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("RemoveBookFromBookClub")]
        
        public async Task<IActionResult> RemoveBookFromBookClub(int bookClubId, int bookId)
        {
            var result = await _bookClubService.RemoveBookFromBookClub(bookClubId, bookId);
            return result.Status == 200 ? Ok(result) : BadRequest(result);

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("RemoveAuthorFromBookClub")]
        
        public async Task<IActionResult> RemoveAuthorToBookClub(int bookClubId, int authorId)
        {
            var result = await _bookClubService.RemoveAuthorFromBookClub(bookClubId, authorId);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        
        public async Task<IActionResult> DeleteBookClub(int id)
        {
            var result = await _bookClubService.DeleteBookClub(id);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }

      
        [HttpGet("{bookClubId}")]

        public async Task<IActionResult> GetBookClubById(int bookClubId)
        {
            var result = await _bookClubService.GetBookClubById(bookClubId);
            return Ok(result);
        }
       
        [HttpGet("byname/{bookClubName}")]

        public async Task<IActionResult> GetBookClubByName(string bookClubName)
        {
            var result = await _bookClubService.GetBookClubByName(bookClubName);
            return Ok(result);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllBookClubs()
        {
            var result = await _bookClubService.GetBookClubs();
            return Ok(result);
        }
    }
}
