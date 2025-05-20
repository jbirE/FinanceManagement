namespace FinanceManagement.Data.Dtos
{
    public class BudgetProjetDto
    {
        public int IdBudgetProjet { get; set; }
        public double MontantAlloue { get; set; }
        public double DepensesTotales { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateFinProjet { get; set; }
        public int ProjetId { get; set; }
        public string ProjetNom { get; set; }
        public string UtilisateurId { get; set; }
        public string UtilisateurNom { get; set; }
    }
}
