using AutoMapper;
using Library.Application.DTOs.BookClubDTOs;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Library.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class BookClubService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IBookClubRepository bookClubRepository, IMapper mapper, IAuthorRepository authorRepository, IBookRepository bookRepository) : IBookClubService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IBookClubRepository _bookClubRepository = bookClubRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<ApiResponse> AddAuthorToBookClub(int bookClubId, int authorId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            var author = await _authorRepository.GetAuthorById(authorId);

            if (bookClub.OwnerId != userId)
                return new ApiResponse(400, "Only the owner can add an author to the bookclub");
            if (bookClub != null && author != null)
            {
                bookClub.Authors ??= new List<Author>();
                if (bookClub.Authors.Any(a => a.AuthorId == authorId))
                    return new ApiResponse(400, "Author already exists");

                bookClub.Authors.Add(author);
                await _bookClubRepository.EditBookClub(bookClub);
                return new ApiResponse(200, "Added author to bookclub");
            }
            return new ApiResponse(400, "Failed to add author to bookclub");
        }

        public async Task<ApiResponse> AddBookClub(AddBookClubDTOs dto)
        {
            if (dto != null)
            {
                if (string.IsNullOrEmpty(dto.Description) && string.IsNullOrEmpty(dto.Name))
                    return new ApiResponse(400, "Do not leave inputs empty");

                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                if (userId == null)
                    return new ApiResponse(400, "User not authenticated");

                    var bookClub = _mapper.Map<BookClub>(dto);
                    bookClub.OwnerId = userId;
                    bookClub.OwnerEmail = userEmail;
                    
                    await _bookClubRepository.AddBookClub(bookClub);
                    return new ApiResponse(200, "Added bookClub successfully");
                }

                          
            return new ApiResponse(400, "Failed to add book");
        }

        public async Task<ApiResponse> AddBookToBookClub(int bookClubId, int bookId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            var book = await _bookRepository.GetBookById(bookId);

            if (bookClub.OwnerId != userId)
                return new ApiResponse(400, "Only the owner can add a book to the bookclub");
            if (bookClub != null && book != null)
            {
                bookClub.Books ??= new List<Book>();

                if (bookClub.Books.Any(b => b.BookId == bookId))
                    return new ApiResponse(400, "Book already exists");

            

                bookClub.Books.Add(book);
                await _bookClubRepository.EditBookClub(bookClub);
                return new ApiResponse(200, "Added book to bookclub");
            }
            return new ApiResponse(400, "Failed to add book to bookclub");
        }

        public async Task<ApiResponse> DeleteBookClub(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)
               return new ApiResponse(400, "User not authenticated");

            var bookClub = await _bookClubRepository.GetBookClubById(id);


            if (bookClub != null)
            {
                if (bookClub.OwnerId != userId)
                    return new ApiResponse(400, "Only the owner can delete this bookclub");

                await _bookClubRepository.DeleteBookClub(bookClub);
                return new ApiResponse(200, "Deleted Book");
            }
            return new ApiResponse(400, "Failed to delete");

        }


        public async Task<ApiResponse> EditBookClub(EditBookClubDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return new ApiResponse(400, "User not authenticated");
            var bookClub = await _bookClubRepository.GetBookClubById(dto.BookClubId);

            if (bookClub != null)
            {
                if (bookClub.OwnerId != userId)
                    return new ApiResponse(400, "Only the owner can edit this bookclub");

                if (bookClub.Description == dto.Description && bookClub.Name == dto.Name)
                    return new ApiResponse(400, "Nothing has changed");

                if (string.IsNullOrEmpty(dto.Description) && string.IsNullOrEmpty(dto.Name))
                    return new ApiResponse(400, "Do not leave inputs blank");

                bookClub.Description = dto.Description;
                bookClub.Name = dto.Name;

                await _bookClubRepository.EditBookClub(bookClub);
                return new ApiResponse(200, "Edited successfully");
            }
            return new ApiResponse(400, "Failed to edit");
        }

        public async Task<GetBookClubDTO> GetBookClubById(int id)
        {
            if (id == 0)
            {
                throw new Exception("id cannot be 0");
            }
            var bookClub = await _bookClubRepository.GetBookClubById(id) ?? throw new Exception($"BookClub with id {id} does not exist");
            var bookClubDto = _mapper.Map<GetBookClubDTO>(bookClub);
            return bookClubDto;
        }



        public async Task<GetBookClubDTO> GetBookClubByName(string name)
        {
            if (name == null)
            {
                throw new Exception("Name cannot be null");
            }
            var bookClub = await _bookClubRepository.GetBookClubByName(name) ?? throw new Exception($"Bookclub with the name {name} does not exist");
            var bookClubDto = _mapper.Map<GetBookClubDTO>(bookClub);
            return bookClubDto;
        }

        public async Task<List<GetBookClubDTO>> GetBookClubByLanguage(string language)
        {
            if (language == null)
            {
                throw new Exception("language cannot be null");
            }
            var bookClub = await _bookClubRepository.GetBookClubByLanguage(language);
            if(bookClub.Count == 0)           
                throw new Exception($"Books with that language {language} do not exist");

            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            return bookClubList;    
        }

        public async Task<List<GetBookClubDTO>> GetBookClubByGenre(string genre)
        {
            if (genre == null)
            {
                throw new Exception("genre cannot be null");
            }
            var bookClub = await _bookClubRepository.GetBookClubByGenre(genre);
            if (bookClub.Count == 0)
                throw new Exception($"Books with that genre {genre} do not exist");

            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            return bookClubList;
        }


        public async Task<List<GetBookClubDTO>> GetBookClubs()
        {
            var bookClub = await _bookClubRepository.GetAllBookClubs();
            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            return bookClubList;
        }

        public async Task<ApiResponse> RemoveAuthorFromBookClub(int bookClubId, int authorId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");
            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            var author = await _authorRepository.GetAuthorById(authorId);

            if (bookClub.OwnerId != userId)
                return new ApiResponse(400, "Only the owner can remove this author from the bookclub");
            if (bookClub != null && author != null)
            {
                bookClub.Authors ??= new List<Author>();

                bookClub.Authors.Remove(author);
                await _bookClubRepository.EditBookClub(bookClub);
                return new ApiResponse(200, "Removed author from bookclub");
            }
            return new ApiResponse(400, "Failed to remove author from bookclub");
        }


        public async Task<ApiResponse> RemoveBookFromBookClub(int bookClubId, int bookId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");
            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            var book = await _bookRepository.GetBookById(bookId);

            if (bookClub.OwnerId != userId)
                return new ApiResponse(400, "Only the owner can remove this book from the bookclub");

            if (bookClub != null && book != null)
            {
                bookClub.Books ??= new List<Book>();
                bookClub.Books.Remove(book);
                await _bookClubRepository.EditBookClub(bookClub);
                return new ApiResponse(200, "Removed book from bookclub");
            }
            return new ApiResponse(400, "Failed to remove book from bookclub");
        }

        public async Task<ApiResponse> RequestToJoinBookClub(int bookClubId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            if (bookClub == null)
                return new ApiResponse(400, "Book Club not found");

            if (bookClub.Members.Any(m => m.Id == userId))
                return new ApiResponse(400, "You are already a member in this bookclub");

            var existingRequest = await _bookClubRepository.GetJoinRequestByBookClubAndUser(bookClubId, userId);
            if (existingRequest != null)
                return new ApiResponse(400, "You have already requested to join this book club");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse(400, "User not found");

            if (bookClub.OwnerId == userId)
                return new ApiResponse(400, "Owner cannot request to join their own club");

            var joinRequest = new BookClubJoinRequest
            {
                BookClubId = bookClubId,
                UserId = userId,               
                FirstName = user.FirstName,
                LastName = user.LastName,
                isAccepted = false
            };

            await _bookClubRepository.AddJoinRequest(joinRequest);
            return new ApiResponse(200, "Join request has been sent");
        }



        public async Task<ApiResponse> AcceptJoinRequest(int joinRequestId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var joinRequest = await _bookClubRepository.GetJoinRequestById(joinRequestId);
            if (joinRequest == null)
                return new ApiResponse(400, "Join request not found");

            var bookClub = await _bookClubRepository.GetBookClubById(joinRequest.BookClubId);
            if (bookClub == null)
                return new ApiResponse(400, "Book Club not found");

            if (bookClub.OwnerId != userId)
                return new ApiResponse(400, "Only the owner can accept join requests");

            joinRequest.isAccepted = true;
            await _bookClubRepository.RemoveJoinRequest(joinRequest);

            var user = await _userManager.FindByIdAsync(joinRequest.UserId);
            if (user == null)
                return new ApiResponse(400, "User not found");


            if (!bookClub.Members.Any(m => m.Id == joinRequest.UserId))
            {
                bookClub.Members.Add(user);
                await _bookClubRepository.EditBookClub(bookClub);
            }

            await _bookClubRepository.EditBookClub(bookClub);

            return new ApiResponse(200, "Join request has been accepted");
        }


        public async Task<ApiResponse> DenyJoinRequest(int joinRequestId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var joinRequest = await _bookClubRepository.GetJoinRequestById(joinRequestId);
            if (joinRequest == null)
                return new ApiResponse(400, "Join request not found");

            var bookClub = await _bookClubRepository.GetBookClubById(joinRequest.BookClubId);
            if (bookClub == null)
                return new ApiResponse(400, "Book Club not found");

            
            if (bookClub.OwnerId != userId)
                return new ApiResponse(400, "Only the owner can deny join requests");

            await _bookClubRepository.RemoveJoinRequest(joinRequest);
            return new ApiResponse(200, "Join request has been denied");
        }
    }
}

