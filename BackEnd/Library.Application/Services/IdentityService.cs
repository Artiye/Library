using AutoMapper;
using Library.Application.DTOs.IdentityDTOs;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
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


        public IdentityService(UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;

        }



        public async Task<ApiResponse> Register(RegisterDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
                return new ApiResponse(400, "This User Exists.");

            if (dto.Password != dto.ConfirmPassword)
                return new ApiResponse(400, "Password and Confirm Password do not Match.");

            var identityUser = _mapper.Map<IdentityUser>(dto);

            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
                return new ApiResponse(400, "Something went Wrong.");

            var addingUser = await _userManager.AddToRoleAsync(identityUser, "User");
            if (addingUser.Succeeded)
                return new ApiResponse(200, "User was Registered. Please Login!");

            return new ApiResponse(400, "Something went Wrong.");
        }
    }
}




