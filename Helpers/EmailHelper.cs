using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace CrudMap.Helpers
{
    public class EmailHelper
    {
        public static void SendVerificationEmail(string toEmail, string link)
        {
            // ✅ Read credentials from Web.config
            var fromEmail = ConfigurationManager.AppSettings["EmailUsername"];
            var fromPassword = ConfigurationManager.AppSettings["EmailPassword"];

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail, "Bashat Parween");
            message.To.Add(new MailAddress(toEmail, "User"));
            message.Subject = "Reset Your Password - Bashat Parween App";
            message.Body = $"Click this link to reset your password: <a href='{link}'>Reset Password</a>";
            message.IsBodyHtml = true;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, fromPassword)
            };

            smtp.Send(message);
        }
    }
}
