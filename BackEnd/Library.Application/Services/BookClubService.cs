﻿using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookClubDTOs;
using Library.Application.DTOs.BookClubJoinRequestDTO;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ResponseDTO;
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

        public async Task<ResponseDTO> GetBookClubById(int id)
        {
            var response = new ResponseDTO();
            if (id == 0)
            {
                response.Message = "Id cannot be 0";
                return response;
            }

            var bookClub = await _bookClubRepository.GetBookClubById(id);
                if(bookClub == null)
            {
                response.Message = $"BookClub with id {id} does not exist";
                return response;
            }
            var bookClubDTO = _mapper.Map<GetBookClubDTO>(bookClub);
            bookClubDTO.Name = _encryptionService.DecryptData(bookClubDTO.Name);
            bookClubDTO.Description = _encryptionService.DecryptData(bookClubDTO.Description);
            bookClubDTO.OwnerEmail = _encryptionService.DecryptData(bookClub.OwnerEmail);
            
            foreach (GetOnlyAuthorDTO bookClubsAuthors in bookClubDTO.Authors)
            {
                bookClubsAuthors.FullName = _encryptionService.DecryptData(bookClubsAuthors.FullName);
                bookClubsAuthors.Nationality = _encryptionService.DecryptData(bookClubsAuthors.Nationality);
                bookClubsAuthors.BioGraphy = _encryptionService.DecryptData(bookClubsAuthors.BioGraphy);
                bookClubsAuthors.ProfileImage = _encryptionService.DecryptData(bookClubsAuthors.ProfileImage);
            }

            foreach (GetOnlyBookDTO bookClubsBooks in bookClubDTO.Books)
            {
                bookClubsBooks.Title = _encryptionService.DecryptData(bookClubsBooks.Title);
                bookClubsBooks.Description = _encryptionService.DecryptData(bookClubsBooks.Description);
                bookClubsBooks.CoverImage = _encryptionService.DecryptData(bookClubsBooks.CoverImage);
            }

            foreach (GetUserDTO bookClubsUsers in bookClubDTO.Members)
            {
                bookClubsUsers.FirstName = _encryptionService.DecryptData(bookClubsUsers.FirstName);
                bookClubsUsers.LastName = _encryptionService.DecryptData(bookClubsUsers.LastName);
            }
            response.Status = 200;
            response.Result = bookClubDTO;
            return response;
        }



        public async Task<ResponseDTO> GetBookClubByName(string name)
        {
            var response = new ResponseDTO();
            var encryptedName = _encryptionService.EncryptData(name);

            if (encryptedName == null)
            {
                response.Message = "Name cannot be null";
                return response;
            }
            var bookClub = await _bookClubRepository.GetBookClubByName(encryptedName);
                if(bookClub == null)
            {
                response.Message = $"Bookclub with the name {name} does not exist";
                return response;
            }
            var bookClubDTO = _mapper.Map<GetBookClubDTO>(bookClub);
            bookClubDTO.Name = _encryptionService.DecryptData(bookClubDTO.Name);
            bookClubDTO.Description = _encryptionService.DecryptData(bookClubDTO.Description);
            bookClubDTO.OwnerEmail = _encryptionService.DecryptData(bookClub.OwnerEmail);
            foreach (GetOnlyAuthorDTO bookClubsAuthors in bookClubDTO.Authors)
            {
                bookClubsAuthors.FullName = _encryptionService.DecryptData(bookClubsAuthors.FullName);
                bookClubsAuthors.Nationality = _encryptionService.DecryptData(bookClubsAuthors.Nationality);
                bookClubsAuthors.BioGraphy = _encryptionService.DecryptData(bookClubsAuthors.BioGraphy);
                bookClubsAuthors.ProfileImage = _encryptionService.DecryptData(bookClubsAuthors.ProfileImage);
            }

            foreach (GetOnlyBookDTO bookClubsBooks in bookClubDTO.Books)
            {
                bookClubsBooks.Title = _encryptionService.DecryptData(bookClubsBooks.Title);
                bookClubsBooks.Description = _encryptionService.DecryptData(bookClubsBooks.Description);
                bookClubsBooks.CoverImage = _encryptionService.DecryptData(bookClubsBooks.CoverImage);
            }

            foreach (GetUserDTO bookClubsUsers in bookClubDTO.Members)
            {
                bookClubsUsers.FirstName = _encryptionService.DecryptData(bookClubsUsers.FirstName);
                bookClubsUsers.LastName = _encryptionService.DecryptData(bookClubsUsers.LastName);
            }
            response.Status = 200;
            response.Result = bookClubDTO;
            return response;
        }

        public async Task<ResponseDTO> GetBookClubByLanguage(string language)
        {
            var response = new ResponseDTO();
            if (language == null)
            {
                response.Message = "Language cannot be null";
                return response;
            }
            var bookClub = await _bookClubRepository.GetBookClubByLanguage(language);
            if (bookClub.Count == 0)
            {
                response.Message = $"Books with the language {language} do not exist";
                return response;
            }
                

            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            foreach (GetBookClubDTO bookClubs in bookClubList)
            {
                bookClubs.Name = _encryptionService.DecryptData(bookClubs.Name);
                bookClubs.Description = _encryptionService.DecryptData(bookClubs.Description);
                bookClubs.OwnerEmail = _encryptionService.DecryptData(bookClubs.OwnerEmail);



                foreach (GetOnlyAuthorDTO bookClubsAuthors in bookClubs.Authors)
                {
                    bookClubsAuthors.FullName = _encryptionService.DecryptData(bookClubsAuthors.FullName);
                    bookClubsAuthors.Nationality = _encryptionService.DecryptData(bookClubsAuthors.Nationality);
                    bookClubsAuthors.BioGraphy = _encryptionService.DecryptData(bookClubsAuthors.BioGraphy);
                    bookClubsAuthors.ProfileImage = _encryptionService.DecryptData(bookClubsAuthors.ProfileImage);
                }

                foreach (GetOnlyBookDTO bookClubsBooks in bookClubs.Books)
                {
                    bookClubsBooks.Title = _encryptionService.DecryptData(bookClubsBooks.Title);
                    bookClubsBooks.Description = _encryptionService.DecryptData(bookClubsBooks.Description);
                    bookClubsBooks.CoverImage = _encryptionService.DecryptData(bookClubsBooks.CoverImage);
                }

                foreach (GetUserDTO bookClubsUsers in bookClubs.Members)
                {
                    bookClubsUsers.FirstName = _encryptionService.DecryptData(bookClubsUsers.FirstName);
                    bookClubsUsers.LastName = _encryptionService.DecryptData(bookClubsUsers.LastName);
                }


            }
            response.Status = 200;
            response.Result = bookClubList;
            return response;
        }

        public async Task<ResponseDTO> GetBookClubByGenre(string genre)
        {
            var response = new ResponseDTO();
            if (genre == null)
            {
                response.Message = "Genre cannot be null";
                return response;
            }
            var bookClub = await _bookClubRepository.GetBookClubByGenre(genre);
            if (bookClub.Count == 0)
            {
                response.Message = $"Books with the genre {genre} do not exist";
                return response;
            }
            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            foreach (GetBookClubDTO bookClubs in bookClubList)
            {
                bookClubs.Name = _encryptionService.DecryptData(bookClubs.Name);
                bookClubs.Description = _encryptionService.DecryptData(bookClubs.Description);
                bookClubs.OwnerEmail = _encryptionService.DecryptData(bookClubs.OwnerEmail);



                foreach (GetOnlyAuthorDTO bookClubsAuthors in bookClubs.Authors)
                {
                    bookClubsAuthors.FullName = _encryptionService.DecryptData(bookClubsAuthors.FullName);
                    bookClubsAuthors.Nationality = _encryptionService.DecryptData(bookClubsAuthors.Nationality);
                    bookClubsAuthors.BioGraphy = _encryptionService.DecryptData(bookClubsAuthors.BioGraphy);
                    bookClubsAuthors.ProfileImage = _encryptionService.DecryptData(bookClubsAuthors.ProfileImage);
                }

                foreach (GetOnlyBookDTO bookClubsBooks in bookClubs.Books)
                {
                    bookClubsBooks.Title = _encryptionService.DecryptData(bookClubsBooks.Title);
                    bookClubsBooks.Description = _encryptionService.DecryptData(bookClubsBooks.Description);
                    bookClubsBooks.CoverImage = _encryptionService.DecryptData(bookClubsBooks.CoverImage);
                }

                foreach (GetUserDTO bookClubsUsers in bookClubs.Members)
                {
                    bookClubsUsers.FirstName = _encryptionService.DecryptData(bookClubsUsers.FirstName);
                    bookClubsUsers.LastName = _encryptionService.DecryptData(bookClubsUsers.LastName);
                }


            }
            response.Status = 200;
            response.Result = bookClubList;
            return response;
        }


        public async Task<ResponseDTO> GetBookClubs()
        {
            var response = new ResponseDTO();
            var bookClub = await _bookClubRepository.GetAllBookClubs();
            if(bookClub.Count == 0 )
            {
                response.Message = "No bookclubs found";
                return response;
            }
            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            foreach(GetBookClubDTO bookClubs in bookClubList)
            {
                bookClubs.Name = _encryptionService.DecryptData(bookClubs.Name);
                bookClubs.Description = _encryptionService.DecryptData(bookClubs.Description);
                bookClubs.OwnerEmail = _encryptionService.DecryptData(bookClubs.OwnerEmail);
                


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
            response.Status = 200;
            response.Result = bookClubList;
            return response;
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
            reason = _encryptionService.EncryptData(reason);

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

            var userEmail = _encryptionService.DecryptData(user.Email);
            var userFirstName = _encryptionService.DecryptData(user.FirstName);
            var userLastName = _encryptionService.DecryptData(user.LastName);
            var bookClubName = _encryptionService.DecryptData(bookClub.Name);
            if (string.IsNullOrEmpty(userEmail))
                return new ApiResponse(400, "User email not found");

            var emailSubject = "Join Request Denied";
            var emailBody = $"Dear {userFirstName} {userLastName}, \n \n" +
                $"Your join request for bookclub '{bookClubName}' has been denied for the following reason: \n \n " +
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

            var memberEmail = _encryptionService.DecryptData(memberToRemove.Email);
            var memberFirstName = _encryptionService.DecryptData(memberToRemove.FirstName);
            var memberLastName = _encryptionService.DecryptData(memberToRemove.LastName);
            var bookClubName = _encryptionService.DecryptData(bookClub.Name);
            if (!string.IsNullOrEmpty(memberEmail))
            {
                var emailSubject = "You have been removed from the book club";
                var emailBody = $"Dear {memberFirstName} {memberLastName}, \n \n  " +
                    $"You have been removed from the bookclub '{bookClubName}' for the following reason: \n \n " +
                    $"{reason}\n \n";

                await _emailSender.SendEmailAsync(memberEmail, emailSubject, emailBody);
            }

            bookClub.Members.Remove(memberToRemove);
            await _bookClubRepository.EditBookClub(bookClub);
            return new ApiResponse(200, "Member kicked successfully");
        }

        public async Task<ResponseDTO> GetJoinRequestsForOwner(int bookClubId)
        {
            var response = new ResponseDTO();

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                response.Message = "Userid not authenticated";
                return response;
            }
              

            var bookClub = await _bookClubRepository.GetBookClubById(bookClubId);
            if (bookClub == null)
            {
                response.Message = "Bookclub not found";
                return response;
            }
               

            if (bookClub.OwnerId != userId)
            {
                response.Message = "Only the owner of this bookclub can see the join requests for this bookclub";
                return response;
            }
                

            var joinRequests = await _bookClubRepository.GetJoinRequestsForBookClub(bookClubId);
            var joinRequestDTO = _mapper.Map<List<GetJoinRequestsDTO>>(joinRequests);
            foreach(GetJoinRequestsDTO joinRequest in joinRequestDTO)
            {
                joinRequest.FirstName = _encryptionService.DecryptData(joinRequest.FirstName);
                joinRequest.LastName = _encryptionService.DecryptData(joinRequest.LastName);
                joinRequest.Reason = _encryptionService.DecryptData(joinRequest.Reason);
            }


            response.Status = 200;
            response.Result = joinRequestDTO;
            return response;
        }
    }
}


