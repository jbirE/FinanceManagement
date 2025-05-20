using FinanceManagement.Data.Models;

namespace FinanceManagement.Repositories.Interface
{
    public interface IUtilisateurRepository
    {
        Task<IEnumerable<Utilisateur>> GetAllAsync();
        Task<Utilisateur?> GetByIdAsync(string id);
        Task<IEnumerable<Utilisateur>> GetByDepartementIdAsync(int departementId);
        Task<IEnumerable<Utilisateur>> GetUsersByRoleAsync(string roleName);
        Task AddAsync(Utilisateur utilisateur);
        Task UpdateAsync(Utilisateur utilisateur);
        Task DeleteAsync(Utilisateur utilisateur);
    }
}
