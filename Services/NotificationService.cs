using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Enum;
using FinanceManagement.Data.Models;
using FinanceManagement.SignalRjobs;
using FinanceTool.Repositories.Interface;
using Microsoft.AspNetCore.Identity;

public class NotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<Utilisateur> _userManager;
    private readonly NotificationProvider _notificationProvider; // New dependency

    public NotificationService(
        IUnitOfWork unitOfWork,
        UserManager<Utilisateur> userManager,
        NotificationProvider notificationProvider) // Add this parameter
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _notificationProvider = notificationProvider;
    }

    // Helper method to centralize notification creation
    public async Task<Notification> CreerNotificationAsync(
        string titre,
        string message,
        string destinataireId,
        int entityId,
        TypeNotification typeNotification)
    {
        if (string.IsNullOrEmpty(destinataireId))
            throw new ArgumentException("L'ID de l'utilisateur ne peut pas être vide");

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

    // Update NotifierApprobateurs to use the helper
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

    // New method to mark a notification as read
    public async Task MarquerCommeVuAsync(int notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null)
            throw new ArgumentException($"Notification avec ID {notificationId} non trouvée");

        notification.IsReaded = true;
        await _unitOfWork.Notifications.UpdateAsync(notification);
        await _unitOfWork.CompleteAsync();

        // If user has other notifications, update their count
        if (!string.IsNullOrEmpty(notification.DestinataireId))
        {
            var count = await _unitOfWork.Notifications
                .CountAsync(n => n.DestinataireId == notification.DestinataireId && !n.IsReaded);

            await _notificationProvider.UpdateNotificationsCount(notification.DestinataireId, count);
        }
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


    public async Task SendDepartementUpdatedNotificationAsync(int departementId)
    {
        Console.WriteLine($"[NotificationService] Starting SendDepartementUpdatedNotificationAsync for DepartementId: {departementId}");

        // Retrieve the department
        var departement = await _unitOfWork.Departements.GetByIdAsync(departementId);
        if (departement == null)
        {
            Console.WriteLine($"[NotificationService] Error: Département with ID {departementId} not found");
            throw new ArgumentException("Département non trouvé");
        }
        Console.WriteLine($"[NotificationService] Department found: {departement.Name}, Region: {departement.Region}");

        // Get users in the department
        try
        {
            Console.WriteLine($"[NotificationService] Fetching users for DepartementId: {departementId}");
            var utilisateurs = await _unitOfWork.Utilisateurs.GetByDepartementIdAsync(departementId);
            Console.WriteLine($"[NotificationService] Found {utilisateurs.Count()} users in department {departementId}");

            // Find the department manager
            Utilisateur manager = null;
            foreach (var user in utilisateurs)
            {
                Console.WriteLine($"[NotificationService] Checking user {user.Id}");
                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"[NotificationService] User {user.Id} roles: {string.Join(", ", roles)}");
                if (roles.Contains("DepartementManager"))
                {
                    manager = user;
                    Console.WriteLine($"[NotificationService] Department manager found: {manager.Id}");
                    break;
                }
            }

            if (manager == null || string.IsNullOrEmpty(manager.Id))
            {
                Console.WriteLine("[NotificationService] Error: No department manager found or manager ID is empty");
                return;
            }

            // Create and send notification
            Console.WriteLine($"[NotificationService] Creating notification for manager {manager.Id}");
            await CreerNotificationAsync(
                "Département mis à jour",
                $"Le département '{departement.Name}' a été mis à jour. Région: {departement.Region}",
                manager.Id,
                departementId,
                TypeNotification.Departement);
            Console.WriteLine($"[NotificationService] Notification sent for DepartementId: {departementId} to manager {manager.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NotificationService] Error in SendDepartementUpdatedNotificationAsync: {ex.Message}");
            throw; // Rethrow to catch in UpdateDepartementAsync
        }
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

        // charger le departement liée au projet 
        var departementId = projet.DepartementId;

        // charger tt les financiers 
        var financierDepartment = await _unitOfWork.Utilisateurs.GetByDepartementIdAsync(departementId);

        // notifier les financiers du départements 
        foreach (var user in financierDepartment)
        {

            await CreerNotificationAsync(
           "Budget proche de la limite",
           $"Le budget du projet '{projet.Nom}' approche de sa limite. " +
           $"Dépenses actuelles: {spendingPercentage:N1}% du budget alloué ({budget.DepensesTotales:C} de {budget.MontantAlloue:C})",
           user.Id,
           budget.IdBudgetProjet,
           TypeNotification.BudgetProjet);
        }

        await CreerNotificationAsync(
            "Budget proche de la limite",
            $"Le budget du projet '{projet.Nom}' approche de sa limite. " +
            $"Dépenses actuelles: {spendingPercentage:N1}% du budget alloué ({budget.DepensesTotales:C} de {budget.MontantAlloue:C})",
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

    public async Task<NotificationPagedResult> GetNotificationsAsync(
    string userId,
    int page = 1,
    int pageSize = 10,
    bool nonLuesSeules = false)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("L'ID utilisateur est requis");

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        // Construction de la requête de base
        var query = await  _unitOfWork.Notifications
            .FindAsync(n => n.DestinataireId == userId);

        // Filtrer par statut de lecture si demandé
        if (nonLuesSeules)
        {
            query = query.Where(n => !n.IsReaded);
        }

        // Compter le total d'éléments
        var totalCount = query.Count();

        // Appliquer la pagination et trier par date de création (plus récent en premier)
        var notifications =  query
            .OrderByDescending(n => n.DateCreation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto
            {
                IdNotif = n.IdNotif,
                Message = n.Message,
                Titre = n.Titre,
                DateCreation = n.DateCreation,
                IsReaded = n.IsReaded,
                EntityId = n.EntityId,
                TypeEntityNotification = n.TypeEntityNotification,
                DestinataireId = n.DestinataireId,
                DestinataireNom = n.Destinataire != null ? n.Destinataire.Nom : null
            })
            .ToList();

        return new NotificationPagedResult
        {
            Notifications = notifications,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
    }

    public async Task<int> GetCountNotificationsNonLuesAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("L'ID utilisateur est requis");

        return await _unitOfWork.Notifications
            .CountAsync(n => n.DestinataireId == userId && !n.IsReaded);
    }

    public async Task<NotificationDto> GetNotificationByIdAsync(int notificationId)
    {
        var notification = await _unitOfWork.Notifications
            .GetByIdAsync(notificationId);
            

        if (notification == null)
            return null;

        return new NotificationDto
        {
            IdNotif = notification.IdNotif,
            Message = notification.Message,
            Titre = notification.Titre,
            DateCreation = notification.DateCreation,
            IsReaded = notification.IsReaded,
            EntityId = notification.EntityId,
            TypeEntityNotification = notification.TypeEntityNotification,
            DestinataireId = notification.DestinataireId,
            DestinataireNom = notification.Destinataire?.Nom
        };
    }
  
}
