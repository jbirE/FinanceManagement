using FinanceManagement.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.DbSql
{
    public class DataContext : IdentityDbContext<Utilisateur>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfiguration());
            // Configuration des relations entre les entités (si nécessaire)
        }
        public DbSet<Facture>  Factures { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<RapportDepense> RapportDepenses { get; set; }
        public DbSet<Projet> Projets { get; set; }
        public DbSet<Notification> Notifications { get; set; }







    }
}
