using Library.Application.DTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [HttpPost]

        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorDTO dto)
        {
            var response = await _authorService.AddAuthor(dto);
            return response.Status == 200 ? Ok(response) : BadRequest(response);
        }
        [HttpPut]
        public async Task<IActionResult> EditAuthor([FromBody] EditAuthorDTO dto)
        {
            var response = await _authorService.EditAuthor(dto);
            return response.Status == 200 ? Ok(response) : BadRequest(response);
        }
        [HttpDelete]

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var response = await _authorService.DeleteAuthor(id);
            return response.Status == 200 ? Ok(response) : BadRequest(response);
        }
        [HttpGet("{authorId}")]

        public async Task<IActionResult> GetAuthorById(int authorId)
        {
            var response = await _authorService.GetAuthorById(authorId);
            return Ok(response);
        }
        [HttpGet("byname/{authorName}")]

        public async Task<IActionResult> GetAuthorByName(string authorName)
        {
            var response = await _authorService.GetAuthorByName(authorName);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var response = await _authorService.GetAllAuthors();
            return Ok(response);
        }
        [HttpGet("{authorId}/books")]
        public async Task<IActionResult> GetBooksByAuthorId(int authorId)
        {
            var response = await _authorService.GetBooksByAuthorId(authorId);
            return Ok(response);
        }            
    }
}
