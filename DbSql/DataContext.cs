using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace FinanceManagement.DbSql;

public class DataContext : IdentityDbContext<Utilisateur>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Departement> Departements { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<BudgetDepartement> BudgetsDepartements { get; set; }
    public DbSet<BudgetProjet> BudgetsProjets { get; set; }
    public DbSet<Projet> Projets { get; set; }
    public DbSet<RapportDepense> RapportDepenses { get; set; }
    public DbSet<Facture> Factures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RoleConfiguration());

        // Configurer les relations

        // Utilisateur -> Notifications
        modelBuilder.Entity<Utilisateur>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.Destinataire)
            .HasForeignKey(n => n.DestinataireId)
            .OnDelete(DeleteBehavior.Cascade);

        // Utilisateur -> Rapports
        modelBuilder.Entity<Utilisateur>()
            .HasMany(u => u.Rapports)
            .WithOne(r => r.Utilisateur)
            .HasForeignKey(r => r.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);

        // Utilisateur -> BudgetsProjets
        modelBuilder.Entity<Utilisateur>()
            .HasMany(u => u.BudgetsProjets)
            .WithOne(bp => bp.Utilisateur)
            .HasForeignKey(bp => bp.UtilisateurId)
            .OnDelete(DeleteBehavior.NoAction);

        // BudgetProjet -> Rapports
        modelBuilder.Entity<BudgetProjet>()
            .HasMany(bp => bp.Rapports)
            .WithOne(r => r.BudgetProjet)
            .HasForeignKey(r => r.BudgetProjetId)
            .OnDelete(DeleteBehavior.Cascade);

        // RapportDepense -> Factures
        modelBuilder.Entity<RapportDepense>()
            .HasMany(r => r.Factures)
            .WithOne(f => f.Rapport)
            .HasForeignKey(f => f.RapportDepenseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Projet -> BudgetsProjets
        modelBuilder.Entity<Projet>()
            .HasMany(p => p.BudgetsProjets)
            .WithOne(bp => bp.Projet)
            .HasForeignKey(bp => bp.ProjetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Projet -> Responsable (Utilisateur)
        modelBuilder.Entity<Projet>()
            .HasOne(p => p.Responsable)
            .WithMany()
            .HasForeignKey(p => p.ResponsableId)
            .OnDelete(DeleteBehavior.SetNull);

        // Departement -> Projets
        modelBuilder.Entity<Departement>()
            .HasMany(d => d.Projets)
            .WithOne(p => p.Departement)
            .HasForeignKey(p => p.DepartementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Departement -> Utilisateurs
        modelBuilder.Entity<Departement>()
            .HasMany(d => d.Utilisateurs)
            .WithOne(u => u.Departement)
            .HasForeignKey(u => u.IdDepartement)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RapportDepense>()
            .Property(d => d.StatutApprobation)
            .HasConversion<string>();
    }
}