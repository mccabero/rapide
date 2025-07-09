using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class PettyCashDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("PettyCashId")]
        public PettyCash PettyCash { get; set; }
        public int PettyCashId { get; set; }

        [Required]
        public string Particulars { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Amount { get; set; }
    }
}