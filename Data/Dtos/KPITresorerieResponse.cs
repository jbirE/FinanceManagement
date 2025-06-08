namespace FinanceManagement.Data.Dtos
{
    public class KPITresorerieResponse
    {
        public int Annee { get; set; }
        public double TresorerieNette { get; set; }
        public double RatioLiquidite { get; set; }
        public double VariationAnnuelle { get; set; }
        public double BudgetTotal { get; set; }
        public double DepensesTotales { get; set; }
        public string StatutTresorerie { get; set; }
        public string RecommandationAlerte { get; set; }
    }
}
