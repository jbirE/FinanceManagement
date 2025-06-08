namespace FinanceManagement.Data.Dtos
{
    public class DepartementDTO
    {
        public int IdDepartement { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public List<string> ProjetsNames { get; set; } = new List<string>();
        public int NombreUtilisateurs { get; set; }
        public double TotalBudget { get; set; }
    }
}