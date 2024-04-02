using Library.Application.DTOs.ProfileDTOs;
using Library.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services.Interfaces
{
    public interface IMemberService
    {
        Task<List<GetProfileDTO>> GetAllMembers();
    }
}
