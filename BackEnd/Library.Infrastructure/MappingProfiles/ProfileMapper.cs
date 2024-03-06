using AutoMapper;
using Library.Application.DTOs;
using Library.Domain.Entity;
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
        }
    }
}
