using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Library.Application.Options;
using Library.Application.Services.Interfaces;

namespace Library.Application.Services
{
    public class EmailSenderService : IEmailSender, IEmailSenderService
    {
        private readonly EmailOptions _smtpSettings;
        public EmailSenderService(IOptions<EmailOptions> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toMail, string subject, string htmlMessage)
        {
            MailMessage message = new();
            message.From = new MailAddress(_smtpSettings.FromMail);
            message.To.Add(new MailAddress(toMail));
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient(_smtpSettings.Host))
            {
                smtpClient.Port = _smtpSettings.Port;
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.FromMail, _smtpSettings.FromPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
            }
        }
 
        public async Task SendRegistrationEmailAsync(string email, string fullName, string mailContent)
        {
            var subject = "Welcome to Our Application";
            StringBuilder bodyBuilder = new();
            bodyBuilder.Append($"Dear {fullName},<br/><br/>");
            bodyBuilder.Append("Welcome to Library!<br/><br/>");
            bodyBuilder.Append("Please login!");
            bodyBuilder.Append("<br/><br/>Thank you for joining us!<br/><br/>");
            bodyBuilder.Append(mailContent);

            await SendEmailAsync(email, subject, bodyBuilder.ToString());
        }


    }

    
}
