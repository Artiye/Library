using Library.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class BookClub
    {
        public int BookClubId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public GenreTypes Genre { get; set; }

        public Languages Languages { get; set; }

        public List<Author> Authors { get; set; }

        public List<Book>  Books { get; set; }

        public List<BookClubJoinRequest> BookClubJoinRequests { get; set; }

        public List<IdentityUser> Members { get; set; }

        public string OwnerId { get; set; }

        public string OwnerEmail { get; set; }

        


    }
}
