using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DataContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DataContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private static readonly Dictionary<Type, string> KeyNames = new()
        {
            { typeof(RapportDepense), "IdRapport" },
            { typeof(Projet), "IdProjet" },
            { typeof(BudgetProjet), "IdBudgetProjet" },
            { typeof(BudgetDepartement), "IdBudgetDepartement" },
            { typeof(Notification), "IdNotif" },
            { typeof(Facture), "IdFacture" },
            { typeof(Departement), "IdDepartement" },
            { typeof(Utilisateur), "Id" }
        };

        public virtual async Task<bool> ExistsAsync(object id)
        {
            var entityType = typeof(T);
            if (KeyNames.TryGetValue(entityType, out var keyName))
            {
                var parameter = Expression.Parameter(entityType, "e");
                var property = Expression.Property(parameter, keyName);
                var constant = Expression.Constant(id);
                var equal = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

                return await _dbSet.AnyAsync(lambda);
            }
            return await _dbSet.FindAsync(id) != null;
        }
    }
}