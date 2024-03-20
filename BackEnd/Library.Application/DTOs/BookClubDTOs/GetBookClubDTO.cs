using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.DTOs.UserDTOs;
using Library.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.BookClubDTOs
{
    public class GetBookClubDTO
    {
        public int BookClubId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public GenreTypes Genre { get; set; }

        public Languages Languages { get; set; }

        public List<GetOnlyAuthorDTO> Authors { get; set; }

        public List<GetOnlyBookDTO> Books { get; set; }

        public List<GetUserDTO> Members { get; set; }

        public string OwnerId { get; set; }

        public string OwnerEmail { get; set; }
    }
}
