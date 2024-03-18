using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendRegistrationEmailAsync(string email, string fullName, string mailContent);
    }
}
