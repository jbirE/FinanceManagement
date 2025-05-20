using FinanceManagement.Data.Models;
using FinanceManagement.Repositories.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Services
{
    public class UtilisateurService
    {
        private readonly IUtilisateurRepository _utilisateurRepository;

        public UtilisateurService(IUtilisateurRepository utilisateurRepository)
        {
            _utilisateurRepository = utilisateurRepository;
        }

        public async Task<IEnumerable<Utilisateur>> GetAllAsync()
        {
            return await _utilisateurRepository.GetAllAsync();
        }

        public async Task<Utilisateur> GetByIdAsync(string id)
        {
            return await _utilisateurRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Utilisateur>> GetByDepartementIdAsync(int departementId)
        {
            return await _utilisateurRepository.GetByDepartementIdAsync(departementId);
        }

        public async Task<IEnumerable<Utilisateur>> GetUsersByRoleAsync(string roleName)
        {
            return await _utilisateurRepository.GetUsersByRoleAsync(roleName);
        }
    }
}
