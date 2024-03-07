using Library.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.BookDTOs
{
    public class GetOnlyBookDTO
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public DateTime PublishingYear { get; set; }

        public string Description { get; set; }

        public int Pages { get; set; }

        public string CoverImage { get; set; }

        public double Rating { get; set; }

        public GenreTypes Genre { get; set; }

        public Languages Languages { get; set; }
    }
}
