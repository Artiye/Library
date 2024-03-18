using AutoMapper;
using Library.Application.DTOs.IdentityDTOs;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IEmailSenderService _emailSenderService;

        public IdentityService(UserManager<IdentityUser> userManager, IMapper mapper, IEmailSender emailSender, IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _emailSenderService = emailSenderService;
        }



        public async Task<ApiResponse> Register(RegisterDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
                return new ApiResponse(400, "This User Exists.");

            if (dto.Password != dto.ConfirmPassword)
                return new ApiResponse(400, "Password and Confirm Password do not Match.");

            var identityUser = _mapper.Map<IdentityUser>(dto);
            identityUser.UserName = dto.Email;
            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
                return new ApiResponse(400, "Something went Wrong.");

            // Send email confirmation
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            string url =  "https://localhost:7278/api/Identity/ConfirmEmail?email=" + identityUser.Email;
            string mailContent = "Confirm your account by  <a href=" + url + ">clicking here</a>";
            await _emailSenderService.SendRegistrationEmailAsync(dto.Email, identityUser.UserName, mailContent);


            // Send email using EmailSender
            await _emailSender.SendEmailAsync(identityUser.Email, "Confirm your email", mailContent);

            var addingUser = await _userManager.AddToRoleAsync(identityUser, "User");
            if (addingUser.Succeeded)
                return new ApiResponse(200, "User was Registered. Please check your email to confirm!");

            return new ApiResponse(400, "Something went Wrong.");
        }

    }
}



