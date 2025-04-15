using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceTool.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTool.Repositories.Implementation
{
    public class DepartementRepository : IDepartementRepository
    {
        private readonly DataContext _context;

        public DepartementRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartementDto>> GetAllDepartementsAsync()
        {
            return await _context.Departements
                .Select(d => new DepartementDto
                {
                    IdDepartement = d.IdDepartement,
                    Name = d.Name,
                    BudgetTotal = d.BudgetTotal
                })
                .ToListAsync();
        }

        public async Task<DepartementDto> AddDepartementAsync(DepartementDto departementDto)
        {
            var departement = new Departement
            {
                Name = departementDto.Name,
                BudgetTotal = departementDto.BudgetTotal
            };

            _context.Departements.Add(departement);
            await _context.SaveChangesAsync();

            return new DepartementDto
            {
                IdDepartement = departement.IdDepartement,
                Name = departement.Name,
                BudgetTotal = departement.BudgetTotal
            };
        }

        public async Task<DepartementDto> UpdateDepartementAsync(int id, DepartementDto departementDto)
        {
            var departement = await _context.Departements.FindAsync(id);
            if (departement == null) return null;

            departement.Name = departementDto.Name;
            departement.BudgetTotal = departementDto.BudgetTotal;

            _context.Departements.Update(departement);
            await _context.SaveChangesAsync();

            return new DepartementDto
            {
                IdDepartement = departement.IdDepartement,
                Name = departement.Name,
                BudgetTotal = departement.BudgetTotal
            };
        }

        public async Task<bool> DeleteDepartementAsync(int id)
        {
            var departement = await _context.Departements.FindAsync(id);
            if (departement == null) return false;

            _context.Departements.Remove(departement);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
