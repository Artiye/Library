using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.ProfileDTOs;
using Library.Application.Encryption;
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
        private readonly IEncryptionService _encryptionService;

        public MemberService(UserManager<ApplicationUser> userManager, IMapper mapper, IEncryptionService encryptionService )
        {
            _userManager = userManager;
            _mapper = mapper;
            _encryptionService = encryptionService;
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
            foreach(GetProfileDTO profiles in profilesDTO)
            {
                profiles.FirstName = _encryptionService.DecryptData(profiles.FirstName);
                profiles.LastName = _encryptionService.DecryptData(profiles.LastName);
                profiles.Gender = _encryptionService.DecryptData(profiles.Gender);
                profiles.Nationality = _encryptionService.DecryptData(profiles.Nationality);
            }
            return profilesDTO;
        }

        public async Task<GetProfileDTO> GetMemberById(string memberId)
        {
            var user = await _userManager.FindByIdAsync(memberId);

            if (user == null)
                throw new Exception("User does not exist");

            var userDTO = _mapper.Map<GetProfileDTO>(user);
            userDTO.FirstName = _encryptionService.DecryptData(userDTO.FirstName);
            userDTO.LastName = _encryptionService.DecryptData(userDTO.LastName);
            userDTO.Gender = _encryptionService.DecryptData(userDTO.Gender);
            userDTO.Nationality = _encryptionService.DecryptData(userDTO.Nationality);
            return userDTO;
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
                return new List<GetBookDTO>();
            

            var readListDTO = _mapper.Map<List<GetBookDTO>>(user.Books);
            foreach(GetBookDTO books in readListDTO)
            {
                books.Title = _encryptionService.DecryptData(books.Title);
                books.Description = _encryptionService.DecryptData(books.Description);
                books.CoverImage = _encryptionService.DecryptData(books.CoverImage);
            }
            return readListDTO;
        }
        
        public async Task<List<GetAuthorDTO>> GetAMembersFavouriteAuthors(string memberId)
        {
            var user = await _userManager.Users
                .Include(u => u.Authors)
                .FirstOrDefaultAsync (u => u.Id == memberId);
            if (user == null)
                throw new Exception("User not found");

            if (user.Authors == null || !user.Authors.Any())
                return new List<GetAuthorDTO>();

            var authorListDTO = _mapper.Map<List<GetAuthorDTO>>(user.Authors);
            foreach(GetAuthorDTO authors in  authorListDTO)
            {
                authors.FullName = _encryptionService.DecryptData(authors.FullName);
                authors.Nationality = _encryptionService.DecryptData(authors.Nationality);
                authors.BioGraphy = _encryptionService.DecryptData(authors.BioGraphy);
                authors.ProfileImage = _encryptionService.DecryptData(authors.ProfileImage);
            }
            return authorListDTO;
        }
      

    }
}
