using Library.Application.DTOs.AuthorDTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Library.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorDTO dto)
        {
            var response = await _authorService.AddAuthor(dto);
            return response.Status == 200 ? Ok(response) : BadRequest(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddAuthorToBook")]
        
        public async Task<IActionResult> AddBooksToAuthor( int authorId,  int bookId)
        {
            var response = await _authorService.AddBookToAuthor(authorId, bookId);
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("RemoveBookFromAuthor")]
       
        public async Task<IActionResult> RemoveBookFromAuthor(int authorId, int bookId)
        {
            var response = await _authorService.RemoveBookFromAuthor(authorId, bookId);
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
       
        public async Task<IActionResult> EditAuthor([FromBody] EditAuthorDTO dto)
        {
            var response = await _authorService.EditAuthor(dto);
            return response.Status == 200 ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var response = await _authorService.DeleteAuthor(id);
            return response.Status == 200 ? Ok(response) : BadRequest(response);
        }

        [AllowAnonymous]
        [HttpGet("{authorId}")]

        public async Task<IActionResult> GetAuthorById(int authorId)
        {
            var response = await _authorService.GetAuthorById(authorId);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpGet("byname/{authorName}")]

        public async Task<IActionResult> GetAuthorByName(string authorName)
        {
            var response = await _authorService.GetAuthorByName(authorName);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpGet]
        
        public async Task<IActionResult> GetAllAuthors()
        {
            var response = await _authorService.GetAllAuthors();
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpGet("{authorId}/books")]
        public async Task<IActionResult> GetBooksByAuthorId(int authorId)
        {
            var response = await _authorService.GetBooksByAuthorId(authorId);
            return Ok(response);
        }            
    }
}
