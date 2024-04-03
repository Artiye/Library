using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.Encryption;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class AuthorService(IAuthorRepository authorRepository, IMapper mapper, IBookRepository bookRepository, IEncryptionService encryptionService) : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IEncryptionService _encryptionService = encryptionService;

        public async Task<ApiResponse> AddAuthor(AddAuthorDTO dto)
        {
            if (dto != null)
            {
                if (string.IsNullOrEmpty(dto.FullName) && string.IsNullOrEmpty(dto.Nationality) && string.IsNullOrEmpty(dto.BioGraphy))
                    return new ApiResponse(400, "Do not leave empty or null strings");

                
                var author = _mapper.Map<Author>(dto);

                author.FullName = _encryptionService.EncryptData(dto.FullName);
                author.Biography = _encryptionService.EncryptData(dto.BioGraphy);
                author.Nationality = _encryptionService.EncryptData(dto.Nationality);
                author.ProfileImage = _encryptionService.EncryptData(dto.ProfileImage);

                await _authorRepository.AddAuthor(author);
                return new ApiResponse(200, "Added author successfully");

            }
            return new ApiResponse(400, "Failed to add author");
            }

       
        public async Task<ApiResponse> AddBookToAuthor(int authorId, int bookId)
        {
            var author = await _authorRepository.GetAuthorById(authorId);


            var book = await _bookRepository.GetBookById(bookId);
            if(author != null && book != null)
            {
                author.Books ??= new List<Book>();
                if (author.Books.Any(b => b.BookId == bookId))
                    return new ApiResponse(400, "The book you're trying to add already exists");

                author.Books.Add(book);
                await _authorRepository.EditAuthor(author);
                return new ApiResponse(200, "Added book to author");

            }
            return new ApiResponse(400, "Failed book to author");


        }

        public async Task<ApiResponse> DeleteAuthor(int id)
        {
            var author = await _authorRepository.GetAuthorById(id);
            if (author != null)
            {
                await _authorRepository.DeleteAuthor(author);
                return new ApiResponse(200, "Deleted author");
            }
            return new ApiResponse(400, "Failed to delete");
        }

        public async Task<ApiResponse> EditAuthor(EditAuthorDTO dto)
        {
            var author = await _authorRepository.GetAuthorById(dto.AuthorId);
            if(author != null)
            {
                if (author.Biography == dto.Biography && 
                    author.FullName == dto.FullName &&                   
                    author.Nationality == dto.Nationality &&
                    author.ProfileImage == dto.ProfileImage)
                return new ApiResponse(400, "Nothing was edited");

                if (string.IsNullOrEmpty(dto.FullName) && string.IsNullOrEmpty(dto.Nationality) && string.IsNullOrEmpty(dto.Biography))
                    return new ApiResponse(400, "Do not leave empty or null strings");

                author.Biography = _encryptionService.EncryptData(dto.Biography);
                author.FullName = _encryptionService.EncryptData(dto.FullName);
                author.Nationality = _encryptionService.EncryptData(dto.Nationality);               
                author.ProfileImage = _encryptionService.EncryptData(dto.ProfileImage);

                await _authorRepository.EditAuthor(author);
                return new ApiResponse(200, "Edited author successfully");

            }
            return new ApiResponse(400, "Failed to edit");
        }

        public async Task<List<GetAuthorDTO>> GetAllAuthors()
        {
            var author = await _authorRepository.GetAllAuthorList();            
            var authorList = _mapper.Map<List<GetAuthorDTO>>(author);
            foreach(GetAuthorDTO author1 in authorList)
            {
                author1.BioGraphy = _encryptionService.DecryptData(author1.BioGraphy);
                author1.FullName = _encryptionService.DecryptData(author1.FullName);
                author1.Nationality = _encryptionService.DecryptData(author1.Nationality);
                author1.ProfileImage = _encryptionService.DecryptData(author1.ProfileImage);
            }

            
            return authorList;
        }

        public async Task<GetAuthorDTO> GetAuthorById(int id)
        {
            if (id == 0)
            {
                throw new Exception("Author id cannot be 0");
            }

            var author = await _authorRepository.GetAuthorById(id) ?? throw new Exception($"Author with id {id} does not exist");
            var authorDTO = _mapper.Map<GetAuthorDTO>(author);
            authorDTO.BioGraphy = _encryptionService.DecryptData(author.Biography);
            authorDTO.FullName = _encryptionService.DecryptData(author.FullName);
            authorDTO.Nationality = _encryptionService.DecryptData(author.Nationality);
            authorDTO.ProfileImage = _encryptionService.DecryptData(author.ProfileImage);
            return authorDTO;
        }

        public async Task<GetAuthorDTO> GetAuthorByName(string name)
        {
            var encryptedName = _encryptionService.EncryptData(name);
            if (encryptedName == null)
            {
                throw new Exception("Name cannot be null");
            }
            var author = await _authorRepository.GetAuthorByName(encryptedName) ?? throw new Exception($"Author with that name {encryptedName} does not exist");
            var authorDTO = _mapper.Map<GetAuthorDTO>(author);
            authorDTO.BioGraphy = _encryptionService.DecryptData(author.Biography);
            authorDTO.FullName = _encryptionService.DecryptData(author.FullName);
            authorDTO.Nationality = _encryptionService.DecryptData(author.Nationality);
            authorDTO.ProfileImage = _encryptionService.DecryptData(author.ProfileImage);
            return authorDTO;                          
        }

        public async Task<List<Book>> GetBooksByAuthorId(int authorId)
        {
            if (authorId == 0)
            {
                throw new Exception("Author id cannot be 0");
            }
            var books = await _authorRepository.GetBooksByAuthorId(authorId) ?? throw new Exception($"Author with id {authorId} does not exist");
            foreach(Book book in books)
            {
                book.Title = _encryptionService.DecryptData(book.Title);
                book.Description = _encryptionService.DecryptData(book.Description);
                book.CoverImage = _encryptionService.DecryptData(book.CoverImage);
            }
            return books;
            }
           
        

        public  async Task<ApiResponse> RemoveBookFromAuthor(int authorId, int bookId)
        {
            var author = await _authorRepository.GetAuthorById(authorId);

            var book = await _bookRepository.GetBookById(bookId);
            if (author != null && book != null)
            {
                author.Books ??= new List<Book>();

                author.Books.Remove(book);
                await _authorRepository.EditAuthor(author);
                return new ApiResponse(200, "Removed book from author");

            }
            return new ApiResponse(400, "Failed  to remove book from author");
        }
    }
}
