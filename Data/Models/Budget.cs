using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinanceManagement.Data.Models;

public class Budget
{
    [Key]
    public int IdBudget { get; set; }
    [Range(0.01, double.MaxValue)]
    public double MontantTotal { get; set; }
    [Range(0, double.MaxValue)]
    public double MontantDepense { get; set; } = 0;
    [ForeignKey("Departement")]
    public int DepartementId { get; set; }
    public Departement Departement { get; set; }

    [ForeignKey("Projet")]
    public int? ProjetId { get; set; }
    [JsonIgnore]
public Projet Projet { get; set; }
public DateTime DateCreation { get; set; } = DateTime.Now;
public DateTime DateFinProjet { get; set; } = DateTime.Now;

    public ICollection<RapportDepense> Rapports { get; set; }


}

