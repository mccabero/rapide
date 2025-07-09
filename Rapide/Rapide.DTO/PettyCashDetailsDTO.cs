namespace Rapide.DTO
{
    public class PettyCashDetailsDTO
    {
        public int Id { get; set; }

        public PettyCashDTO PettyCash { get; set; }
        public int PettyCashId { get; set; }

        public string Particulars { get; set; }

        public decimal Amount { get; set; }
    }
}