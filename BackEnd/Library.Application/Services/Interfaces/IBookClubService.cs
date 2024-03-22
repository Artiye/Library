﻿using Library.Application.DTOs.BookClubDTOs;
using Library.Application.Responses;
using Library.Domain.Entity;
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

        Task<List<GetBookClubDTO>> GetBookClubByGenre(string genre);

        Task<List<GetBookClubDTO>> GetBookClubByLanguage(string language);

        Task<ApiResponse> AddBookToBookClub(int bookClubId, int bookId);

        Task<ApiResponse> AddAuthorToBookClub(int bookClubId, int authorId);

        Task<ApiResponse> RemoveBookFromBookClub(int bookClubId, int bookId);

        Task<ApiResponse> RemoveAuthorFromBookClub(int bookClubId, int authorId);

        Task<ApiResponse> RequestToJoinBookClub(int bookClubId);

        Task<ApiResponse> AcceptJoinRequest(int joinRequestId);

        Task<ApiResponse> DenyJoinRequest(int joinRequestId);

        Task<ApiResponse> RemoveMemberFromBookClub(int bookClubId, string memberId, string reason);

    }
}
