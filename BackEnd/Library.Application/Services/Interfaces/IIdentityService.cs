using Library.Application.DTOs.IdentityDTOs;
using Library.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<ApiResponse> Register(RegisterDTO dto);
    }
}
