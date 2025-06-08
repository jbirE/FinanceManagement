using FinanceManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories.Interface
{
    public interface IProjetRepository : IGenericRepository<Projet>
    {
        Task<Projet> GetByIdAsync(int projetId);
        Task<IEnumerable<Projet>> FindAsync(Func<Projet, bool> predicate);
        Task<Projet> GetProjetWithBudgetsAsync(int projetId);
        Task<IEnumerable<Projet>> GetProjetsByDepartementAsync(int departementId);

        // New methods
        Task<IEnumerable<Projet>> GetByDepartmentWithBudgetsForYearAsync(int departementId, int annee);
        Task<(double TotalAlloue, double TotalDepenses)> GetDepartmentProjectsFinancialsAsync(int departementId, int annee);
        Task<IEnumerable<Projet>> GetProjectsWithMonthlyExpensesAsync(int departementId, int annee);
    }
}