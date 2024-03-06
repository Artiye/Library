using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Responses
{
    public class ApiResponse
    {
        public int Status { get; set; }
 
        public string Message { get; set; }

        public ApiResponse(int status, string message) {
            Status = status;
            Message = message;
        }
    }
}
