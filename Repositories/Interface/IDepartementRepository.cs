using FinanceManagement.Data.Dtos;

namespace FinanceTool.Repositories.Interface
{
    public interface IDepartementRepository
    {
        Task<IEnumerable<DepartementDto>> GetAllDepartementsAsync();
        Task<DepartementDto> AddDepartementAsync(DepartementDto departementDto);
        Task<DepartementDto> UpdateDepartementAsync(int id, DepartementDto departementDto);
        Task<bool> DeleteDepartementAsync(int id);
    }
}
