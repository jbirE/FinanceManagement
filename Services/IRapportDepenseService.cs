using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Services
{
    public interface IRapportDepenseService
    {
        Task<IEnumerable<RapportDepenseDTO>> GetAllRapportDepenses();
        Task<RapportDepenseDTO> GetRapportDepenseById(int id);
        Task<IEnumerable<RapportDepenseDTO>> GetRapportDepensesByBudgetProjetId(int budgetProjetId);
        Task<IEnumerable<RapportDepenseDTO>> GetRapportDepensesByStatut(RapportDepense.StatutRapport statut);
        Task<IEnumerable<RapportDepenseDTO>> GetRapportDepensesByUtilisateurId(string utilisateurId);
        Task AddNewRapportDepense(RapportDepenseDTO rapportDepenseDTO); // notification
        Task UpdateRapportDepense(RapportDepenseDTO rapportChanges); // si statut validé => MAJ total depenses du budget projet et notification
        Task DeleteRapportDepense(int rapportDepenseId); // il faut verifier le total depense dans le budget projet et le mettre à jour
        Task AcceptRapportDepense(int rapportDepenseId); // MAJ le total depenses du budget projet et autorisé juste pour l'admin
        Task RejeterRapportDepense(int rapportDepenseId); // autorisé juste pour l'admin
    }
}