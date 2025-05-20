using System;
using System.Threading.Tasks;
using FinanceManagement.Repositories.Interface;

namespace FinanceTool.Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IDepartementRepository Departements { get; }
        IBudgetDepartementRepository BudgetsDepartements { get; }
        IProjetRepository Projets { get; }
        IBudgetProjetRepository BudgetsProjets { get; }
        IRapportDepenseRepository RapportsDepenses { get; } 
        INotificationRepository Notifications { get; }
        IUtilisateurRepository Utilisateurs { get; }
        Task<int> CompleteAsync();
        Task<int> SaveChangesAsync();
    }
}