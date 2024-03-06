using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
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

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse> AddBook(AddBookDTO dto)
        {
            if (dto != null)
            {
                if (string.IsNullOrEmpty(dto.Title) && string.IsNullOrEmpty(dto.Description) && string.IsNullOrEmpty(dto.CoverImage))
                    return new ApiResponse(400, "Don't leave inputs blank");

                var book = _mapper.Map<Book>(dto);

                await _bookRepository.AddBook(book);
                return new ApiResponse(200, "Added book successfully");
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

        public async Task<List<GetAuthorDTO>> GetAuthorsOfABook(int bookId)
        {
           var authors = await _bookRepository.GetAuthorsOfBook(bookId);
           var authorDTO = _mapper.Map<List<GetAuthorDTO>>(authors);
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
