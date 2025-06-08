using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Implementation;
using FinanceManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories
{
    public class BudgetDepartementRepository : GenericRepository<BudgetDepartement>, IBudgetDepartementRepository
    {
        public BudgetDepartementRepository(DataContext context) : base(context) { }

        public async Task<BudgetDepartement> GetByDepartementIdAndYearAsync(int departementId, int year)
        {
            return await _context.BudgetsDepartements
                .FirstOrDefaultAsync(b => b.DepartementId == departementId && b.Annee == year);
        }

        // Override methods where interface signature differs from generic repository
        public async Task<BudgetDepartement> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BudgetDepartement>> FindAsync(Expression<Func<BudgetDepartement, bool>> predicate)
        {
            return await base.FindAsync(predicate);
        }

        public async Task<BudgetDepartement> GetCurrentBudgetForDepartementAsync(int departementId, int year)
        {
            return await GetByDepartementIdAndYearAsync(departementId, year);
        }
    }
}