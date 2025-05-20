using FinanceManagement.Data.Models;

namespace FinanceManagement.Repositories.Interface
{
    public interface IBudgetProjetRepository : IGenericRepository<BudgetProjet>
    {
        Task<IEnumerable<BudgetProjet>> GetBudgetsByProjetAsync(int projetId);
        Task<double> GetCurrentSpendingForProjetAsync(int projetId);
        Task<bool> IsBudgetOverspentAsync(int budgetProjetId);
    }
}
