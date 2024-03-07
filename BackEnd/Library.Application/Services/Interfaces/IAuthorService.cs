using Library.Application.DTOs.AuthorDTOs;
using Library.Application.Responses;
using Library.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<ApiResponse> AddAuthor(AddAuthorDTO dto);

        Task<ApiResponse> EditAuthor(EditAuthorDTO dto);

        Task<ApiResponse> DeleteAuthor(int id);

        Task<GetAuthorDTO> GetAuthorById(int id);

        Task<GetAuthorDTO> GetAuthorByName(string name);

        Task<List<GetAuthorDTO>> GetAllAuthors();

        Task<List<Book>> GetBooksByAuthorId(int authorId);
     
        Task<ApiResponse> AddBookToAuthor(int authorId, int bookId);

        Task<ApiResponse> RemoveBookFromAuthor(int authorId, int bookId);
    }
}
