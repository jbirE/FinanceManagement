using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Data.Models;

public class Departement
{
    [Key]
    public int IdDepartement { get; set; }
    public required string Name { get; set; }
    public string? Region { get; set; }


    public virtual ICollection<Projet> Projets { get; set; } = new List<Projet>();
    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
    public virtual ICollection<BudgetDepartement> BudgetsDepartements { get; set; } = new List<BudgetDepartement>();
}