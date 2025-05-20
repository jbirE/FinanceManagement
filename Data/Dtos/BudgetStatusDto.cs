namespace FinanceManagement.Data.Dtos
{
    public class BudgetStatusDto
    {
        public double Allocated { get; set; }
        public double Spent { get; set; }
        public double Remaining { get; set; }
        public double UtilizationPercentage { get; set; }
    }
}
