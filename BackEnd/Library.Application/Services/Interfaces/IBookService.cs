using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ResponseDTO;
using Library.Application.Responses;
using Library.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<ApiResponse> AddBook(AddBookDTO dto);

        Task<ApiResponse> EditBook(EditBookDTO dto);

        Task<ApiResponse> DeleteBook(int id);

        Task<ResponseDTO> GetBookById(int id);

        Task<ResponseDTO> GetBookByTitle(string title);

        Task<ResponseDTO> GetBooks();

        Task<ResponseDTO> GetAuthorOfABook(int bookId);

        Task<ResponseDTO> GetBooksByLanguage(string language);

        Task<ResponseDTO> GetBooksByGenre(string genre);
    }
}
