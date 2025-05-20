namespace FinanceManagement.Data.Dtos
{
    public class FactureDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public DateTime Date { get; set; }
        public string Fournisseur { get; set; }
        public double Montant { get; set; }
        public string CheminFichier { get; set; }
    }
}
