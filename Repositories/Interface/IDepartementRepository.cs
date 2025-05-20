using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.Repositories.Implementation;
using FinanceManagement.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace FinanceTool.Repositories.Interface;

public interface IDepartementRepository 
{
    Task<IEnumerable<Departement>> GetAllAsync();
    Task<Departement> GetByIdAsync(int id);
    Task<Departement> GetByIdWithDetailsAsync(int id);
    Task AddAsync(Departement departement);
    Task UpdateAsync(Departement departement);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name);
}