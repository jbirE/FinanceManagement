using FinanceManagement.Data.Models;

namespace FinanceManagement.Repositories.Interface;
 public interface IProjetRepository : IGenericRepository<Data.Models.Projet>
{
    Task<Data.Models.Projet> GetProjetWithBudgetsAsync(int projetId);
    Task<IEnumerable<Data.Models.Projet>> GetProjetsByDepartementAsync(int departementId);
    Task<Projet> GetByIdAsync(int id);
    Task<IEnumerable<Projet>> FindAsync(Func<Projet, bool> predicate);

}
