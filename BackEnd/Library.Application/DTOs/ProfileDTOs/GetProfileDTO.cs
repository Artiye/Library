using Library.Application.DTOs.BookClubDTOs;
using Library.Application.DTOs.BookDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.ProfileDTOs
{
    public class GetProfileDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Gender { get; set; }

        public string? Nationality { get; set; }

       
    }
}
