using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ProfileDTOs;
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
    public interface IProfileService
    {
        Task<ResponseDTO> GetMyProfile();

        Task<ApiResponse> EditProfile(EditProfileDTO dto);

        Task<ApiResponse> DeleteProfile(string password);

        Task<ApiResponse> ConfirmDelete(string userId);

        Task<ApiResponse> AddBookToReadList(int bookId);

        Task<ResponseDTO> GetMyReadList();

        Task<ApiResponse> AddAuthorToMyFavourites(int authorId);

        Task<ResponseDTO> GetMyFavouriteAuthors();

        Task<ApiResponse> RemoveAuthorFromMyFavourites(int authorId);

        Task<ApiResponse> RemoveBookFromReadList(int bookId);   
     }
}
