namespace FinanceManagement.Repositories.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string password, string body);
        Task SendEmailTest(string toEmail, string result, string body);
        Task SendForgotPassEmailAsync(string toEmail, string resetCode);
        Task SendNewPasswordEmailAsync(string toEmail, string resetCode);
    }
}