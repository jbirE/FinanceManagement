namespace FinanceManagement.Data.Dtos
{
    public class EtatBudgetResponse
    {
        public int Annee { get; set; }
        public string NomDepartement { get; set; }
        public double BudgetDepartementAnnuel { get; set; }
        public double TotalMontantAlloue { get; set; }
        public double TotalDepenses { get; set; }
        public double SoldeDisponible { get; set; }
        public double PourcentageUtilisation { get; set; }
    }
}
