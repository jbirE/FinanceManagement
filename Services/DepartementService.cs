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
    public class DepartementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DepartementDTO>> GetAllDepartementsAsync()
        {
            var departements = await _unitOfWork.Departements.GetAllAsync();
            return departements.Select(d => MapToDTO(d));
        }

        public async Task<DepartementDTO> GetDepartementByIdAsync(int id)
        {
            var departement = await _unitOfWork.Departements.GetByIdAsync(id);
            if (departement == null)
                return null;

            return MapToDTO(departement);
        }

        public async Task<DepartementDTO> GetDepartementWithDetailsAsync(int id)
        {
            var departement = await _unitOfWork.Departements.GetByIdWithDetailsAsync(id);
            if (departement == null)
                return null;

            return MapToDTOWithDetails(departement);
        }

        public async Task<DepartementDTO> CreateDepartementAsync(CreateDepartementDTO departementDto)
        {
            if (await _unitOfWork.Departements.NameExistsAsync(departementDto.Name))
                throw new InvalidOperationException($"Un département avec le nom '{departementDto.Name}' existe déjà.");

            var departement = new Departement
            {
                Name = departementDto.Name,
                Region = departementDto.Region
            };

            await _unitOfWork.Departements.AddAsync(departement);
            await _unitOfWork.SaveChangesAsync();

            return MapToDTO(departement);
        }

        public async Task<DepartementDTO> UpdateDepartementAsync(int id, UpdateDepartementDTO departementDto)
        {
            var departement = await _unitOfWork.Departements.GetByIdAsync(id);
            if (departement == null)
                throw new KeyNotFoundException($"Département avec ID {id} non trouvé.");

            if (departementDto.Name != departement.Name &&
                await _unitOfWork.Departements.NameExistsAsync(departementDto.Name))
                throw new InvalidOperationException($"Un département avec le nom '{departementDto.Name}' existe déjà.");

            departement.Name = departementDto.Name;
            departement.Region = departementDto.Region;

            await _unitOfWork.Departements.UpdateAsync(departement);
            await _unitOfWork.SaveChangesAsync();

            return MapToDTO(departement);
        }

        public async Task DeleteDepartementAsync(int id)
        {
            if (!await _unitOfWork.Departements.ExistsAsync(id))
                throw new KeyNotFoundException($"Département avec ID {id} non trouvé.");

            await _unitOfWork.Departements.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DepartementExistsAsync(int id)
        {
            return await _unitOfWork.Departements.ExistsAsync(id);
        }

        public async Task<bool> DepartementNameExistsAsync(string name)
        {
            return await _unitOfWork.Departements.NameExistsAsync(name);
        }

        private DepartementDTO MapToDTO(Departement departement)
        {
            return new DepartementDTO
            {
                IdDepartement = departement.IdDepartement,
                Name = departement.Name,
                Region = departement.Region
            };
        }

        private DepartementDTO MapToDTOWithDetails(Departement departement)
        {
            var dto = MapToDTO(departement);

            if (departement.Projets != null)
            {
                dto.ProjetsNames = departement.Projets.Select(p => p.Nom).ToList();
            }

            if (departement.Utilisateurs != null)
            {
                dto.NombreUtilisateurs = departement.Utilisateurs.Count;
            }

            if (departement.BudgetsDepartements != null)
            {
                dto.TotalBudget = departement.BudgetsDepartements.Sum(b => b.MontantAnnuel);
            }

            return dto;
        }
    }
}