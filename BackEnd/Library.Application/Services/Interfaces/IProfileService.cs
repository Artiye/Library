using Library.Application.DTOs.ProfileDTOs;
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
        Task<GetProfileDTO> GetMyProfile();

        Task<ApiResponse> EditProfile(EditProfileDTO dto);

        Task<ApiResponse> DeleteProfile();
    }
}
