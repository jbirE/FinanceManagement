namespace FinanceManagement.Data.Dtos
{
  
    public class ForceChangePasswordDto
    {
        public string Email { get; set; }
        public string TempToken { get; set; }
        public string NewPassword { get; set; }
    }
}
