using Library.Application.DTOs.ProfileDTOs;
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

        public async Task<IActionResult> DeleteMyProfile(string password)
        {
            var result = await _profileService.DeleteProfile(password);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        
        [HttpGet("confirm-delete")]
        public async Task<IActionResult> ConfirmDelete(string userId)
        {
            var result = await _profileService.ConfirmDelete(userId);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpPost("add-book-to-readlist")]

        public async Task<IActionResult> AddBookToReadList(int bookId)
        {
            var result = await _profileService.AddBookToReadList(bookId);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpPost("add-author-to-favourites")]

        public async Task<IActionResult> AddAuthorToMyFavourites(int authorId)
        {
            var result = await _profileService.AddAuthorToMyFavourites(authorId);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }

        [HttpGet("read-list")]

        public async Task<IActionResult> GetMyReadList()
        {
            var result = await _profileService.GetMyReadList();
            return Ok(result);
        }
        [HttpGet("author-list")]

        public async Task<IActionResult> GetMyFavouriteAuthors()
        {
            var result = await _profileService.GetMyFavouriteAuthors();
            return Ok(result);
        }

    }
}
