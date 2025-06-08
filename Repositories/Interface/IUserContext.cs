namespace FinanceManagement.Repositories.Interface
{
    public interface IUserContext
    {
        public Task<Utilisateur> Get() => null;

        public bool IsAuthenticated() => false;
    }
}
