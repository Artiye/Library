using AutoMapper;
using Library.Application.DTOs.IdentityDTOs;
using Library.Application.Encryption;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IEmailSenderService _emailSenderService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContext;

        public IdentityService(IEncryptionService encryptionService,UserManager<ApplicationUser> userManager, IMapper mapper, IEmailSender emailSender, IEmailSenderService emailSenderService, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContext)
        {
            _encryptionService = encryptionService;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _emailSenderService = emailSenderService;
            _roleManager = roleManager;
            _httpContext = httpContext;
        }



        public async Task<ApiResponse> Register(RegisterDTO dto)
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                
                return new ApiResponse(400, "You are already registered and logged in.");
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
                return new ApiResponse(400, "This User Exists.");

            if (dto.Password != dto.ConfirmPassword)
                return new ApiResponse(400, "Password and Confirm Password do not Match.");

            var identityUser = _mapper.Map<ApplicationUser>(dto);
            identityUser.Email = _encryptionService.EncryptData(dto.Email);
            identityUser.FirstName = _encryptionService.EncryptData(dto.FirstName);
            identityUser.LastName = _encryptionService.EncryptData(dto.LastName);
            identityUser.Gender = _encryptionService.EncryptData(dto.Gender);
            identityUser.Nationality = _encryptionService.EncryptData(dto.Nationality);
            
            identityUser.UserName = dto.Email;
            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
                return new ApiResponse(400, "Something went Wrong.");

          
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            string url =  "https://localhost:7278/api/Identity/ConfirmEmail?email=" + identityUser.Email;
            string mailContent = "Confirm your account by  <a href=" + url + ">clicking here</a>";
            await _emailSenderService.SendRegistrationEmailAsync(dto.Email, identityUser.UserName, mailContent);



            var addingUser = await _userManager.AddToRoleAsync(identityUser, "User");
            if (addingUser.Succeeded)
                return new ApiResponse(200, "User was Registered. Please check your email to confirm!");

            return new ApiResponse(400, "Something went Wrong.");
        }
           public async Task<ApiResponse> EditRole(RoleChangeDTO dto)
          {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ApiResponse(400, "User not authenticated");

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return new ApiResponse(400, "User not found");

            if (!await _roleManager.RoleExistsAsync(dto.NewRole))
                return new ApiResponse(400, "Role doesnt exist");

           

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, dto.NewRole);
            if (!result.Succeeded)
                return new ApiResponse(400, "Did not add role successfully");

            return new ApiResponse(200, "Added role");
           }
      

    }
}



