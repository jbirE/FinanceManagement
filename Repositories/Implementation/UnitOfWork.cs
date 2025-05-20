using System;
using System.Threading.Tasks;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using FinanceTool.Repositories.Interface;
using Microsoft.AspNetCore.Identity;

namespace FinanceManagement.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly UserManager<Utilisateur> _userManager;


        public UnitOfWork(DataContext context, UserManager<Utilisateur> userManager)
        {
            _context = context;
            Departements = new DepartementRepository(_context);
            BudgetsDepartements = new BudgetDepartementRepository(_context);
            Projets = new ProjetRepository(_context);
            BudgetsProjets = new BudgetProjetRepository(_context);
            RapportsDepenses = new RapportDepenseRepository(_context);
            Notifications = new NotificationRepository(_context);
            Utilisateurs = new UtilisateurRepository(_context, _userManager);
            _userManager = userManager;
        }

        public IDepartementRepository Departements { get; }
        public IBudgetDepartementRepository BudgetsDepartements { get; }
        public IProjetRepository Projets { get; }
        public IBudgetProjetRepository BudgetsProjets { get; }
        public IRapportDepenseRepository RapportsDepenses { get; }
        public INotificationRepository Notifications { get; }
        public IUtilisateurRepository Utilisateurs { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}