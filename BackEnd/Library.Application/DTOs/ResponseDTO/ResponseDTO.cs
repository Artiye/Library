using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.DTOs.ResponseDTO
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public int Status { get; set; } = 400;
        public string Message { get; set; } = "";
    }
}
