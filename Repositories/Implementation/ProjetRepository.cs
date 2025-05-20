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
    }
}