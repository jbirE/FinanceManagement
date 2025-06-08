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

        // NOT CHANGEABLE: Constructor remains unchanged as it defines dependencies
        public BudgetProjetService(
            IUnitOfWork unitOfWork,
            NotificationService notificationService,
            BudgetDepartementService budgetDepartementService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _budgetDepartementService = budgetDepartementService;
        }

        // NOT CHANGEABLE: Returns BudgetProjetDto and is already aligned with your goal
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

        // NOT CHANGEABLE: Returns BudgetProjetDto and is already aligned with your goal
        public async Task<BudgetProjetDto> GetBudgetProjetByIdAsync(int budgetProjetId)
        {
            var budget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetId);
            if (budget == null)
                return null;

            var projet = await _unitOfWork.Projets.GetByIdAsync(budget.ProjetId);
            return await MapToBudgetProjetDtoAsync(budget, projet);
        }

        // NOT CHANGEABLE: Returns BudgetProjetDto and is already aligned with your goal
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

        // NOT CHANGEABLE: Internal logic, no DTO input/output changes needed
        public async Task<double> GetCurrentSpendingForProjetAsync(int projetId)
        {
            return await _unitOfWork.BudgetsProjets.GetCurrentSpendingForProjetAsync(projetId);
        }

        // NOT CHANGEABLE: Internal logic, no DTO input/output changes needed
        public async Task<bool> IsBudgetOverspentAsync(int budgetProjetId)
        {
            var isOverSpeending = await _unitOfWork.BudgetsProjets.IsBudgetOverspentAsync(budgetProjetId);

            // envoyer norification pour informer le manager 
            await _notificationService.SendBudgetApproachingLimitNotificationAsync(budgetProjetId, 80);

            return isOverSpeending;
        }

        // CHANGEABLE: Modified to accept BudgetProjetDto instead of BudgetProjet
        public async Task<BudgetProjetDto> CreateBudgetProjetAsync(BudgetProjetDto budgetProjetDto)
        {
            // Map DTO to model
            var budgetProjet = new BudgetProjet
            {
                IdBudgetProjet = budgetProjetDto.IdBudgetProjet, // May be 0 for new budgets
                MontantAlloue = budgetProjetDto.MontantAlloue,
                DepensesTotales = 0, // Initialize to 0 as per original logic
                ProjetId = budgetProjetDto.ProjetId,
                UtilisateurId = budgetProjetDto.UtilisateurId,
                DateCreation = DateTime.UtcNow // Set in service as per original logic
            };

            var projet = await _unitOfWork.Projets.GetByIdAsync(budgetProjet.ProjetId);
            if (projet == null)
                throw new ArgumentException("Projet n'existe pas");

            await _unitOfWork.BudgetsProjets.AddAsync(budgetProjet);
            await _unitOfWork.CompleteAsync();
            await _notificationService.SendBudgetCreatedNotificationAsync(budgetProjet.IdBudgetProjet);

            return await MapToBudgetProjetDtoAsync(budgetProjet, projet);
        }

        // CHANGEABLE: Modified to accept BudgetProjetDto instead of BudgetProjet
        public async Task UpdateBudgetProjetAsync(BudgetProjetDto budgetProjetDto)
        {
            var existingBudget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetDto.IdBudgetProjet);
            if (existingBudget == null)
                throw new ArgumentException("Budget non trouvé");

            // Update only the fields that should be modified from DTO
            existingBudget.MontantAlloue = budgetProjetDto.MontantAlloue;
            // Preserve original fields as per original logic
            existingBudget.ProjetId = existingBudget.ProjetId;
            existingBudget.UtilisateurId = existingBudget.UtilisateurId;
            existingBudget.DateCreation = existingBudget.DateCreation;
            existingBudget.DepensesTotales = existingBudget.DepensesTotales;

            await _unitOfWork.BudgetsProjets.UpdateAsync(existingBudget);
            await _unitOfWork.CompleteAsync();
        }

        // NOT CHANGEABLE: No DTO input, internal logic remains intact
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

        // NOT CHANGEABLE: No DTO input, complex business logic tied to models
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
                await _notificationService.SendBudgetApproachingLimitNotificationAsync(budgetProjetId, 80);
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

        // NOT CHANGEABLE: Mapping logic already returns BudgetProjetDto, no changes needed
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
                DateFinProjet = projet?.DateFin ?? DateTime.MinValue,
                ProjetId = budget.ProjetId,
                ProjetNom = projet?.Nom ?? "Inconnu",
                UtilisateurId = budget.UtilisateurId,
                UtilisateurNom = utilisateurNom
            };
        }
    }
}