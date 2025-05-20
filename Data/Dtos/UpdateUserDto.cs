namespace FinanceManagement.Data.Dtos
{
    public class UpdateUserDto
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Addresse { get; set; }
        public string Cin { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int IdDepartement { get; set; }
        public DateTime dateEmbauche { get; set; }
        public bool Status { get; set; }
        public string Role { get; set; }
    }
}
