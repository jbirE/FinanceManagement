using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories.Implementation
{
    public class BudgetProjetRepository : GenericRepository<BudgetProjet>, IBudgetProjetRepository
    {
        public BudgetProjetRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<BudgetProjet>> GetBudgetsByProjetAsync(int projetId)
        {
            return await _context.BudgetsProjets
                .Where(b => b.ProjetId == projetId)
                .Include(b => b.Rapports)
                .ToListAsync();
        }

        public async Task<double> GetCurrentSpendingForProjetAsync(int projetId)
        {
            var budgets = await _context.BudgetsProjets
                .Where(b => b.ProjetId == projetId)
                .ToListAsync();

            return budgets.Sum(b => b.DepensesTotales);
        }

        public async Task<bool> IsBudgetOverspentAsync(int budgetProjetId)
        {
            var budget = await _context.BudgetsProjets
                .FirstOrDefaultAsync(b => b.IdBudgetProjet == budgetProjetId);

            if (budget == null)
                return false;

            return budget.DepensesTotales > budget.MontantAlloue;
        }
    }
}
