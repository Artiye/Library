using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.IdentityDTOs
{
    public class RoleChangeDTO
    {
        public string UserId { get; set; }

        public string NewRole {  get; set; }
    }
}
