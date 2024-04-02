using AutoMapper;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ProfileDTOs;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public MemberService(UserManager<ApplicationUser> userManager, IMapper mapper )
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<List<GetProfileDTO>> GetAllMembers()
        {
            var userList = await _userManager.Users
                .Where(u => u.EmailConfirmed)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
            if(userList == null )
            {
                return new List<GetProfileDTO>();
            }
            var profilesDTO = _mapper.Map<List<GetProfileDTO>>(userList);
            return profilesDTO;
        }

        public async Task<List<GetBookDTO>> GetAMembersReadList(string memberId)
        {
            var user = await _userManager.Users
                .Include(u => u.Books)
                .FirstOrDefaultAsync(u => u.Id == memberId); 
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            if (user.Books == null || !user.Books.Any())
            {
                return new List<GetBookDTO>();
            }

            var readListDTO = _mapper.Map<List<GetBookDTO>>(user.Books);
            return readListDTO;
        }


        public async Task<GetProfileDTO> GetMemberById(string memberId)
        {
            var user = await _userManager.FindByIdAsync(memberId);

            if (user == null)
                throw new Exception("User does not exist");

            var userDTO = _mapper.Map<GetProfileDTO>(user);
            return userDTO;
        }

    }
}
