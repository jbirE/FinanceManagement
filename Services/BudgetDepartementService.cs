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

        public async Task<BudgetDepartementDto> CreateBudgetDepartementAsync(BudgetDepartementDto budgetDepartement)
        {
            var departement = await _unitOfWork.Departements.GetByIdAsync(budgetDepartement.DepartementId);
            if (departement == null)
                throw new ArgumentException("Département n'existe pas");

            var existingBudget = await _unitOfWork.BudgetsDepartements
                .FindAsync(b => b.DepartementId == budgetDepartement.DepartementId && b.Annee == budgetDepartement.Annee);
            if (existingBudget.Any())
                throw new InvalidOperationException("Un budget existe déjà pour ce département et cette année");

            var newBudgetDepartement = new BudgetDepartement
            {
                DateCreation = DateTime.Now,
                DepartementId = budgetDepartement.DepartementId,
                Annee = budgetDepartement.Annee,
                MontantAnnuel = budgetDepartement.MontantAnnuel
            };

            await _unitOfWork.BudgetsDepartements.AddAsync(newBudgetDepartement);
            await _unitOfWork.CompleteAsync();

            await _notificationService.SendBudgetDepartementUpdatedNotificationAsync(newBudgetDepartement.IdBudgetDepartement);
            return MapToBudgetDepartementDto(newBudgetDepartement, departement);
        }

        public async Task UpdateBudgetDepartementAsync(BudgetDepartementDto budgetDepartement)
        {
            var existingBudget = await _unitOfWork.BudgetsDepartements.GetByIdAsync(budgetDepartement.IdBudgetDepartement);
            if (existingBudget == null)
                throw new ArgumentException("Budget non trouvé");

            var changesBudgetDepartement = new BudgetDepartement
            {
                DateCreation = DateTime.Now,
                DepartementId = budgetDepartement.DepartementId,
                Annee = budgetDepartement.Annee,
                MontantAnnuel = budgetDepartement.MontantAnnuel
            };

            await _unitOfWork.BudgetsDepartements.UpdateAsync(changesBudgetDepartement);
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