using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.BookClubJoinRequestDTO
{
    public class GetJoinRequestsDTO
    {
        public int BookClubJoinRequestId { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Reason { get; set; }
    }
}
