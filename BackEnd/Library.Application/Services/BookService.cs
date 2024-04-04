using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
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

        public async Task<List<GetOnlyAuthorDTO>> GetAuthorOfABook(int bookId)
        {
            if (bookId == 0)
            {
                throw new Exception("book id cannot be 0");
            }
                var book = await _bookRepository.GetBookById(bookId) ?? throw new Exception($"Author with that book id {bookId} does not exist");
                var authorDTO = _mapper.Map<List<GetOnlyAuthorDTO>>(book.Authors);
            foreach (GetOnlyAuthorDTO author1 in authorDTO)
            {
                author1.BioGraphy = _encryptionService.DecryptData(author1.BioGraphy);
                author1.FullName = _encryptionService.DecryptData(author1.FullName);
                author1.Nationality = _encryptionService.DecryptData(author1.Nationality);
                author1.ProfileImage = _encryptionService.DecryptData(author1.ProfileImage);
            }
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

                return bookDTO ?? throw new Exception($"Book with that id {id} does not exist");                
        }
           
        

        public async Task<GetBookDTO> GetBookByTitle(string title)
        {
            var encryptedTitle = _encryptionService.EncryptData(title);
            if (encryptedTitle == null)
            {
                throw new Exception("Title cannot be null");
            }
            var book = await _bookRepository.GetBookByTitle(encryptedTitle) ?? throw new Exception($"Book with that title {title} does not exist");
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
            return bookDTO;               
         }          
        


        public async Task<List<GetBookDTO>> GetBooks()
        {
            var book = await _bookRepository.GetAllBooks();
            var bookList = _mapper.Map<List<GetBookDTO>>(book);
            foreach(GetBookDTO books in bookList)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);

                books.Authors = await GetAuthorOfABook(books.BookId);
            }
           

            return bookList;
        }
        public async Task<List<GetBookDTO>> GetBooksByLanguage(string language)
        {
            if(language == null)
            {
                throw new Exception("language cannot be null");
            }
            var book = await _bookRepository.GetBooksByLanguage(language);

            if(book.Count == 0)            
                throw new Exception($"Books with language {language} do not exist");
            
            var bookList = _mapper.Map<List<GetBookDTO>>(book);
            foreach (GetBookDTO books in bookList)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);

             foreach(GetOnlyAuthorDTO authors in books.Authors)
                {
                    authors.BioGraphy = _encryptionService.DecryptData(authors.BioGraphy);
                    authors.FullName = _encryptionService.DecryptData(authors.FullName);
                    authors.Nationality = _encryptionService.DecryptData(authors.Nationality);
                    authors.ProfileImage = _encryptionService.DecryptData(authors.ProfileImage);
                }

            }

            return bookList;
        }
        public async Task<List<GetBookDTO>> GetBooksByGenre(string genre) { 
            if (genre == null)
            {
                throw new Exception("genre cannot be null");
            }

            var book = await _bookRepository.GetBooksByGenre(genre);

            if (book.Count == 0)
                throw new Exception($"Books with that genre {genre} do not exist");

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
            return bookList;
        
        }      
    }
}
