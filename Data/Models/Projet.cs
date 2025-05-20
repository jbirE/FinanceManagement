using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Data.Models;

public class Projet
{
    [Key]
    public int IdProjet { get; set; }
    public string Nom { get; set; }
    public DateTime DateDebut { get; set; }

    public virtual ICollection<BudgetProjet> BudgetsProjets { get; set; } = new List<BudgetProjet>();

    public DateTime? DateFin { get; set; }

    [ForeignKey("Departement")]
    public int DepartementId { get; set; }
    public virtual Departement Departement { get; set; }

    [ForeignKey("Utilisateur")]
    public string? ResponsableId { get; set; }
    public Utilisateur Responsable { get; set; }
}