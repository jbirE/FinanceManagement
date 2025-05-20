
using FinanceManagement.Data.Models;
using FinanceManagement.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using System.Linq;
using FinanceTool.Repositories.Interface;
using FinanceManagement.SignalRjobs;
using Microsoft.AspNetCore.SignalR;
using FinanceManagement.SignalRjobs.Hubs;

namespace FinanceManagement.Services
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly NotificationProvider _notificationProvider;

        public NotificationService(
            IUnitOfWork unitOfWork,
            UserManager<Utilisateur> userManager,
            NotificationProvider notificationProvider)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _notificationProvider = notificationProvider;
        }

        // Helper method to centralize notification creation
        private async Task<Notification> CreerNotificationAsync(
            string titre,
            string message,
            string destinataireId,
            int entityId,
            TypeNotification typeNotification)
        {
            var notification = new Notification
            {
                Titre = titre,
                Message = message,
                DateCreation = DateTime.UtcNow,
                IsReaded = false,
                DestinataireId = destinataireId,
                EntityId = entityId,
                TypeEntityNotification = typeNotification
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            // Send real-time notification via SignalR
            await _notificationProvider.SendNotificationAsync(notification);

            return notification;
        }

        public async Task EnvoyerNotification(int rapportDepenseId, string utilisateurId, string titre, string message)
        {
            if (string.IsNullOrEmpty(utilisateurId))
                throw new ArgumentException("L'ID de l'utilisateur ne peut pas être vide");

            await CreerNotificationAsync(
                titre,
                message,
                utilisateurId,
                rapportDepenseId,
                TypeNotification.RapportDepenses);
        }

        public async Task NotifierApprobateurs(int rapportDepenseId, string titre, string message)
        {
            var approbateurs = await _unitOfWork.Utilisateurs.GetUsersByRoleAsync("DepartementManager");

            foreach (var approbateur in approbateurs)
            {
                await CreerNotificationAsync(
                    titre,
                    message,
                    approbateur.Id,
                    rapportDepenseId,
                    TypeNotification.RapportDepenses);
            }
        }

        public async Task SendBudgetOverspentNotificationAsync(int budgetProjetId)
        {
            var budget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetId);
            if (budget == null)
                throw new ArgumentException("Budget not found");

            var projet = await _unitOfWork.Projets.GetByIdAsync(budget.ProjetId);
            if (projet == null || string.IsNullOrEmpty(projet.ResponsableId))
                return;

            await CreerNotificationAsync(
                "Budget dépassé",
                $"Le budget du projet '{projet.Nom}' a été dépassé. " +
                $"Budget alloué: {budget.MontantAlloue:C}, Dépenses: {budget.DepensesTotales:C}",
                projet.ResponsableId,
                budget.IdBudgetProjet,
                TypeNotification.BudgetProjet);
        }

        public async Task SendBudgetApproachingLimitNotificationAsync(int budgetProjetId, double percentageThreshold = 80)
        {
            var budget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetId);
            if (budget == null)
                throw new ArgumentException("Budget not found");

            var spendingPercentage = (budget.DepensesTotales / budget.MontantAlloue) * 100;
            if (spendingPercentage < percentageThreshold)
                return;

            var projet = await _unitOfWork.Projets.GetByIdAsync(budget.ProjetId);
            if (projet == null || string.IsNullOrEmpty(projet.ResponsableId))
                return;

            await CreerNotificationAsync(
                "Budget proche de la limite",
                $"Le budget du projet '{projet.Nom}' approche de sa limite. " +
                $"Dépenses actuelles: {spendingPercentage:N1}% du budget alloué ({budget.DepensesTotales:C} de {budget.MontantAlloue:C})",
                projet.ResponsableId,
                budget.IdBudgetProjet,
                TypeNotification.BudgetProjet);
        }

        public async Task SendBudgetCreatedNotificationAsync(int budgetProjetId)
        {
            var budget = await _unitOfWork.BudgetsProjets.GetByIdAsync(budgetProjetId);
            if (budget == null)
                throw new ArgumentException("Budget not found");

            var projet = await _unitOfWork.Projets.GetByIdAsync(budget.ProjetId);
            if (projet == null || string.IsNullOrEmpty(projet.ResponsableId))
                return;

            await CreerNotificationAsync(
                "Nouveau budget créé",
                $"Un nouveau budget de {budget.MontantAlloue:C} a été créé pour le projet '{projet.Nom}'",
                projet.ResponsableId,
                budget.IdBudgetProjet,
                TypeNotification.BudgetProjet);
        }

        public async Task SendBudgetDepartementUpdatedNotificationAsync(int budgetDepartementId)
        {
            var budget = await _unitOfWork.BudgetsDepartements.GetByIdAsync(budgetDepartementId);
            if (budget == null)
                throw new ArgumentException("Budget départemental non trouvé");

            var departement = await _unitOfWork.Departements.GetByIdAsync(budget.DepartementId);
            if (departement == null)
                throw new ArgumentException("Département non trouvé");

            var utilisateurs = await _unitOfWork.Utilisateurs.GetByDepartementIdAsync(budget.DepartementId);

            Utilisateur manager = null;
            foreach (var u in utilisateurs)
            {
                var roles = await _userManager.GetRolesAsync(u);
                if (roles.Contains("DepartementManager"))
                {
                    manager = u;
                    break;
                }
            }

            if (manager == null || string.IsNullOrEmpty(manager.Id))
                return;

            await CreerNotificationAsync(
                "Budget départemental mis à jour",
                $"Le budget du département '{departement.Name}' pour l'année {budget.Annee} a été mis à jour. " +
                $"Nouveau montant: {budget.MontantAnnuel:C}",
                manager.Id,
                budgetDepartementId,
                TypeNotification.BudgetDepartement);
        }

        // Method to mark a notification as read
        public async Task MarquerCommeVuAsync(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
                throw new ArgumentException($"Notification avec ID {notificationId} non trouvée");

            notification.IsReaded = true;

            // Use the Update method from your repository interface
            // Adapt this to match your actual repository interface
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.CompleteAsync();

            // If user has other notifications, update their count
            if (!string.IsNullOrEmpty(notification.DestinataireId))
            {
                var count = await _unitOfWork.Notifications
                    .CountAsync(n => n.DestinataireId == notification.DestinataireId && !n.IsReaded);

                await _notificationProvider._hubContext.Clients.User(notification.DestinataireId)
                    .ReceiveNotificationCount(count);
            }
        }
    }
}