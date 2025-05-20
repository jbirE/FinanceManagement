using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Repositories.Implementation
{
    public class UtilisateurRepository : GenericRepository<Utilisateur>, IUtilisateurRepository
    {
        private readonly UserManager<Utilisateur> _userManager;

        public UtilisateurRepository(DataContext context, UserManager<Utilisateur> userManager)
            : base(context)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<Utilisateur>> GetByDepartementIdAsync(int departementId)
        {
            return await _userManager.Users
                .Where(u => u.IdDepartement == departementId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Utilisateur>> GetUsersByRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<Utilisateur> GetByIdAsync(string id)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
