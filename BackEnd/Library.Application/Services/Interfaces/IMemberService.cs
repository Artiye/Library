using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
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

        Task<GetProfileDTO> GetMemberById(string memberId);

        Task<List<GetBookDTO>> GetAMembersReadList(string memberId);

        Task<List<GetAuthorDTO>> GetAMembersFavouriteAuthors(string memberId);
    }
}
