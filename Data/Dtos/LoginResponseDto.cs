namespace FinanceManagement.Data.Dtos
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Username { get; set; }
        public bool ChangePasswordRequired { get; set; }
        public string? TempToken { get; set; }
    }
}
