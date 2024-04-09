using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ResponseDTO;
using Library.Application.Encryption;
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
    public class BookService(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository, IEncryptionService encryptionService) : IBookService
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IEncryptionService _encryptionService = encryptionService;

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
                    book.Title = _encryptionService.EncryptData(dto.Title);
                    book.Description = _encryptionService.EncryptData(dto.Description);
                    book.CoverImage = _encryptionService.EncryptData(dto.CoverImage);
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

                book.Title = _encryptionService.EncryptData(dto.Title);
                book.Description = _encryptionService.EncryptData(dto.Description);
                book.CoverImage = _encryptionService.EncryptData(dto.CoverImage);

                await _bookRepository.EditBook(book);
                return new ApiResponse(200, "Edited successfully");
            }
            return new ApiResponse(400, "Failed to edit");
        }

        public async Task<ResponseDTO> GetAuthorOfABook(int bookId)
        {
            var response = new ResponseDTO();
            if (bookId == 0)
            {
                response.Message = "Id cannot be 0";
                return response;
            }
            var book = await _bookRepository.GetBookById(bookId);
            if(book == null)
            {
                response.Message = $"Author with the id {bookId} does not exist";
                return response;
            }
                var authorDTO = _mapper.Map<List<GetOnlyAuthorDTO>>(book.Authors);
            foreach (GetOnlyAuthorDTO author1 in authorDTO)
            {
                author1.BioGraphy = _encryptionService.DecryptData(author1.BioGraphy);
                author1.FullName = _encryptionService.DecryptData(author1.FullName);
                author1.Nationality = _encryptionService.DecryptData(author1.Nationality);
                author1.ProfileImage = _encryptionService.DecryptData(author1.ProfileImage);
            }
            response.Status = 200;
            response.Result = authorDTO;
            return response;
         }
            
        

        public async Task<ResponseDTO> GetBookById(int id)
        {
            var response = new ResponseDTO();
            if (id == 0)
            {
                response.Message = "id cannot be 0";
                return response;
            }
            var book = await _bookRepository.GetBookById(id);
            if(book == null)
            {
                response.Message = $"Book with that id {id} does not exist";
                return response;
            }
                var bookDTO = _mapper.Map<GetBookDTO>(book);
                bookDTO.Title = _encryptionService.DecryptData(book.Title);
                bookDTO.Description = _encryptionService.DecryptData(book.Description);
                bookDTO.CoverImage = _encryptionService.DecryptData(book.CoverImage);

                 foreach(var author1 in bookDTO.Authors)
                {
                    author1.BioGraphy = _encryptionService.DecryptData(author1.BioGraphy);
                    author1.FullName = _encryptionService.DecryptData(author1.FullName);
                    author1.Nationality = _encryptionService.DecryptData(author1.Nationality);
                    author1.ProfileImage = _encryptionService.DecryptData(author1.ProfileImage);
                }

            response.Status = 200;
            response.Result = bookDTO;
            return response;
        }
           
        

        public async Task<ResponseDTO> GetBookByTitle(string title)
        {
            var response = new ResponseDTO();

            var encryptedTitle = _encryptionService.EncryptData(title);
            if (encryptedTitle == null)
            {
                response.Message = "Title cannot be null";
                return response;
            }
            var book = await _bookRepository.GetBookByTitle(encryptedTitle);

                if(book == null)
            {
                response.Message = $"Book with that title {title} does not exist";
                return response;
            }
            var bookDTO = _mapper.Map<GetBookDTO>(book);
            bookDTO.Title = _encryptionService.DecryptData(book.Title);
            bookDTO.Description = _encryptionService.DecryptData(book.Description);
            bookDTO.CoverImage = _encryptionService.DecryptData(book.CoverImage);

            foreach (var author1 in bookDTO.Authors)
            {
                author1.BioGraphy = _encryptionService.DecryptData(author1.BioGraphy);
                author1.FullName = _encryptionService.DecryptData(author1.FullName);
                author1.Nationality = _encryptionService.DecryptData(author1.Nationality);
                author1.ProfileImage = _encryptionService.DecryptData(author1.ProfileImage);
            }
            response.Status = 200;
            response.Result = bookDTO;
            return response;
         }          
        


        public async Task<ResponseDTO> GetBooks()
        {
            var response = new ResponseDTO();

            var book = await _bookRepository.GetAllBooks();
            if(book.Count == 0)
            {
                response.Message = "No books found";
                return response;
            }
            var bookList = _mapper.Map<List<GetBookDTO>>(book);
            foreach(GetBookDTO books in bookList)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);

               foreach(GetOnlyAuthorDTO authors in  books.Authors)
                {
                    authors.BioGraphy = _encryptionService.DecryptData(authors.BioGraphy);
                    authors.FullName = _encryptionService.DecryptData(authors.FullName);
                    authors.Nationality = _encryptionService.DecryptData(authors.Nationality);
                    authors.ProfileImage = _encryptionService.DecryptData(authors.ProfileImage);
                }
            }
           

            response.Status = 200;
            response.Result = bookList;
            return response;
        }
        public async Task<ResponseDTO> GetBooksByLanguage(string language)
        {
            var response = new ResponseDTO();

            if (language == null)
            {
                response.Message = "Language cannot be null";
                return response;
            }

            var book = await _bookRepository.GetBooksByLanguage(language);

            if (book.Count == 0)
            {
                response.Message = $"Books with language {language} do not exist";
                return response;
            }

            var bookList = _mapper.Map<List<GetBookDTO>>(book);
            foreach (GetBookDTO books in bookList)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);

                foreach (GetOnlyAuthorDTO authors in books.Authors)
                {
                    authors.BioGraphy = _encryptionService.DecryptData(authors.BioGraphy);
                    authors.FullName = _encryptionService.DecryptData(authors.FullName);
                    authors.Nationality = _encryptionService.DecryptData(authors.Nationality);
                    authors.ProfileImage = _encryptionService.DecryptData(authors.ProfileImage);
                }
            }

            response.Status = 200;
            response.Result = bookList;
            return response;
        }

        public async Task<ResponseDTO> GetBooksByGenre(string genre) {
            var response = new ResponseDTO();

            if (genre == null)
            {
                response.Message = "Genre cannot be null";
                return response;
            }

            var book = await _bookRepository.GetBooksByGenre(genre);

            if (book.Count == 0)
            {
                response.Message = $"Books with that genre {genre} do not exist";
                return response;
            }

            var bookList = _mapper.Map<List<GetBookDTO>>(book);
            foreach (GetBookDTO books in bookList)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);

                foreach (GetOnlyAuthorDTO authors in books.Authors)
                {
                    authors.BioGraphy = _encryptionService.DecryptData(authors.BioGraphy);
                    authors.FullName = _encryptionService.DecryptData(authors.FullName);
                    authors.Nationality = _encryptionService.DecryptData(authors.Nationality);
                    authors.ProfileImage = _encryptionService.DecryptData(authors.ProfileImage);
                }
            }
            response.Status = 200;
            response.Result = bookList;
            return response;
        
        }      
    }
}
