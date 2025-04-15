namespace FinanceManagement.Data.Dtos
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Addresse { get; set; }
        public string Cin { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } // Role can be "Financier", "Employee", or "DepartmentManager"
    }
}
