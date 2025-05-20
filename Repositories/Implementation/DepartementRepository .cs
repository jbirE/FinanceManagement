using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceTool.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories.Implementation
{
    public class DepartementRepository : IDepartementRepository
    {
        private readonly DataContext _context;

        public DepartementRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Departement>> GetAllAsync()
        {
            return await _context.Departements.ToListAsync();
        }

        public async Task<Departement> GetByIdAsync(int id)
        {
            return await _context.Departements.FindAsync(id);
        }

        public async Task<Departement> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Departements
                .Include(d => d.Projets)
                .Include(d => d.Utilisateurs)
                .Include(d => d.BudgetsDepartements)
                .FirstOrDefaultAsync(d => d.IdDepartement == id);
        }

        public async Task AddAsync(Departement departement)
        {
            await _context.Departements.AddAsync(departement);
        }

        public async Task UpdateAsync(Departement departement)
        {
            _context.Departements.Update(departement);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var departement = await GetByIdAsync(id);
            if (departement != null)
            {
                _context.Departements.Remove(departement);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Departements.AnyAsync(d => d.IdDepartement == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Departements.AnyAsync(d => d.Name.ToLower() == name.ToLower());
        }
    }
}