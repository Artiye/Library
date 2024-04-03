﻿using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookClubDTOs;
using Library.Application.DTOs.BookClubJoinRequestDTO;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.UserDTOs;
using Library.Application.Encryption;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Library.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class BookClubService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IBookClubRepository bookClubRepository, IMapper mapper, IAuthorRepository authorRepository, IBookRepository bookRepository, IEmailSender emailSender, IEncryptionService encryptionService) : IBookClubService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IBookClubRepository _bookClubRepository = bookClubRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IEncryptionService _encryptionService = encryptionService;

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
                bookClub.Name = _encryptionService.EncryptData(dto.Name);
                bookClub.Description = _encryptionService.EncryptData(dto.Description);

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
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var bookClub = await _bookClubRepository.GetBookClubById(id);


            if (bookClub != null)
            {
                if (bookClub.OwnerId != userId)
                    return new ApiResponse(400, "Only the owner can delete this bookclub");

                await _bookClubRepository.DeleteBookClub(bookClub);
                return new ApiResponse(200, "Deleted BookClub");
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

                bookClub.Name = _encryptionService.EncryptData(dto.Name);
                bookClub.Description = _encryptionService.EncryptData(dto.Description);

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
            var bookClubDTO = _mapper.Map<GetBookClubDTO>(bookClub);
            bookClubDTO.Name = _encryptionService.DecryptData(bookClubDTO.Name);
            bookClubDTO.Description = _encryptionService.DecryptData(bookClubDTO.Description);

            return bookClubDTO;
        }



        public async Task<GetBookClubDTO> GetBookClubByName(string name)
        {
            var encryptedName = _encryptionService.EncryptData(name);

            if (encryptedName == null)
            {
                throw new Exception("Name cannot be null");
            }
            var bookClub = await _bookClubRepository.GetBookClubByName(encryptedName) ?? throw new Exception($"Bookclub with the name {encryptedName} does not exist");
            var bookClubDTO = _mapper.Map<GetBookClubDTO>(bookClub);
            bookClubDTO.Name = _encryptionService.DecryptData(bookClubDTO.Name);
            bookClubDTO.Description = _encryptionService.DecryptData(bookClubDTO.Description);
            return bookClubDTO;
        }

        public async Task<List<GetBookClubDTO>> GetBookClubByLanguage(string language)
        {
            if (language == null)
            {
                throw new Exception("language cannot be null");
            }
            var bookClub = await _bookClubRepository.GetBookClubByLanguage(language);
            if (bookClub.Count == 0)
                throw new Exception($"Books with that language {language} do not exist");

            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            foreach(GetBookClubDTO bookClubs in bookClubList)
            {
                bookClubs.Name = _encryptionService.DecryptData(bookClubs.Name);
                bookClubs.Description = _encryptionService.DecryptData(bookClubs.Description);
            }

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
            foreach (GetBookClubDTO bookClubs in bookClubList)
            {
                bookClubs.Name = _encryptionService.DecryptData(bookClubs.Name);
                bookClubs.Description = _encryptionService.DecryptData(bookClubs.Description);
            }
            return bookClubList;
        }


        public async Task<List<GetBookClubDTO>> GetBookClubs()
        {
            var bookClub = await _bookClubRepository.GetAllBookClubs();
            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            foreach(GetBookClubDTO bookClubs in bookClubList)
            {
                bookClubs.Name = _encryptionService.DecryptData(bookClubs.Name);
                bookClubs.Description = _encryptionService.DecryptData(bookClubs.Description);


                foreach(GetOnlyAuthorDTO bookClubsAuthors in bookClubs.Authors)
                {
                    bookClubsAuthors.FullName = _encryptionService.DecryptData(bookClubsAuthors.FullName);
                    bookClubsAuthors.Nationality = _encryptionService.DecryptData(bookClubsAuthors.Nationality);
                    bookClubsAuthors.BioGraphy = _encryptionService.DecryptData(bookClubsAuthors.BioGraphy);
                    bookClubsAuthors.ProfileImage = _encryptionService.DecryptData(bookClubsAuthors.ProfileImage);
                }

                foreach(GetOnlyBookDTO bookClubsBooks in bookClubs.Books)
                {
                    bookClubsBooks.Title = _encryptionService.DecryptData(bookClubsBooks.Title);
                    bookClubsBooks.Description = _encryptionService.DecryptData(bookClubsBooks.Description);
                    bookClubsBooks.CoverImage = _encryptionService.DecryptData(bookClubsBooks.CoverImage);
                }
               
               foreach(GetUserDTO bookClubsUsers in bookClubs.Members)
                {
                    bookClubsUsers.FirstName = _encryptionService.DecryptData(bookClubsUsers.FirstName);
                    bookClubsUsers.LastName = _encryptionService.DecryptData(bookClubsUsers.LastName);
                }

                
            }
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

        public async Task<ApiResponse> RequestToJoinBookClub(int bookClubId, string reason)
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
                Reason = reason,
                IsAccepted = false
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

            joinRequest.IsAccepted = true;
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


        public async Task<ApiResponse> DenyJoinRequest(int joinRequestId, string reason)
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

            var user = await _userManager.FindByIdAsync(joinRequest.UserId);
            if (user == null)
                return new ApiResponse(400, "User not found");

            var userEmail = await _userManager.GetEmailAsync(user);
            if (string.IsNullOrEmpty(userEmail))
                return new ApiResponse(400, "User email not found");

            var emailSubject = "Join Request Denied";
            var emailBody = $"Dear {user.FirstName} {user.LastName}, \n \n" +
                $"Your join request for bookclub '{bookClub.Name}' has been denied for the following reason: \n \n " +
                $"Reason: {reason} \n \n";

            await _emailSender.SendEmailAsync(userEmail, emailSubject, emailBody);


            await _bookClubRepository.RemoveJoinRequest(joinRequest);
            return new ApiResponse(200, "Join request has been denied");
        }
        public async Task<ApiResponse> RemoveMemberFromBookClub(int bookClubId, string memberId, string reason)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            if (bookClub == null)
                return new ApiResponse(400, "Bookclub not found");

            if (bookClub.OwnerId != userId)
                return new ApiResponse(400, "Only the owner can kick a member from the bookclub");

            var memberToRemove = await _userManager.FindByIdAsync(memberId);
            if (memberToRemove == null)
                return new ApiResponse(400, "Member does not exist");

            if (!bookClub.Members.Any(m => m.Id == memberToRemove.Id))
                return new ApiResponse(400, "The user specified is not a part of this bookclub");

            var memberEmail = await _userManager.GetEmailAsync(memberToRemove);
            if (!string.IsNullOrEmpty(memberEmail))
            {
                var emailSubject = "You have been removed from the book club";
                var emailBody = $"Dear {memberToRemove.FirstName}, \n \n  " +
                    $"You have been removed from the bookclub {bookClub.Name} for the following reason: \n \n " +
                    $"{reason}\n \n";

                await _emailSender.SendEmailAsync(memberEmail, emailSubject, emailBody);
            }

            bookClub.Members.Remove(memberToRemove);
            await _bookClubRepository.EditBookClub(bookClub);
            return new ApiResponse(200, "Member kicked successfully");
        }

        public async Task<List<GetJoinRequestsDTO>> GetJoinRequestsForOwner(int bookClubId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new Exception("You are not authorized");

            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            if (bookClub == null)
                throw new Exception("BookClub not found");

            if (bookClub.OwnerId != userId)
                throw new Exception("Only the owner can see the join requests");

            var joinRequests = await _bookClubRepository.GetJoinRequestsForBookClub(bookClubId);
            var joinRequestDTO = _mapper.Map<List<GetJoinRequestsDTO>>(joinRequests);

            return joinRequestDTO;
        }
    }
}


