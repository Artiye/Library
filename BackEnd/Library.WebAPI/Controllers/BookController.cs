using Library.Application.DTOs.BookDTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [HttpPost]

        public async Task<IActionResult> AddBook(AddBookDTO dto)
        {
            var result = await _bookService.AddBook(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpPut]

        public async Task<IActionResult> EditBook(EditBookDTO dto)
        {
            var result = await _bookService.EditBook(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBook(id);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{bookId}")]

        public async Task<IActionResult> GetBookById(int bookId)
        {
            var result = await _bookService.GetBookById(bookId);
            return Ok(result);
        }
        [HttpGet("bytitle/{bookTitle}")]
        public async Task<IActionResult> GetBookByTitle(string bookTitle)
        {
            var result = await _bookService.GetBookByTitle(bookTitle);
            return Ok(result);
        }
        [HttpGet]

        public async Task<IActionResult> GetBooks()
        {
            var result = await _bookService.GetBooks();
            return Ok(result);
        }
        [HttpGet("{bookId}/authors")]

        public async Task<IActionResult> GetAuthorsOfBook(int bookId)
        {
            var result = await _bookService.GetAuthorsOfABook(bookId);
            return Ok(result);
        }
    }
}
