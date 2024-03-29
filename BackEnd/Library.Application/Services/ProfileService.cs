using AutoMapper;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ProfileDTOs;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IBookRepository _bookRepository;

        public ProfileService(IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailSender emailSender, IBookRepository bookRepository)
        {
            _httpContext = httpContext;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _bookRepository = bookRepository;
        }

        public async Task<ApiResponse> DeleteProfile(string password)
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse(400, "User not found");

            var validPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!validPassword)
                return new ApiResponse(400, "Password is not valid");

            var confirmationLink = $"https://localhost:7278/api/Profile/confirm-delete?userId={user.Id}";
            await _emailSender.SendEmailAsync(user.Email, "Confirm account deletion",
                $"To confirm the deletion of your account, click <a href='{confirmationLink}'>here</a>.");

           
            return new ApiResponse(200, "Confirmation email sent for deleting your profile");
        }

        public async Task<ApiResponse> ConfirmDelete(string userId)
        {
            var check = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (check == null)
                return new ApiResponse(400, "User not authenticated");

            if (check != userId)
                return new ApiResponse(400, "You cannot delete another users account");

            if (userId == null)
                return new ApiResponse(400, "Invalid user ID");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse(400, "User not found");
                 
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return new ApiResponse(500, "Failed to delete the user");

            return new ApiResponse(200, "Account deleted successfully");
        }
    


        public async Task<ApiResponse> EditProfile(EditProfileDTO dto)
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            if(dto != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(string.IsNullOrEmpty(dto.FirstName) || string.IsNullOrEmpty(dto.LastName))
                {
                   return new ApiResponse(400, "You cannot have an empty first or last name");
                }
                if(dto.FirstName == user.FirstName && dto.LastName == user.LastName && dto.Nationality == user.Nationality && dto.Gender == user.Gender) {
                    return new ApiResponse(400, "No changes have been made");
                }
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Nationality = dto.Nationality;
                user.Gender = dto.Gender;

                await _userManager.UpdateAsync(user);
                return new ApiResponse(200, "Profile edited successfully");
            }
            return new ApiResponse(400, "Failed to edit");
        }

        public async Task<GetProfileDTO> GetMyProfile()
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new Exception("User not authorized");

            var user = await _userManager.FindByIdAsync(userId);
            var userDTO = _mapper.Map<GetProfileDTO>(user);

            return userDTO;
        }

        public async Task<ApiResponse> AddBookToReadList(int bookId)
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse(400, "User not found");

            
            var bookToAdd = await _bookRepository.GetBookById(bookId); 
            if (bookToAdd == null)
                return new ApiResponse(400, "Book not found");

           
            await _userManager.Users.Include(u => u.Books).FirstOrDefaultAsync(u => u.Id == userId);

            
            if (!user.Books.Any(b => b.BookId == bookToAdd.BookId))
            {
                
                user.Books.Add(bookToAdd);
            }
            else
            {
                return new ApiResponse(400, "Book already exists in the user list");
            }

           
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiResponse(200, "Book added to read list successfully");
            }
            else
            {
                return new ApiResponse(400, "Failed to add book to read list");
            }
        }


        public async Task<List<GetBookDTO>> GetMyReadList()
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new Exception("User not authorized");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            
            await _userManager.Users.Include(u => u.Books).FirstOrDefaultAsync(u => u.Id == userId);

            var booksInReadList = user.Books;

            if (booksInReadList == null || booksInReadList.Count == 0)
                throw new Exception("No books added");

            var booksDTO = _mapper.Map<List<GetBookDTO>>(booksInReadList);

            return booksDTO;
        }

    }
}
