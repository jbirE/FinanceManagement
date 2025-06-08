using FinanceManagement.Data.Dtos;
using FinanceManagement.Services;

namespace FinanceManagement.Repositories.Interface
{
    public interface IDashboardService
    {
        public  Task<List<DetailProjetResponse>> GetDetailsProjetsAsync(int annee);
        public Task<EtatBudgetResponse> GetEtatBudgetParAnneeAsync(int annee);
        public Task<List<MoisDepenseResponse>> GetDepensesParMoisAsync(int annee);
        public Task<KPITresorerieResponse> GetKPITresorerieAsync(int annee);



    }
}
