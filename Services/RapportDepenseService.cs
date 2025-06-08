//using FinanceManagement.Data.Dtos;
//using FinanceManagement.Data.Models;
//using FinanceManagement.Repositories.Interface;
//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using FinanceTool.Repositories.Interface;
//using FinanceManagement.Data.Enum;

//namespace FinanceManagement.Services
//{
//    public class RapportDepenseService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly BudgetProjetService _budgetService;
//        private readonly NotificationService _notificationService;

//        public RapportDepenseService(
//            IUnitOfWork unitOfWork,
//            IMapper mapper,
//            BudgetProjetService budgetService,
//            NotificationService notificationService)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _budgetService = budgetService;
//            _notificationService = notificationService;
//        }

//        public async Task AcceptRapportDepense(int rapportDepenseId)
//        {
//            var rapport = await _unitOfWork.RapportsDepenses.GetByIdAsync(rapportDepenseId);
//            if (rapport == null)
//                throw new KeyNotFoundException($"Rapport de dépense avec ID {rapportDepenseId} non trouvé");

//            rapport.StatutApprobation = RapportDepense.StatutRapport.approuve;
//            await _unitOfWork.RapportsDepenses.UpdateAsync(rapport);
//            await _unitOfWork.SaveChangesAsync();

//            // Notification pour l'utilisateur
//            await _notificationService.CreerNotificationAsync(
//                  "Rapport de dépense approuvé",
//                  $"Votre rapport de dépense du {rapport.DateSoumission.ToShortDateString()} a été approuvé.",
//                  rapport.UtilisateurId,
//                  rapportDepenseId,
//                  TypeNotification.RapportDepenses);
           
//            // Mettre à jour le budget du projet
//            await _budgetService.AjusterBudgetApresApprobation(rapport.BudgetProjetId, rapport.Montant);
//        }

//        public async Task AddNewRapportDepense(RapportDepenseDTO rapportDepenseDTO)
//        {
//            var rapportDepense = _mapper.Map<RapportDepense>(rapportDepenseDTO);
//            rapportDepense.DateSoumission = DateTime.UtcNow; // Use UtcNow for consistency
//            rapportDepense.StatutApprobation = RapportDepense.StatutRapport.enAttente;

//            await _unitOfWork.RapportsDepenses.AddAsync(rapportDepense);
//            await _unitOfWork.SaveChangesAsync();

//            // Notification pour les approbateurs
//            await _notificationService.NotifierApprobateurs(
//                rapportDepense.IdRpport, // Added EntityId
//                "Nouveau rapport de dépense",
//                $"Un nouveau rapport de dépense a été soumis par {rapportDepense.Utilisateur?.Nom ?? "Utilisateur inconnu"} et nécessite votre approbation."
//            );
//        }

//        public async Task DeleteRapportDepense(int rapportDepenseId)
//        {
//            var entity = await _unitOfWork.RapportsDepenses.GetByIdAsync(rapportDepenseId);
//            if (entity == null)
//                throw new KeyNotFoundException($"Rapport de dépense avec ID {rapportDepenseId} non trouvé");

//            await _unitOfWork.RapportsDepenses.DeleteAsync(entity);
//            await _unitOfWork.SaveChangesAsync();
//        }

//        public async Task<IEnumerable<RapportDepenseDTO>> GetAllRapportDepenses()
//        {
//            var rapports = await _unitOfWork.RapportsDepenses.GetAllAsync();
//            return _mapper.Map<IEnumerable<RapportDepenseDTO>>(rapports);
//        }

//        public async Task<RapportDepenseDTO> GetRapportDepenseById(int id)
//        {
//            var rapport = await _unitOfWork.RapportsDepenses.GetByIdAsync(id);
//            if (rapport == null)
//                throw new KeyNotFoundException($"Rapport de dépense avec ID {id} non trouvé");

//            return _mapper.Map<RapportDepenseDTO>(rapport);
//        }

//        public async Task<IEnumerable<RapportDepenseDTO>> GetRapportDepensesByBudgetProjetId(int budgetProjetId)
//        {
//            var rapports = await _unitOfWork.RapportsDepenses.GetByBudgetProjetIdAsync(budgetProjetId);
//            return _mapper.Map<IEnumerable<RapportDepenseDTO>>(rapports);
//        }

//        public async Task<IEnumerable<RapportDepenseDTO>> GetRapportDepensesByStatut(RapportDepense.StatutRapport statut)
//        {
//            var rapports = await _unitOfWork.RapportsDepenses.GetByStatutAsync(statut);
//            return _mapper.Map<IEnumerable<RapportDepenseDTO>>(rapports);
//        }

//        public async Task<IEnumerable<RapportDepenseDTO>> GetRapportDepensesByUtilisateurId(string utilisateurId)
//        {
//            var rapports = await _unitOfWork.RapportsDepenses.GetByUtilisateurIdAsync(utilisateurId);
//            return _mapper.Map<IEnumerable<RapportDepenseDTO>>(rapports);
//        }

//        public async Task RejeterRapportDepense(int rapportDepenseId)
//        {
//            var rapport = await _unitOfWork.RapportsDepenses.GetByIdAsync(rapportDepenseId);
//            if (rapport == null)
//                throw new KeyNotFoundException($"Rapport de dépense avec ID {rapportDepenseId} non trouvé");

//            rapport.StatutApprobation = RapportDepense.StatutRapport.rejete;
//            await _unitOfWork.RapportsDepenses.UpdateAsync(rapport);
//            await _unitOfWork.SaveChangesAsync();

//            // Notification pour l'utilisateur
//            await _notificationService.CreerNotificationAsync(
//                 "Rapport de dépense rejeté",
//                $"Votre rapport de dépense du {rapport.DateSoumission.ToShortDateString()} a été rejeté.",
//                rapport.UtilisateurId,
//                rapportDepenseId,
//                TypeNotification.RapportDepenses);

          
//        }

//        public async Task UpdateRapportDepense(RapportDepenseDTO rapportChanges)
//        {
//            if (!await _unitOfWork.RapportsDepenses.ExistsAsync(rapportChanges.IdRpport)) // Fixed typo
//                throw new KeyNotFoundException($"Rapport de dépense avec ID {rapportChanges.IdRpport} non trouvé");

//            var existingRapport = await _unitOfWork.RapportsDepenses.GetByIdAsync(rapportChanges.IdRapport);

//            if (existingRapport.StatutApprobation != RapportDepense.StatutRapport.enAttente)
//                throw new InvalidOperationException("Impossible de modifier un rapport déjà approuvé ou rejeté");

//            _mapper.Map(rapportChanges, existingRapport);
//            existingRapport.StatutApprobation = RapportDepense.StatutRapport.enAttente;

//            await _unitOfWork.RapportsDepenses.UpdateAsync(existingRapport);
//            await _unitOfWork.SaveChangesAsync();
//        }
//    }
//}