﻿using Library.Application.DTOs.BookClubDTOs;
using Library.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookClubController : ControllerBase
    {
        private readonly IBookClubService _bookClubService;

        public BookClubController(IBookClubService bookClubService)
        {
           _bookClubService = bookClubService;
        }
        [HttpPost]

        public async Task<IActionResult> AddBookClub([FromBody] AddBookClubDTOs dto)
        {
            var result = await _bookClubService.AddBookClub(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpPut]

        public async Task<IActionResult> EditBookClub([FromBody] EditBookClubDTO dto)
        {
            var result = await _bookClubService.EditBookClub(dto);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpDelete]

        public async Task<IActionResult> DeleteBookClub(int id)
        {
            var result = await _bookClubService.DeleteBookClub(id);
            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{bookClubId}")]

        public async Task<IActionResult> GetBookClubById(int bookClubId)
        {
            var result = await _bookClubService.GetBookClubById(bookClubId);
            return Ok(result);
        }
        [HttpGet("byname/{bookClubName}")]

        public async Task<IActionResult> GetBookClubByName(string bookClubName)
        {
            var result = await _bookClubService.GetBookClubByName(bookClubName);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBookClubs()
        {
            var result = await _bookClubService.GetBookClubs();
            return Ok(result);
        }
    }
}