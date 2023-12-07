using System.Net.Mail;
using System.Net;

namespace Flavorique_Web_App
{
    public class EmailSender : IEmailSender
    {

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "dionkelmendi2003@gmail.com";
            var password = "peaz drfq uakz svyk";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            var mailMessage = new MailMessage(from: mail,
                                to: email,
                                subject,
                                message
                                );

            mailMessage.IsBodyHtml = true;

            return client.SendMailAsync(mailMessage);
        }
    }
}
