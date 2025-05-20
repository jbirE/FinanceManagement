using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.Repositories.Interface;
using FinanceTool.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Services
{
    public class BudgetProjetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationService _notificationService;
        private readonly BudgetDepartementService _budgetDepartementService;

        public BudgetProjetService(
            IUnitOfWork unitOfWork,
            NotificationService notificationService,
            BudgetDepartementService budgetDepartementService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _budgetDepartementService = budgetDepartementService;
        }

        public async Task<IEnumerable<BudgetProjetDto>> GetAllBudgetsProjetsAsync()
        {
            var budgets = await _unitOfWork.BudgetsProjets.GetAllAsync();
            var result = new List<BudgetProjetDto>();

            foreach (var budget in budgets)
            {
                var projet = await _unitOfWork.Projets.GetByIdAsync(budget.ProjetId);
                result.Add(await MapToBudgetProjetDtoAsync(budget, projet));
            }

            return result;
        }

        public async Task<BudgetProjetDto> GetBudgetProjetByIdAsync(int budgetProjetId)
        {
            var budget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetId);
            if (budget == null)
                return null;

            var projet = await _unitOfWork.Projets.GetByIdAsync(budget.ProjetId);
            return await MapToBudgetProjetDtoAsync(budget, projet);
        }

        public async Task<IEnumerable<BudgetProjetDto>> GetBudgetsByProjetAsync(int projetId)
        {
            var budgets = await _unitOfWork.BudgetsProjets.GetBudgetsByProjetAsync(projetId);
            var result = new List<BudgetProjetDto>();
            var projet = await _unitOfWork.Projets.GetByIdAsync(projetId);

            foreach (var budget in budgets)
            {
                result.Add(await MapToBudgetProjetDtoAsync(budget, projet));
            }

            return result;
        }

        public async Task<double> GetCurrentSpendingForProjetAsync(int projetId)
        {
            return await _unitOfWork.BudgetsProjets.GetCurrentSpendingForProjetAsync(projetId);
        }

        public async Task<bool> IsBudgetOverspentAsync(int budgetProjetId)
        {
            return await _unitOfWork.BudgetsProjets.IsBudgetOverspentAsync(budgetProjetId);
        }

        public async Task<BudgetProjetDto> CreateBudgetProjetAsync(BudgetProjet budgetProjet)
        {
            var projet = await _unitOfWork.Projets.GetByIdAsync(budgetProjet.ProjetId);
            if (projet == null)
                throw new ArgumentException("Projet n'existe pas");

            budgetProjet.DepensesTotales = 0;
            budgetProjet.DateCreation = DateTime.UtcNow;

            await _unitOfWork.BudgetsProjets.AddAsync(budgetProjet);
            await _unitOfWork.CompleteAsync();
            await _notificationService.SendBudgetCreatedNotificationAsync(budgetProjet.IdBudgetProjet);

            return await MapToBudgetProjetDtoAsync(budgetProjet, projet);
        }

        public async Task UpdateBudgetProjetAsync(BudgetProjet budgetProjet)
        {
            var existingBudget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjet.IdBudgetProjet);
            if (existingBudget == null)
                throw new ArgumentException("Budget non trouvé");

            budgetProjet.ProjetId = existingBudget.ProjetId;
            budgetProjet.UtilisateurId = existingBudget.UtilisateurId;
            budgetProjet.DateCreation = existingBudget.DateCreation;

            await _unitOfWork.BudgetsProjets.UpdateAsync(budgetProjet);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteBudgetProjetAsync(int budgetProjetId)
        {
            var budget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetId);
            if (budget == null)
                throw new ArgumentException("Budget non trouvé");

            var rapports = await _unitOfWork.RapportsDepenses.GetByBudgetProjetIdAsync(budgetProjetId);
            if (rapports.Any())
                throw new InvalidOperationException("Impossible de supprimer un budget avec des rapports de dépense existants");

            await _unitOfWork.BudgetsProjets.DeleteAsync(budget);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AjusterBudgetApresApprobation(int budgetProjetId, double montant)
        {
            if (montant <= 0)
                throw new ArgumentException("Le montant doit être positif.");

            var budgetProjet = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetId);
            if (budgetProjet == null)
                throw new KeyNotFoundException($"BudgetProjet avec ID {budgetProjetId} non trouvé");

            budgetProjet.DepensesTotales += montant;

            if (budgetProjet.DepensesTotales > budgetProjet.MontantAlloue)
            {
                await _notificationService.SendBudgetOverspentNotificationAsync(budgetProjetId);
                throw new InvalidOperationException($"Le budget du projet {budgetProjetId} est dépassé.");
            }

            await _notificationService.SendBudgetApproachingLimitNotificationAsync(budgetProjetId, 80);

            var projet = await _unitOfWork.Projets.GetByIdAsync(budgetProjet.ProjetId);
            if (projet == null)
                throw new KeyNotFoundException($"Projet avec ID {budgetProjet.ProjetId} non trouvé");

            var budgetDepartement = await _unitOfWork.BudgetsDepartements.GetByDepartementIdAndYearAsync(
                projet.DepartementId, DateTime.UtcNow.Year);
            if (budgetDepartement == null)
                throw new KeyNotFoundException($"BudgetDepartement pour l'année {DateTime.UtcNow.Year} non trouvé");

            budgetDepartement.MontantAnnuel -= montant;

            if (budgetDepartement.MontantAnnuel < 0)
                throw new InvalidOperationException($"Le budget du département {projet.DepartementId} est dépassé.");

            await _unitOfWork.BudgetsProjets.UpdateAsync(budgetProjet);
            await _unitOfWork.BudgetsDepartements.UpdateAsync(budgetDepartement);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendBudgetDepartementUpdatedNotificationAsync(budgetDepartement.IdBudgetDepartement);
        }

        private async Task<BudgetProjetDto> MapToBudgetProjetDtoAsync(BudgetProjet budget, Projet projet)
        {
            string utilisateurNom = budget.UtilisateurId;
            var utilisateur = await _unitOfWork.Utilisateurs.GetByIdAsync(budget.UtilisateurId);
            if (utilisateur != null)
                utilisateurNom = $"{utilisateur.Prenom} {utilisateur.Nom}";

            return new BudgetProjetDto
            {
                IdBudgetProjet = budget.IdBudgetProjet,
                MontantAlloue = budget.MontantAlloue,
                DepensesTotales = budget.DepensesTotales,
                DateCreation = budget.DateCreation,
                DateFinProjet = projet?.DateFin ?? DateTime.MinValue, // Use Projet.DateFin
                ProjetId = budget.ProjetId,
                ProjetNom = projet?.Nom ?? "Inconnu",
                UtilisateurId = budget.UtilisateurId,
                UtilisateurNom = utilisateurNom
            };
        }
    }
}