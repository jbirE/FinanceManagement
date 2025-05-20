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
    public class RapportDepenseRepository : GenericRepository<RapportDepense>, IRapportDepenseRepository
    {
        private readonly DataContext _context;

        public RapportDepenseRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RapportDepense>> GetByBudgetProjetIdAsync(int budgetProjetId)
        {
            return await _context.RapportDepenses
                .Where(r => r.BudgetProjetId == budgetProjetId)
                .Include(r => r.BudgetProjet)
                .Include(r => r.Utilisateur)
                .Include(r => r.Factures)
                .ToListAsync();
        }

        public async Task<IEnumerable<RapportDepense>> GetByStatutAsync(RapportDepense.StatutRapport statut)
        {
            return await _context.RapportDepenses
                .Where(r => r.StatutApprobation == statut)
                .Include(r => r.BudgetProjet)
                .Include(r => r.Utilisateur)
                .Include(r => r.Factures)
                .ToListAsync();
        }

        public async Task<IEnumerable<RapportDepense>> GetByUtilisateurIdAsync(string utilisateurId)
        {
            return await _context.RapportDepenses
                .Where(r => r.UtilisateurId == utilisateurId)
                .Include(r => r.BudgetProjet)
                .Include(r => r.Utilisateur)
                .Include(r => r.Factures)
                .ToListAsync();
        }

        public async Task<IEnumerable<RapportDepense>> GetRapportsByBudgetProjetAsync(int budgetProjetId)
        {
            return await _context.RapportDepenses
                .Where(r => r.BudgetProjetId == budgetProjetId)
                .Include(r => r.BudgetProjet)
                .Include(r => r.Utilisateur)
                .Include(r => r.Factures)
                .ToListAsync();
        }

        // Override base methods to include navigation properties
        public override async Task<IEnumerable<RapportDepense>> GetAllAsync()
        {
            return await _context.RapportDepenses
                .Include(r => r.BudgetProjet)
                .Include(r => r.Utilisateur)
                .Include(r => r.Factures)
                .ToListAsync();
        }

       
    }
}