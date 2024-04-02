using AutoMapper;
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
    }
}
