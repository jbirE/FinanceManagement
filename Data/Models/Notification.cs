using FinanceManagement.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Data.Models;

public class Notification
{
    [Key]
    public int IdNotif { get; set; }
    public string Message { get; set; }
    public DateTime DateCreation { get; set; }

    [ForeignKey("Utilisateur")]
    public string DestinataireId { get; set; }
    public Utilisateur Destinataire { get; set; }
    public string Titre { get; internal set; }
    public bool IsReaded { get; set; }

    public int EntityId { get; set; }

    public TypeNotification TypeEntityNotification {  get; set; }
}
