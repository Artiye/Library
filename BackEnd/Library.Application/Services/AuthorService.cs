﻿using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ResponseDTO;
using Library.Application.Encryption;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class AuthorService(IAuthorRepository authorRepository, IMapper mapper, IBookRepository bookRepository, IEncryptionService encryptionService, ILogger<AuthorService> logger) : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IEncryptionService _encryptionService = encryptionService;
        private readonly ILogger<AuthorService> _logger = logger;

        public async Task<ApiResponse> AddAuthor(AddAuthorDTO dto)
        {           
                if (dto != null)
                {
                
                    if (string.IsNullOrEmpty(dto.FullName) && string.IsNullOrEmpty(dto.Nationality) && string.IsNullOrEmpty(dto.BioGraphy))
                    {
                        _logger.LogWarning("AddAuthor: Empty or null strings found in dto");
                        return new ApiResponse(400, "Do not leave empty or null strings");
                    }


                    var author = _mapper.Map<Author>(dto);

                    author.FullName = _encryptionService.EncryptData(dto.FullName);
                    author.Biography = _encryptionService.EncryptData(dto.BioGraphy);
                    author.Nationality = _encryptionService.EncryptData(dto.Nationality);
                    author.ProfileImage = _encryptionService.EncryptData(dto.ProfileImage);

                    await _authorRepository.AddAuthor(author);
                    _logger.LogInformation("AddAuthor: Author added successfully");
                    return new ApiResponse(200, "Added author successfully");

                }
            _logger.LogError("AddAuthor: Failed to add author ");
                return new ApiResponse(400, "Failed to add author");
            }
           
        


        public async Task<ApiResponse> AddBookToAuthor(int authorId, int bookId)
        {

            var author = await _authorRepository.GetAuthorById(authorId);


            var book = await _bookRepository.GetBookById(bookId);
            if (author != null && book != null)
            {
                author.Books ??= new List<Book>();
                if (author.Books.Any(b => b.BookId == bookId))
                {
                    _logger.LogWarning("AddBookToAuthor: Book already exists in the author's data");
                    return new ApiResponse(400, "The book you're trying to add already exists");
                }

                author.Books.Add(book);
                await _authorRepository.EditAuthor(author);
                _logger.LogInformation("AddBookToAuthor: Book added to author's data");
                return new ApiResponse(200, "Added book to author");

            }
            _logger.LogError("AddBookToAuthor: Failed to add book to author");
            return new ApiResponse(400, "Failed to add book to author");
        }
    

        public async Task<ApiResponse> DeleteAuthor(int id)
        {
              var author = await _authorRepository.GetAuthorById(id);
                if (author != null)
                {
                _logger.LogInformation("DeleteAuthor: Author deleted successfully");
                    await _authorRepository.DeleteAuthor(author);
                    return new ApiResponse(200, "Deleted Author");
                }
               _logger.LogError("DeleteAuthor: Failed to delete author");
                return new ApiResponse(400, "Failed to delete author");
            
        }

        public async Task<ApiResponse> EditAuthor(EditAuthorDTO dto)
        {
           
                var author = await _authorRepository.GetAuthorById(dto.AuthorId);
                if (author != null)
                {
                if (author.Biography == dto.Biography &&
                    author.FullName == dto.FullName &&
                    author.Nationality == dto.Nationality &&
                    author.ProfileImage == dto.ProfileImage)
                {
                    _logger.LogWarning("EditAuthor: Author edit had no changes");
                    return new ApiResponse(400, "Nothing was edited");
                }

                if (string.IsNullOrEmpty(dto.FullName) && string.IsNullOrEmpty(dto.Nationality) && string.IsNullOrEmpty(dto.Biography))
                {
                    _logger.LogWarning("EditAuthor: Empty or null strings found in dto ");
                    return new ApiResponse(400, "Do not leave empty or null strings");
                }

                    author.Biography = _encryptionService.EncryptData(dto.Biography);
                    author.FullName = _encryptionService.EncryptData(dto.FullName);
                    author.Nationality = _encryptionService.EncryptData(dto.Nationality);
                    author.ProfileImage = _encryptionService.EncryptData(dto.ProfileImage);

                    await _authorRepository.EditAuthor(author);
                _logger.LogInformation("EditAuthor: Author edited successfully");
                    return new ApiResponse(200, "Edited author successfully");

                }
            _logger.LogError("EditAuthor: Author failed to be edited");
                return new ApiResponse(400, "Failed to edit");
            
        }

        public async Task<ResponseDTO> GetAllAuthors()
        {

            var response = new ResponseDTO();
            var author = await _authorRepository.GetAllAuthorList();   
            if(author.Count == 0)
            {
                _logger.LogWarning("GetAllAuthors: No authors found");
                response.Message = "No authors found";
                return response;
            }
            var authorList = _mapper.Map<List<GetAuthorDTO>>(author);
            foreach(GetAuthorDTO author1 in authorList)
            {
                author1.BioGraphy = _encryptionService.DecryptData(author1.BioGraphy);
                author1.FullName = _encryptionService.DecryptData(author1.FullName);
                author1.Nationality = _encryptionService.DecryptData(author1.Nationality);
                author1.ProfileImage = _encryptionService.DecryptData(author1.ProfileImage);


                foreach(GetOnlyBookDTO books in author1.Books)
                {
                    books.Title = _encryptionService.DecryptData(books.Title);
                    books.Description = _encryptionService.DecryptData(books.Description);
                    books.CoverImage = _encryptionService.DecryptData(books.CoverImage);
                }
            }

            _logger.LogInformation("GetAllAuthors: Authors retrieved successfully");
            response.Status = 200;
            response.Result = authorList;
            return response;
        }

        public async Task<ResponseDTO> GetAuthorById(int id)
        {
            var response = new ResponseDTO();
            if (id == 0)
            {
                _logger.LogWarning("GetAuthorById: Id provided is not valid");
                response.Message = "Id cannot be 0";
                return response;
            }

            var author = await _authorRepository.GetAuthorById(id);

            if(author == null)
            {
                _logger.LogWarning("GetAuthorById: Author with the id provided does not exist");
                response.Message = $"Author with the id {id} does not exist";
                return response;
            }

            var authorDTO = _mapper.Map<GetAuthorDTO>(author);
            authorDTO.BioGraphy = _encryptionService.DecryptData(author.Biography);
            authorDTO.FullName = _encryptionService.DecryptData(author.FullName);
            authorDTO.Nationality = _encryptionService.DecryptData(author.Nationality);
            authorDTO.ProfileImage = _encryptionService.DecryptData(author.ProfileImage);

            foreach(GetOnlyBookDTO books in authorDTO.Books)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);
            }
            _logger.LogInformation("GetAuthorById: Author retrieved successfully");
            response.Status = 200;
            response.Result = authorDTO;
            return response;
        }

        public async Task<ResponseDTO> GetAuthorByName(string name)
        {
            var response = new ResponseDTO();

            var encryptedName = _encryptionService.EncryptData(name);
            if (encryptedName == null)
            {
                _logger.LogWarning("GetAuthorByName: Name is null");
                response.Message = "Name cannot be null";
                return response;
            }
            var author = await _authorRepository.GetAuthorByName(encryptedName);
            if(author == null)
            {
                _logger.LogWarning("GetAuthorByName: Author with the provided name has not been found");
                response.Message = $"Author with the name {name} does not exist";
                return response;
            }

            var authorDTO = _mapper.Map<GetAuthorDTO>(author);
            authorDTO.BioGraphy = _encryptionService.DecryptData(author.Biography);
            authorDTO.FullName = _encryptionService.DecryptData(author.FullName);
            authorDTO.Nationality = _encryptionService.DecryptData(author.Nationality);
            authorDTO.ProfileImage = _encryptionService.DecryptData(author.ProfileImage);
            foreach (GetOnlyBookDTO books in authorDTO.Books)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);
            }
            _logger.LogInformation("GetAuthorByName: Author retrieved successfully");
            response.Status = 200;
            response.Result = authorDTO;
            return response;
        }

        public async Task<ResponseDTO> GetBooksByAuthorId(int authorId)
        {
            var response = new ResponseDTO();   
            if (authorId == 0)
            {
                _logger.LogWarning("GetBooksByAuthorId: Id provided cannot be 0");
                response.Message = "Id cannot be 0";
                return response;
            }
            var author = await _authorRepository.GetAuthorById(authorId);
            if(author == null)
            {
                _logger.LogWarning("GetBooksByAuthorId: Author with the id provided does not exist");
                response.Message = $"Author with the id {authorId} does not exist";
                return response;
            }

            var books = await _authorRepository.GetBooksByAuthorId(authorId);
            
            foreach (GetBookDTO book in books)
            {
                book.Title = _encryptionService.DecryptData(book.Title);
                book.Description = _encryptionService.DecryptData(book.Description);
                book.CoverImage = _encryptionService.DecryptData(book.CoverImage);

                foreach(GetOnlyAuthorDTO authors in book.Authors)
                {
                    authors.BioGraphy = _encryptionService.DecryptData(authors.BioGraphy);
                    authors.FullName = _encryptionService.DecryptData(authors.FullName);
                    authors.Nationality = _encryptionService.DecryptData(authors.Nationality);
                    authors.ProfileImage = _encryptionService.DecryptData(authors.ProfileImage);
                }
            }
            _logger.LogInformation("GetBooksByAuthorId: Author retrieved successfully");
            response.Status = 200;
            response.Result = books;
            return response;
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
                _logger.LogInformation("RemoveBookFromAuthor: Book has been removed from author successfully");
                return new ApiResponse(200, "Removed book from author");

            }
            _logger.LogError("RemoveBookFromAuthor: Book has failed to removed from author");
            return new ApiResponse(400, "Failed  to remove book from author");
        }
    }
}
