using Library.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class Book
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public DateTime PublishingYear { get; set; }

        public string Description { get; set; }

        public int Pages { get; set; }

        public string CoverImage { get; set; }

        public double Rating { get; set; }
        
        public GenreTypes  Genre { get; set; }

        public Languages Languages { get; set; }

        public List<Author> Authors { get; set; }

        public List<BookClub> Clubs { get; set; }

    }
}
