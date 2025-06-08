using FinanceManagement.Data.Models;
using System.Linq.Expressions;

namespace FinanceManagement.Repositories.Interface
{
    public interface IBudgetDepartementRepository  
        {
        Task<IEnumerable<BudgetDepartement>> GetAllAsync();
        Task<BudgetDepartement> GetByIdAsync(int id);
        Task<BudgetDepartement> GetByDepartementIdAndYearAsync(int departementId, int year);
        Task<IEnumerable<BudgetDepartement>> FindAsync(Expression<Func<BudgetDepartement, bool>> predicate);
        Task AddAsync(BudgetDepartement budget);
        Task UpdateAsync(BudgetDepartement budget);
        Task DeleteAsync(BudgetDepartement budget);
    }

}
