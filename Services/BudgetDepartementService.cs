using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.Repositories.Interface;
using FinanceTool.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Services
{
    public class BudgetDepartementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationService _notificationService;

        public BudgetDepartementService(IUnitOfWork unitOfWork, NotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<BudgetDepartementDto>> GetAllBudgetsDepartementsAsync()
        {
            var budgets = await _unitOfWork.BudgetsDepartements.GetAllAsync();
            var result = new List<BudgetDepartementDto>();

            foreach (var budget in budgets)
            {
                var departement = await _unitOfWork.Departements.GetByIdAsync(budget.DepartementId);
                result.Add(MapToBudgetDepartementDto(budget, departement));
            }

            return result;
        }

        public async Task<BudgetDepartementDto> GetBudgetDepartementByIdAsync(int budgetDepartementId)
        {
            var budget = await _unitOfWork.BudgetsDepartements.GetByIdAsync(budgetDepartementId);
            if (budget == null)
                return null;

            var departement = await _unitOfWork.Departements.GetByIdAsync(budget.DepartementId);
            return MapToBudgetDepartementDto(budget, departement);
        }

        public async Task<BudgetDepartementDto> GetCurrentBudgetForDepartementAsync(int departementId, int year)
        {
            var budget = await _unitOfWork.BudgetsDepartements.GetByDepartementIdAndYearAsync(departementId, year);
            if (budget == null)
                return null;

            var departement = await _unitOfWork.Departements.GetByIdAsync(budget.DepartementId);
            return MapToBudgetDepartementDto(budget, departement);
        }

        public async Task<BudgetDepartementDto> CreateBudgetDepartementAsync(BudgetDepartement budgetDepartement)
        {
            var departement = await _unitOfWork.Departements.GetByIdAsync(budgetDepartement.DepartementId);
            if (departement == null)
                throw new ArgumentException("Département n'existe pas");

            var existingBudget = await _unitOfWork.BudgetsDepartements
                .FindAsync(b => b.DepartementId == budgetDepartement.DepartementId && b.Annee == budgetDepartement.Annee);
            if (existingBudget.Any())
                throw new InvalidOperationException("Un budget existe déjà pour ce département et cette année");

            budgetDepartement.DateCreation = DateTime.UtcNow;
            await _unitOfWork.BudgetsDepartements.AddAsync(budgetDepartement);
            await _unitOfWork.CompleteAsync();

            await _notificationService.SendBudgetDepartementUpdatedNotificationAsync(budgetDepartement.IdBudgetDepartement);
            return MapToBudgetDepartementDto(budgetDepartement, departement);
        }

        public async Task UpdateBudgetDepartementAsync(BudgetDepartement budgetDepartement)
        {
            var existingBudget = await _unitOfWork.BudgetsDepartements.GetByIdAsync(budgetDepartement.IdBudgetDepartement);
            if (existingBudget == null)
                throw new ArgumentException("Budget non trouvé");

            budgetDepartement.DateCreation = existingBudget.DateCreation;
            await _unitOfWork.BudgetsDepartements.UpdateAsync(budgetDepartement);
            await _unitOfWork.CompleteAsync();

            await _notificationService.SendBudgetDepartementUpdatedNotificationAsync(budgetDepartement.IdBudgetDepartement);
        }

        public async Task DeleteBudgetDepartementAsync(int budgetDepartementId)
        {
            var budget = await _unitOfWork.BudgetsDepartements.GetByIdAsync(budgetDepartementId);
            if (budget == null)
                throw new ArgumentException("Budget non trouvé");

            var projets = await _unitOfWork.Projets.FindAsync(p => p.DepartementId == budget.DepartementId);
            var budgetProjets = await _unitOfWork.BudgetsProjets.FindAsync(bp => projets.Any(p => p.IdProjet == bp.ProjetId));
            if (budgetProjets.Any())
                throw new InvalidOperationException("Impossible de supprimer un budget départemental avec des budgets de projet existants");

            await _unitOfWork.BudgetsDepartements.DeleteAsync(budget);
            await _unitOfWork.CompleteAsync();
        }

        private BudgetDepartementDto MapToBudgetDepartementDto(BudgetDepartement budget, Departement departement)
        {
            return new BudgetDepartementDto
            {
                IdBudgetDepartement = budget.IdBudgetDepartement,
                MontantAnnuel = budget.MontantAnnuel,
                Annee = budget.Annee,
                DepartementId = budget.DepartementId,
                DepartementNom = departement?.Name ?? "Inconnu"
            };
        }
    }
}