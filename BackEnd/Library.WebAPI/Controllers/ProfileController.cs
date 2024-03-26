﻿using Library.Application.DTOs.ProfileDTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _profileService.GetMyProfile();
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> EditMyProfile([FromBody] EditProfileDTO dto)
        {
            var result = await _profileService.EditProfile(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpDelete]
       
        public async Task<IActionResult> DeleteMyProfile()
        {
            var result = await _profileService.DeleteProfile();
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
    }
}