using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class BookClubJoinRequest
    {
        public int BookClubJoinRequestId { get; set; }

        public int BookClubId { get; set; }

        public BookClub BookClub { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public bool isAccepted { get; set; }
    }
}
