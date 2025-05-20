using FinanceManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories.Interface
{
    public interface IRapportDepenseRepository : IGenericRepository<RapportDepense>
    {
        Task<IEnumerable<RapportDepense>> GetByBudgetProjetIdAsync(int budgetProjetId);
        Task<IEnumerable<RapportDepense>> GetByStatutAsync(RapportDepense.StatutRapport statut);
        Task<IEnumerable<RapportDepense>> GetByUtilisateurIdAsync(string utilisateurId);
        Task<IEnumerable<RapportDepense>> GetRapportsByBudgetProjetAsync(int budgetProjetId);
    }
}