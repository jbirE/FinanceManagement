using FinanceManagement.Data.Models;

namespace FinanceManagement.Repositories.Interface
{
    public interface IBudgetDepartementRepository  
        {
        Task<IEnumerable<BudgetDepartement>> GetAllAsync();
        Task<BudgetDepartement> GetByIdAsync(int id);
        Task<BudgetDepartement> GetByDepartementIdAndYearAsync(int departementId, int year);
        Task<IEnumerable<BudgetDepartement>> FindAsync(Func<BudgetDepartement, bool> predicate);
        Task AddAsync(BudgetDepartement budget);
        Task UpdateAsync(BudgetDepartement budget);
        Task DeleteAsync(BudgetDepartement budget);
    }

}
