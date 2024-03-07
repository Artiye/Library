using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.RepositoryInterfaces
{
    public interface IBookRepository
    {
        Task<Book> AddBook(Book book);

        Task<Book> EditBook(Book book); 

        Task<Book> DeleteBook(Book book);   

        Task<Book> GetBookById(int id);

        Task<Book> GetBookByTitle(string title);

        Task<List<Book>> GetAllBooks();

        Task<List<Author>> GetAuthorOfBook(int bookId);
    }
}
