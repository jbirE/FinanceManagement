using FinanceManagement.Data.Models;
using FinanceManagement.Repositories.Interface;
using FinanceTool.Repositories.Interface;

namespace FinanceManagement.Services
{
    public class ProjetService 
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Projet>> GetAllProjetsAsync()
        {
            return await _unitOfWork.Projets.GetAllAsync();
        }

        public async Task<Projet> GetProjetByIdAsync(int projetId)
        {
            return await _unitOfWork.Projets.GetProjetWithBudgetsAsync(projetId);
        }

        public async Task<IEnumerable<Projet>> GetProjetsByDepartementAsync(int departementId)
        {
            return await _unitOfWork.Projets.GetProjetsByDepartementAsync(departementId);
        }

        public async Task<Projet> CreateProjetAsync(Projet projet)
        {
            // Verify department exists
            var departement = await _unitOfWork.Departements.GetByIdAsync(projet.DepartementId);
            if (departement == null)
                throw new ArgumentException("Department does not exist");

            projet.DateDebut = DateTime.Now;

            await _unitOfWork.Projets.AddAsync(projet);
            await _unitOfWork.CompleteAsync();

            return projet;
        }

        public async Task UpdateProjetAsync(Projet projet)
        {
            var existingProjet = await _unitOfWork.Projets.GetByIdAsync(projet.IdProjet);

            if (existingProjet == null)
                throw new ArgumentException("Project not found");

            // Keep creation date
            projet.DateDebut = existingProjet.DateDebut;

            await _unitOfWork.Projets.UpdateAsync(projet);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProjetAsync(int projetId)
        {
            var projet = await _unitOfWork.Projets.GetByIdAsync(projetId);

            if (projet == null)
                throw new ArgumentException("Project not found");

            // Check if project has budgets
            var budgets = await _unitOfWork.BudgetsProjets.GetBudgetsByProjetAsync(projetId);
            if (budgets.Any())
                throw new InvalidOperationException("Cannot delete project with existing budgets");

            await _unitOfWork.Projets.DeleteAsync(projet);
            await _unitOfWork.CompleteAsync();
        }
    }
}

