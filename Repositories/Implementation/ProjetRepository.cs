using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories.Implementation
{
    public class ProjetRepository : GenericRepository<Projet>, IProjetRepository
    {
        public ProjetRepository(DataContext context) : base(context) { }

        // Existing methods...
        public async Task<Projet> GetByIdAsync(int projetId)
        {
            return await _context.Projets
                .FirstOrDefaultAsync(p => p.IdProjet == projetId);
        }

        public async Task<IEnumerable<Projet>> FindAsync(Func<Projet, bool> predicate)
        {
            return await Task.FromResult(_context.Projets
                .Where(predicate)
                .ToList());
        }

        public async Task<Projet> GetProjetWithBudgetsAsync(int projetId)
        {
            return await _context.Projets
                .Include(p => p.BudgetsProjets)
                .Include(p => p.Departement)
                .FirstOrDefaultAsync(p => p.IdProjet == projetId);
        }

        public async Task<IEnumerable<Projet>> GetProjetsByDepartementAsync(int departementId)
        {
            return await _context.Projets
                .Where(p => p.DepartementId == departementId)
                .ToListAsync();
        }

        // New methods for dashboard functionality

        /// <summary>
        /// Gets projects with their budgets for a specific department and year
        /// </summary>
        public async Task<IEnumerable<Projet>> GetByDepartmentWithBudgetsForYearAsync(int departementId, int annee)
        {
            return await _context.Projets
                .Where(p => p.DepartementId == departementId)
                .Include(p => p.BudgetsProjets
                    .Where(bp => bp.DateCreation.Year == annee))
                .Include(p => p.Departement)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the total allocated amount and expenses for projects in a department for a specific year
        /// </summary>
        public async Task<(double TotalAlloue, double TotalDepenses)> GetDepartmentProjectsFinancialsAsync(int departementId, int annee)
        {
            var projets = await _context.Projets
                .Where(p => p.DepartementId == departementId)
                .Include(p => p.BudgetsProjets
                    .Where(bp => bp.DateCreation.Year == annee))
                .ToListAsync();

            var totalAlloue = projets
                .SelectMany(p => p.BudgetsProjets)
                .Sum(bp => bp.MontantAlloue);

            var totalDepenses = projets
                .SelectMany(p => p.BudgetsProjets)
                .Sum(bp => bp.DepensesTotales);

            return (totalAlloue, totalDepenses);
        }

        /// <summary>
        /// Gets projects with their monthly expenses for a specific year
        /// </summary>
        public async Task<IEnumerable<Projet>> GetProjectsWithMonthlyExpensesAsync(int departementId, int annee)
        {
            return await _context.Projets
                .Where(p => p.DepartementId == departementId)
                .Include(p => p.BudgetsProjets
                    .Where(bp => bp.DateCreation.Year == annee))
                .ThenInclude(bp => bp.Rapports
                    .Where(rd => rd.DateSoumission.Year == annee))
                .ToListAsync();
        }
    }
}