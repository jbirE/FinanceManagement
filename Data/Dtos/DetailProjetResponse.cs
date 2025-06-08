namespace FinanceManagement.Data.Dtos
{
    public class DetailProjetResponse
    {
        public string NomProjet { get; set; }
        public double PourcentageCompletion { get; set; }
        public double MontantAlloue { get; set; }
        public double TotalDepenses { get; set; }
        public double SoldeRestant { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string StatutBudget { get; set; }
    }
}
