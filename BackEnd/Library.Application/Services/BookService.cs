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
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IAuthorRepository _authorRepository;

        public BookService(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _authorRepository = authorRepository;
        }
       
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

                    
                    if (book.Authors == null)
                    {
                        book.Authors = new List<Author>();
                    }

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
            if(book != null)
            {
                await _bookRepository.DeleteBook(book);
                return new ApiResponse(200, "Deleted Book");
            }
            return new ApiResponse(400, "Failed to delete");
        }

        public async Task<ApiResponse> EditBook(EditBookDTO dto)
        {
            var book = await _bookRepository.GetBookById(dto.BookId);

            if(book != null)
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
           var book = await _bookRepository.GetBookById(bookId);

           var authorDTO = _mapper.Map<List<GetOnlyAuthorDTO>>(book.Authors);
           return authorDTO;
        }

        public async Task<GetBookDTO> GetBookById(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            var bookDTO = _mapper.Map<GetBookDTO>(book);
            return bookDTO;
        }

        public async Task<GetBookDTO> GetBookByTitle(string title)
        {
            var book = await _bookRepository.GetBookByTitle(title);
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
