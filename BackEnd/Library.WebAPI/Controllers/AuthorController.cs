using Library.Application.DTOs.AuthorDTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
            var result = await _authorService.AddAuthor(dto);
            return StatusCode(result.Status, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddAuthorToBook")]
        
        public async Task<IActionResult> AddBooksToAuthor( int authorId,  int bookId)
        {
            var result = await _authorService.AddBookToAuthor(authorId, bookId);
            return StatusCode(result.Status, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("RemoveBookFromAuthor")]
       
        public async Task<IActionResult> RemoveBookFromAuthor(int authorId, int bookId)
        {
            var result = await _authorService.RemoveBookFromAuthor(authorId, bookId);
            return StatusCode(result.Status, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
       
        public async Task<IActionResult> EditAuthor([FromBody] EditAuthorDTO dto)
        {
            var result = await _authorService.EditAuthor(dto);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var result = await _authorService.DeleteAuthor(id);
            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpGet("{authorId}")]

        public async Task<IActionResult> GetAuthorById(int authorId)
        {
            var result = await _authorService.GetAuthorById(authorId);
            return StatusCode(result.Status, result);
        }
        [AllowAnonymous]
        [HttpGet("byname/{authorName}")]

        public async Task<IActionResult> GetAuthorByName(string authorName)
        {
            var result = await _authorService.GetAuthorByName(authorName);
            return StatusCode(result.Status, result);
        }
        [AllowAnonymous]
        [HttpGet]
        
        public async Task<IActionResult> GetAllAuthors()
        {
            var result = await _authorService.GetAllAuthors();
            return StatusCode(result.Status, result);
        }
        [AllowAnonymous]
        [HttpGet("{authorId}/books")]
        public async Task<IActionResult> GetBooksByAuthorId(int authorId)
        {
            var result = await _authorService.GetBooksByAuthorId(authorId);
            return StatusCode(result.Status, result);
        }            
    }
}
