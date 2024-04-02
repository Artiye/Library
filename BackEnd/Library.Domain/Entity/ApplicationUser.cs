using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Gender { get; set; }

        public string? Nationality { get; set; }

       
        public List<BookClub>? Clubs { get; set;}

        public List<Book>? Books { get; set;}

        public List<Author>? Authors { get; set; }


    }
}
