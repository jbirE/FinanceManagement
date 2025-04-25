namespace FinanceManagement.Data.Dtos
{
    public class EmployeeDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Addresse { get; set; }
        public string Cin { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public DateTime dateEmbauche { get; set; }
        public int IdDepartement { get; set; }
        public DateTime DerniereConnexion { get; set; } 
        public bool Status { get; set; }
    }
}
