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

          

            // Seed Departements
            builder.Entity<Departement>().HasData(
                new Departement { IdDepartement = 1, Name = "TAX", BudgetTotal = 0 },
                new Departement { IdDepartement = 2, Name = "Assurance", BudgetTotal = 0 },
                new Departement { IdDepartement = 3, Name = "CBS", BudgetTotal = 0 },
                new Departement { IdDepartement = 4, Name = "Consulting", BudgetTotal = 0 },
                new Departement { IdDepartement = 5, Name = "Strategy & Transactions", BudgetTotal = 0 }
            );
        }

        public DbSet<Facture> Factures { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<RapportDepense> RapportDepenses { get; set; }
        public DbSet<Projet> Projets { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}