using Library.Application.DTOs.BookClubDTOs;
using Library.Application.DTOs.BookClubJoinRequestDTO;
using Library.Application.DTOs.ResponseDTO;
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

        Task<ResponseDTO> GetBookClubById(int id);

        Task<ResponseDTO> GetBookClubByName(string name);

        Task<ResponseDTO> GetBookClubs();

        Task<ResponseDTO> GetBookClubByGenre(string genre);

        Task<ResponseDTO> GetBookClubByLanguage(string language);

        Task<ResponseDTO> GetJoinRequestsForOwner(int bookClubId);

        Task<ApiResponse> AddBookToBookClub(int bookClubId, int bookId);

        Task<ApiResponse> AddAuthorToBookClub(int bookClubId, int authorId);

        Task<ApiResponse> RemoveBookFromBookClub(int bookClubId, int bookId);

        Task<ApiResponse> RemoveAuthorFromBookClub(int bookClubId, int authorId);

        Task<ApiResponse> RequestToJoinBookClub(int bookClubId, string reason);

        Task<ApiResponse> AcceptJoinRequest(int joinRequestId);

        Task<ApiResponse> DenyJoinRequest(int joinRequestId, string reason);

        Task<ApiResponse> RemoveMemberFromBookClub(int bookClubId, string memberId, string reason);

        

    }
}
