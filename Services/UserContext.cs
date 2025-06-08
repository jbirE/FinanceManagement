using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceManagement.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _context;
    private readonly Guid _id;
    private SemaphoreSlim _semaphore;

    public UserContext(
        DataContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _id = Guid.NewGuid();
        _semaphore = new SemaphoreSlim(1);
    }

    public async Task<Utilisateur> Get()
    {
        if (!IsAuthenticated())
            return null;

        try
        {
            _semaphore.Wait();
            var userIdentifier = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            // convertir l'utilisateur authentifier depuis le token vers User
            var user = await _context
                .Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == userIdentifier);

            return user ?? throw new Exception("global:exceptions.ChargerUtilisateur" );
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool IsAuthenticated()
        => _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) != null;
}