using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace MyShop_Utility
{
    public class EmailSender : IEmailSender
    {
        public EmailSender()
        {
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string fromMail = "sikrierdanila@gmail.com";
            string fromPassword = "qxkpclfwlpavnpis";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body> " + htmlMessage + " </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
        }


    }
}
