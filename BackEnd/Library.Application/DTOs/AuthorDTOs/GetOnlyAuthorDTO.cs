using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.AuthorDTOs
{
    public class GetOnlyAuthorDTO
    {
        public int AuthorId { get; set; }

        public string FullName { get; set; }

        public string Nationality { get; set; }

        public string BioGraphy { get; set; }

        public string ProfileImage { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }
    }
}
