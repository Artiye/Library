using Library.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.BookClubDTOs
{
    public class EditBookClubDTO
    {
        public int BookClubId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public GenreTypes Genre { get; set; }

        public Languages Languages { get; set; }
    }
}
