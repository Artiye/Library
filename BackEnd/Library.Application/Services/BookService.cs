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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class BookService(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository, IEncryptionService encryptionService, ILogger<BookService> logger) : IBookService
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IEncryptionService _encryptionService = encryptionService;
        private readonly ILogger<BookService> _logger = logger;

        public async Task<ApiResponse> AddBook(AddBookDTO dto)
        {
            if (dto != null)
            {
                if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrEmpty(dto.Description) || string.IsNullOrEmpty(dto.CoverImage))
                {
                    _logger.LogWarning("AddBook: Title, Description and CoverImage must be filled in and not left blank or null");
                    return new ApiResponse(400, "Title, Description, and CoverImage are required");
                }
                if (dto.AuthorIds == null || !dto.AuthorIds.Any())
                {
                    _logger.LogWarning("AddBook: An author must be provided when adding a book");
                    return new ApiResponse(400, "At least one author is required");
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
                            _logger.LogWarning("AddBook: Author with the provided id was not found");
                            return new ApiResponse(404, $"Author with ID {authorId} not found");
                        }
                        book.Authors.Add(author);
                    }

                    await _bookRepository.AddBook(book);
                    _logger.LogInformation("AddBook: Book has been added successfully");
                    return new ApiResponse(200, "Added book successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AddBook: Failed to add book");
                    return new ApiResponse(400, $"Failed to add book: {ex.Message}");
                }
            }
            _logger.LogError("AddBook: Failed to add book");
            return new ApiResponse(400, "Failed to add book");
        }


        public async Task<ApiResponse> DeleteBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book != null)
            {
                _logger.LogInformation("DeleteBook: Book deleted successfully");
                await _bookRepository.DeleteBook(book);
                return new ApiResponse(200, "Deleted Book");
            }
            _logger.LogError("DeleteBook: Book failed to be deleted");
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
                {
                    _logger.LogWarning("EditBook: Nothing was edited");
                    return new ApiResponse(400, "Nothing changed");
                }

                if (string.IsNullOrEmpty(dto.Title) && string.IsNullOrEmpty(dto.Description) && string.IsNullOrEmpty(dto.CoverImage))
                {
                    _logger.LogWarning("EditBook: Inputs were left blank");
                    return new ApiResponse(400, "Don't leave inputs blank");
                }

                book.Title = _encryptionService.EncryptData(dto.Title);
                book.Description = _encryptionService.EncryptData(dto.Description);
                book.CoverImage = _encryptionService.EncryptData(dto.CoverImage);

                await _bookRepository.EditBook(book);
                _logger.LogInformation("EditBook: Book edited successfully");
                return new ApiResponse(200, "Edited successfully");
            }
            _logger.LogError("EditBook: Book has failed to be edited");
            return new ApiResponse(400, "Failed to edit");
        }

        public async Task<ResponseDTO> GetAuthorOfABook(int bookId)
        {
            var response = new ResponseDTO();
            if (bookId == 0)
            {
                _logger.LogWarning("GetAuthorOfABook: Id provided cannot be 0");
                response.Message = "Id cannot be 0";
                return response;
            }
            var book = await _bookRepository.GetBookById(bookId);
            if(book == null)
            {
                _logger.LogWarning("GetAuthorOfABook: Author with the book id provided does not exist");
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
            _logger.LogInformation("GetAuthorOfABook: Author successfully retrieved");
            response.Status = 200;
            response.Result = authorDTO;
            return response;
         }
            
        

        public async Task<ResponseDTO> GetBookById(int id)
        {
            var response = new ResponseDTO();
            if (id == 0)
            {
                _logger.LogWarning("GetBookById: Id provided cannot be 0");
                response.Message = "id cannot be 0";
                return response;
            }
            var book = await _bookRepository.GetBookById(id);
            if(book == null)
            {
                _logger.LogWarning("GetBookById: Book with the provided id does not exist");
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
            _logger.LogInformation("GetBookById: Book successfully retrieved");
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
                _logger.LogWarning("GetBookByTitle: Title provided is null");
                response.Message = "Title cannot be null";
                return response;
            }
            var book = await _bookRepository.GetBookByTitle(encryptedTitle);

                if(book == null)
            {
                _logger.LogWarning("GetBookByTitle: Book with the title provided does not exist");
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
            _logger.LogInformation("GetBookByTitle: Book successfully retrieved");
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
                _logger.LogWarning("GetBooks: No books found in the database");
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

            _logger.LogInformation("GetBooks: Books successfully retrieved");
            response.Status = 200;
            response.Result = bookList;
            return response;
        }
        public async Task<ResponseDTO> GetBooksByLanguage(string language)
        {
            var response = new ResponseDTO();

            if (language == null)
            {
                _logger.LogWarning("GetBooksByLanguage: Language provided is null");
                response.Message = "Language cannot be null";
                return response;
            }

            var book = await _bookRepository.GetBooksByLanguage(language);

            if (book.Count == 0)
            {
                _logger.LogWarning("GetBooksByLanguage: Books with the language provided do not exist");
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
            _logger.LogInformation("GetBooksByLanguage: Books successfully retrieved");
            response.Status = 200;
            response.Result = bookList;
            return response;
        }

        public async Task<ResponseDTO> GetBooksByGenre(string genre) {
            var response = new ResponseDTO();

            if (genre == null)
            {
                _logger.LogWarning("GetBooksByGenre: Genre provided cannot be null");
                response.Message = "Genre cannot be null";
                return response;
            }

            var book = await _bookRepository.GetBooksByGenre(genre);

            if (book.Count == 0)
            {
                _logger.LogWarning("GetBooksByGenre: Books with the genre provided do not exist");
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
            _logger.LogInformation("GetBooksByGenre: Books successfully retrieved");
            response.Status = 200;
            response.Result = bookList;
            return response;
        
        }      
    }
}
