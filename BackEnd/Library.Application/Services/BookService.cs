using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class BookService(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository) : IBookService
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthorRepository _authorRepository = authorRepository;

        public async Task<ApiResponse> AddBook(AddBookDTO dto)
        {
            if (dto != null)
            {
                if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrEmpty(dto.Description) || string.IsNullOrEmpty(dto.CoverImage))
                {
                    return new ApiResponse(400, "Title, Description, and CoverImage are required");
                }
                try
                {
                    var book = _mapper.Map<Book>(dto);
                    book.Authors ??= new List<Author>();

                    foreach (var authorId in dto.AuthorIds)
                    {
                        var author = await _authorRepository.GetAuthorById(authorId);
                        if (author == null)
                        {
                            return new ApiResponse(404, $"Author with ID {authorId} not found");
                        }
                        book.Authors.Add(author);
                    }

                    await _bookRepository.AddBook(book);
                    return new ApiResponse(200, "Added book successfully");
                }
                catch (Exception ex)
                {
                    return new ApiResponse(500, $"Failed to add book: {ex.Message}");
                }
            }

            return new ApiResponse(400, "Failed to add book");
        }


        public async Task<ApiResponse> DeleteBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book != null)
            {
                await _bookRepository.DeleteBook(book);
                return new ApiResponse(200, "Deleted Book");
            }
            return new ApiResponse(400, "Failed to delete");
        }

        public async Task<ApiResponse> EditBook(EditBookDTO dto)
        {
            var book = await _bookRepository.GetBookById(dto.BookId);

            if (book != null)
            {
                if (book.Title == dto.Title &&
                    book.Description == dto.Description &&
                    book.CoverImage == dto.CoverImage)
                    return new ApiResponse(400, "Nothing changed");

                if (string.IsNullOrEmpty(dto.Title) && string.IsNullOrEmpty(dto.Description) && string.IsNullOrEmpty(dto.CoverImage))
                    return new ApiResponse(400, "Don't leave inputs blank");

                book.Title = dto.Title;
                book.Description = dto.Description;
                book.CoverImage = dto.CoverImage;

                await _bookRepository.EditBook(book);
                return new ApiResponse(200, "Edited successfully");
            }
            return new ApiResponse(400, "Failed to edit");
        }

        public async Task<List<GetOnlyAuthorDTO>> GetAuthorOfABook(int bookId)
        {
            if (bookId == 0)
            {
                throw new Exception("book id cannot be 0");
            }
                var book = await _bookRepository.GetBookById(bookId) ?? throw new Exception($"Author with that book id {bookId} does not exist");
                var authorDTO = _mapper.Map<List<GetOnlyAuthorDTO>>(book.Authors);
                return authorDTO;                
         }
            
        

        public async Task<GetBookDTO> GetBookById(int id)
        {
            if (id == 0)
            {
                throw new Exception("id cannot be 0");
            }
                var book = await _bookRepository.GetBookById(id) ?? throw new Exception($"Book with that id {id} does not exist");
                var bookDTO = _mapper.Map<GetBookDTO>(book);
                return bookDTO ?? throw new Exception($"Book with that id {id} does not exist");                
        }
           
        

        public async Task<GetBookDTO> GetBookByTitle(string title)
        {
            if (title == null)
            {
                throw new Exception("Title cannot be null");
            }
            var book = await _bookRepository.GetBookByTitle(title) ?? throw new Exception($"Book with that title {title} does not exist");
            var bookDTO = _mapper.Map<GetBookDTO>(book);
            return bookDTO;               
         }          
        


        public async Task<List<GetBookDTO>> GetBooks()
        {
            var book = await _bookRepository.GetAllBooks();
            var bookList = _mapper.Map<List<GetBookDTO>>(book);
            return bookList;
        }
    }
}
