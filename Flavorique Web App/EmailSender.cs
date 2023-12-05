using System.Net.Mail;
using System.Net;

namespace Flavorique_Web_App
{
    public class EmailSender : IEmailSender
    {

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "ENTER_EMAIL_HERE";
            var password = "ENTER_APP_PASSWORD_HERE";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject, 
                                message
                                ));
        }
    }
}
