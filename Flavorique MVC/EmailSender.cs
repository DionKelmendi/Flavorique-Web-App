using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Flavorique_MVC
{
	public class EmailSender : IEmailSender
	{

		public Task SendEmailAsync(string email, string subject, string message)
		{
			var mail = "flavoriqueblog@gmail.com";
			var password = "tdzb xvvk mkqe bvdl";

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
