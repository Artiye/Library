using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.Responses;
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

        Task<GetBookDTO> GetBookById(int id);

        Task<GetBookDTO> GetBookByTitle(string title);

        Task<List<GetBookDTO>> GetBooks();

        Task<List<GetAuthorDTO>> GetAuthorsOfABook(int bookId);
    }
}
