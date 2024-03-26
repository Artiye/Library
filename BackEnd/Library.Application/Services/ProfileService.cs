using AutoMapper;
using Library.Application.DTOs.ProfileDTOs;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
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
    public class ProfileService : IProfileService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ProfileService(IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _httpContext = httpContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse> DeleteProfile()
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse(400, "User not found");

            await _userManager.DeleteAsync(user);
            return new ApiResponse(200, "Deleted your profile");
           
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
    }
}
