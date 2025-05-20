using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Data.Models;

public class BudgetDepartement
{
    [Key]
    public int IdBudgetDepartement { get; set; }

    [Range(0.01, double.MaxValue)]
    public double MontantAnnuel { get; set; }

    public int Annee { get; set; }

    [ForeignKey("Departement")]
    public int DepartementId { get; set; }
    public Departement Departement { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;
}