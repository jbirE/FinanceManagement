
using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Enum;
using FinanceManagement.Data.Models;
using FinanceManagement.Repositories.Interface;
using FinanceTool.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Services
{
    public class ProjetService
    {
        private readonly IUnitOfWork _unitOfWork;

        // NOT CHANGEABLE: Constructor remains unchanged
        public ProjetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // NOT CHANGEABLE: Logic unchanged, returns ProjetDto
        public async Task<IEnumerable<ProjetDto>> GetAllProjetsAsync()
        {
            var projets = await _unitOfWork.Projets.GetAllAsync();
            var result = new List<ProjetDto>();

            foreach (var projet in projets)
            {
                result.Add(await MapToProjetDtoAsync(projet));
            }

            return result;
        }

        // NOT CHANGEABLE: Logic unchanged, returns ProjetDto
        public async Task<ProjetDto> GetProjetByIdAsync(int projetId)
        {
            var projet = await _unitOfWork.Projets.GetProjetWithBudgetsAsync(projetId);
            if (projet == null)
                return null;

            return await MapToProjetDtoAsync(projet);
        }

        // NOT CHANGEABLE: Logic unchanged, returns ProjetDto
        public async Task<IEnumerable<ProjetDto>> GetProjetsByDepartementAsync(int departementId)
        {
            var projets = await _unitOfWork.Projets.GetProjetsByDepartementAsync(departementId);
            var result = new List<ProjetDto>();

            foreach (var projet in projets)
            {
                result.Add(await MapToProjetDtoAsync(projet));
            }

            return result;
        }

        // NOT CHANGEABLE: Logic unchanged from previous fix (IdProjet removed)
        public async Task<ProjetDto> CreateProjetAsync(ProjetDto projetDto)
        {
            var projet = new Projet
            {
                Nom = projetDto.Nom,
                DateDebut = DateTime.UtcNow,
                DateFin = projetDto.DateFin,
                DepartementId = projetDto.DepartementId,
                ResponsableId = projetDto.ResponsableId,
                Status = Enum.TryParse<ProjetStatus>(projetDto.Status, true, out var status) ? status : ProjetStatus.NotStarted
            };

            var departement = await _unitOfWork.Departements.GetByIdAsync(projet.DepartementId);
            if (departement == null)
                throw new ArgumentException("Department does not exist");

            await _unitOfWork.Projets.AddAsync(projet);
            await _unitOfWork.CompleteAsync();

            return await MapToProjetDtoAsync(projet);
        }

        // NOT CHANGEABLE: Logic unchanged
        public async Task UpdateProjetAsync(ProjetDto projetDto)
        {
            var existingProjet = await _unitOfWork.Projets.GetByIdAsync(projetDto.IdProjet);
            if (existingProjet == null)
                throw new ArgumentException("Project not found");

            existingProjet.Nom = projetDto.Nom;
            existingProjet.DateFin = projetDto.DateFin;
            existingProjet.DepartementId = projetDto.DepartementId;
            existingProjet.ResponsableId = projetDto.ResponsableId;
            existingProjet.Status = Enum.TryParse<ProjetStatus>(projetDto.Status, true, out var status) ? status : existingProjet.Status;
            existingProjet.DateDebut = existingProjet.DateDebut;

            await _unitOfWork.Projets.UpdateAsync(existingProjet);
            await _unitOfWork.CompleteAsync();
        }

        // NOT CHANGEABLE: No DTO input, logic unchanged
        public async Task DeleteProjetAsync(int projetId)
        {
            var projet = await _unitOfWork.Projets.GetByIdAsync(projetId);
            if (projet == null)
                throw new ArgumentException("Project not found");

            var budgets = await _unitOfWork.BudgetsProjets.GetBudgetsByProjetAsync(projetId);
            if (budgets.Any())
                throw new InvalidOperationException("Cannot delete project with existing budgets");

            await _unitOfWork.Projets.DeleteAsync(projet);
            await _unitOfWork.CompleteAsync();
        }

        // CHANGEABLE: Fixed typo in MontantAlloueTotal assignment
        private async Task<ProjetDto> MapToProjetDtoAsync(Projet projet)
        {
            var departement = await _unitOfWork.Departements.GetByIdAsync(projet.DepartementId);
            string departementNom = departement?.Name ?? "Inconnu";

            string responsableNom = projet.ResponsableId;
            if (!string.IsNullOrEmpty(projet.ResponsableId))
            {
                var utilisateur = await _unitOfWork.Utilisateurs.GetByIdAsync(projet.ResponsableId);
                responsableNom = utilisateur != null ? $"{utilisateur.Prenom} {utilisateur.Nom}" : "Inconnu";
            }

            var budgets = await _unitOfWork.BudgetsProjets.GetBudgetsByProjetAsync(projet.IdProjet);
            double montantAlloueTotal = budgets?.Sum(b => b.MontantAlloue) ?? 0;

            return new ProjetDto
            {
                IdProjet = projet.IdProjet,
                Nom = projet.Nom,
                DateDebut = projet.DateDebut,
                DateFin = projet.DateFin,
                DepartementId = projet.DepartementId,
                DepartementNom = departementNom,
                ResponsableId = projet.ResponsableId,
                ResponsableNom = responsableNom,
                MontantAlloueTotal = montantAlloueTotal, // Fixed typo: was montantAlloueTotallearn
                Status = projet.Status.ToString()
            };
        }
    }
}
