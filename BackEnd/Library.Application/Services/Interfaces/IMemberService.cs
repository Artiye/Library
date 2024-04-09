using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ProfileDTOs;
using Library.Application.DTOs.ResponseDTO;
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
        Task<ResponseDTO> GetAllMembers();

        Task<ResponseDTO> GetMemberById(string memberId);

        Task<ResponseDTO> GetAMembersReadList(string memberId);

        Task<ResponseDTO> GetAMembersFavouriteAuthors(string memberId);
    }
}
