using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Options
{
    public class EmailOptions
    {
        public const string EmailSender = "EmailSender";

        public string Host { get; set; } = String.Empty;
        public string FromMail { get; set; } = String.Empty;
        public string FromPassword { get; set; } = String.Empty;
        public int Port { get; set; }
    }
}
