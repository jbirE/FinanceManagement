using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Data.Models
{


public partial class Facture
    {
        [Key]

        public int IdFacture { get; set; }
        [ForeignKey("RapportDepense")]
        public int RapportDepenseId { get; set; }
        public RapportDepense Rapport { get; set; }
        public double MontantFacture { get; set; }
        public string Fournisseur { get; set; }
       public string FileName { get; set; } //Attachement
        public string DocFilePath { get; set; }


        public DateTime DateFacture { get; set; }

    public string Statut { get; set; }

}
}