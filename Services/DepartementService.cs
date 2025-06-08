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
        private readonly NotificationService _notificationService;

        public DepartementService(IUnitOfWork unitOfWork, NotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
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
            Console.WriteLine($"[DepartementService] Starting UpdateDepartementAsync for DepartementId: {id}");

            // Validate input
            if (departementDto == null)
            {
                Console.WriteLine("[DepartementService] Error: UpdateDepartementDTO is null");
                throw new ArgumentNullException(nameof(departementDto));
            }

            // Retrieve the department
            Console.WriteLine($"[DepartementService] Fetching department with ID: {id}");
            var departement = await _unitOfWork.Departements.GetByIdAsync(id);
            if (departement == null)
            {
                Console.WriteLine($"[DepartementService] Error: Département with ID {id} not found");
                throw new KeyNotFoundException($"Département avec ID {id} non trouvé.");
            }
            Console.WriteLine($"[DepartementService] Department found: {departement.Name}, Region: {departement.Region}");

            // Check for duplicate name
            Console.WriteLine($"[DepartementService] Checking if name '{departementDto.Name}' exists");
            if (departementDto.Name != departement.Name &&
                await _unitOfWork.Departements.NameExistsAsync(departementDto.Name))
            {
                Console.WriteLine($"[DepartementService] Error: Department name '{departementDto.Name}' already exists");
                throw new InvalidOperationException($"Un département avec le nom '{departementDto.Name}' existe déjà.");
            }

            // Update department properties
            Console.WriteLine($"[DepartementService] Updating department: Name={departementDto.Name}, Region={departementDto.Region}");
            departement.Name = departementDto.Name;
            departement.Region = departementDto.Region;

            // Save changes
            try
            {
                Console.WriteLine("[DepartementService] Calling UpdateAsync");
                await _unitOfWork.Departements.UpdateAsync(departement);
                Console.WriteLine("[DepartementService] Calling SaveChangesAsync");
                await _unitOfWork.SaveChangesAsync();
                Console.WriteLine("[DepartementService] Department updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DepartementService] Error updating department: {ex.Message}");
                throw;
            }

            // Send notification
            try
            {
                Console.WriteLine($"[DepartementService] Sending notification for DepartementId: {id}");
                await _notificationService.SendDepartementUpdatedNotificationAsync(id);
                Console.WriteLine("[DepartementService] Notification sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DepartementService] Error sending notification: {ex.Message}");
                // Log the error but don't throw to avoid interrupting the update
            }

            // Map to DTO
            Console.WriteLine("[DepartementService] Mapping department to DTO");
            var result = MapToDTO(departement);
            Console.WriteLine($"[DepartementService] UpdateDepartementAsync completed for DepartementId: {id}");

            return result;
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

        // Consolidated MapToDTO method
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
            var dto = MapToDTO(departement); // Use the consolidated MapToDTO

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