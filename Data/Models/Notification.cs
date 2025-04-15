using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Data.Models
{
public partial class Notification
    {
        [Key]

        public int IdNotif { get; set; }

    public string Message { get; set; }

    public DateTime DateEnvoi { get; set; }

        [ForeignKey("Utilisateur")]
        public string DestinataireId { get; set; }
        public Utilisateur Destinataire { get; set; }

    }
 }