using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookClubDTOs;
using Library.Application.DTOs.BookClubJoinRequestDTO;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.IdentityDTOs;
using Library.Application.DTOs.ProfileDTOs;
using Library.Application.DTOs.ResponseDTO;
using Library.Application.DTOs.UserDTOs;
using Library.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.MappingProfiles
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<Author, AddAuthorDTO>().ReverseMap();
            CreateMap<Author, EditAuthorDTO>().ReverseMap();
            CreateMap<Author, GetAuthorDTO>().ReverseMap(); 
            CreateMap<Author,GetOnlyAuthorDTO>().ReverseMap();

            CreateMap<Book, AddBookDTO>().ReverseMap();
            CreateMap<Book, EditBookDTO>().ReverseMap();
            CreateMap<Book, GetBookDTO>().ReverseMap();
            CreateMap<Book, GetOnlyBookDTO>().ReverseMap(); 

            CreateMap<BookClub, AddBookClubDTOs>().ReverseMap();
            CreateMap<BookClub, EditBookClubDTO>().ReverseMap();
            CreateMap<BookClub, GetBookClubDTO>().ReverseMap();

            CreateMap<BookClubJoinRequest,GetJoinRequestsDTO>().ReverseMap(); 
            
            CreateMap<RegisterDTO, ApplicationUser>().ReverseMap();
            CreateMap<GetUserDTO, ApplicationUser>().ReverseMap();

            CreateMap<GetProfileDTO, ApplicationUser>().ReverseMap();
            CreateMap<EditProfileDTO,  ApplicationUser>().ReverseMap();

            CreateMap<ResponseDTO, Book>().ReverseMap();
        }
    }
}
