﻿using Library.Application.DTOs.BookClubDTOs;
using Library.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services.Interfaces
{
    public interface IBookClubService
    {
        Task<ApiResponse> AddBookClub(AddBookClubDTOs dto);

        Task<ApiResponse> EditBookClub(EditBookClubDTO dto);

        Task<ApiResponse> DeleteBookClub(int id);

        Task<GetBookClubDTO> GetBookClubById(int id);

        Task<GetBookClubDTO> GetBookClubByName(string name);

        Task<List<GetBookClubDTO>> GetBookClubs();
    }
}
