using System.Net.Mail;
using System.Net;
using FinanceManagement.Repositories.Interface;

namespace FinanceManagement.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string password, string body)
        {
            try
            {
                // SMTP configuration
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "racem123jbir@gmail.com";
                //string smtpPassword = "";

                using (var client = new SmtpClient(smtpHost))
                {
                    client.Port = smtpPort;
                    //here in the credential i add Password from google account for security reasons and I allow "Less secure  Account"
                    client.Credentials = new NetworkCredential(smtpUsername, "jbbzqajsnleabzuf");
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(new MailAddress(toEmail));
                    mailMessage.Subject = "your Login";
                    mailMessage.Body = body;

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"An error occurred while sending the email: {ex.Message}");
            }
        }
        //*****************test email*****************//

        public async Task SendEmailTest(string toEmail, string result, string body)
        {
            try
            {
                // SMTP configuration
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "emna.jbir33@gmail.com";
                //string smtpPassword = "";

                using (var client = new SmtpClient(smtpHost))
                {
                    client.Port = smtpPort;
                    //here in the credential i add Password from google account for security reasons and I allow "Less secure  Account"
                    client.Credentials = new NetworkCredential(smtpUsername, "qzuzjpfeuuryjlon");
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(new MailAddress(toEmail));
                    mailMessage.Subject = "WELCOME ";

                    // Customize the email body with the provided referent name and password


                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"An error occurred while sending the email: {ex.Message}");
            }
        }





        public async Task SendForgotPassEmailAsync(string toEmail, string resetCode)
        {
            try
            {
                // SMTP configuration
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "emna.jbir33@gmail.com";

                using (var client = new SmtpClient(smtpHost))
                {
                    client.Port = smtpPort;
                    //here in the credential i add Password from google account for security reasons and I allow "Less secure  "
                    client.Credentials = new NetworkCredential(smtpUsername, "qzuzjpfeuuryjlon");
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(new MailAddress(toEmail));
                    mailMessage.Subject = "Tatuus Password Reset";

                    // Customize the email body with the provided reset code
                    mailMessage.Body = $"You have requested a password reset for your Tatuus account.\r\n\r\n" +
                                       $"Your reset code is: {resetCode}\r\n\r\n" +
                                       $"Please use this code to reset your password.\r\n\r\n" +
                                       $"If you did not request this reset, please ignore this email.";

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while sending the email: {ex.Message}");
            }
        }
        public async Task SendNewPasswordEmailAsync(string toEmail, string newPassword)
        {
            try
            {
                // SMTP configuration
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "emna.jbir33@gmail.com";

                using (var client = new SmtpClient(smtpHost))
                {
                    client.Port = smtpPort;
                    client.Credentials = new NetworkCredential(smtpUsername, "ltwupikyzcqxject");
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(new MailAddress(toEmail));
                    mailMessage.Subject = "PlatformBank New Password";

                    // Customize the email body with the new password
                    mailMessage.Body = $"Your new password is: {newPassword}\r\n\r\n" +
                                       $"Please log in using your new password.\r\n\r\n" +
                                       $"If you did not request this reset, please contact our support." +
                                       $"[Reset Password Interface Link]";

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while sending the email: {ex.Message}");
            }
        }

    }
}
