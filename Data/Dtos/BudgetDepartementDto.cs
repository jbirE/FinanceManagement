namespace FinanceManagement.Data.Dtos
{
    public class BudgetDepartementDto
    {
        public int IdBudgetDepartement { get; set; }
        public double MontantAnnuel { get; set; }
        public int Annee { get; set; }
        public int DepartementId { get; set; }
        public string DepartementNom { get; set; }
    }
}
