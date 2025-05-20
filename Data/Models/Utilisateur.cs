using FinanceManagement.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class Utilisateur : IdentityUser
{
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Addresse { get; set; }
    public string Cin { get; set; }
    public DateTime DateEmbauche { get; set; }
    public DateTime DerniereConnexion { get; set; }
    public bool Status { get; set; }
    public bool PasswordChanged { get; set; } = false;
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiry { get; set; }

    [ForeignKey("Departement")]
    public int IdDepartement { get; set; }
    public Departement Departement { get; set; }

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<RapportDepense> Rapports { get; set; } = new List<RapportDepense>();
    public ICollection<BudgetProjet> BudgetsProjets { get; set; } = new List<BudgetProjet>();
}