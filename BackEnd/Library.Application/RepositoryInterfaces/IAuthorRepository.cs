using Library.Application.DTOs.BookClubDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.RepositoryInterfaces
{
    public interface IAuthorRepository
    {
        Task<Author> AddAuthor(Author author);
       

        Task<Author> EditAuthor(Author author);

        Task<Author> DeleteAuthor(Author author);

        Task<Author> GetAuthorById(int id);

        Task<Author> GetAuthorByName(string name);

        Task<List<Author>> GetAllAuthorList();

        Task<List<GetBookDTO>> GetBooksByAuthorId(int authorId);

       


    }
}
