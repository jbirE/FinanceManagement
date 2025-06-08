namespace FinanceManagement.Data.Dtos
{
    public class DashboardResumeResponse
    {
        public EtatBudgetResponse EtatBudget { get; set; }
        public List<DetailProjetResponse> DetailsProjets { get; set; }
        public List<MoisDepenseResponse> DepensesMensuelles { get; set; }
        public KPITresorerieResponse KPITresorerie { get; set; }
        public DateTime DateGeneration { get; set; }
    }
}
