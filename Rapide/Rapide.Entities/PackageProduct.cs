using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class PackageProduct : BaseEntity
    {
        [Required]
        [ForeignKey("PackageId")]
        public Package Package { get; set; }
        public int PackageId { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int ProductId { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Price { get; set; }

        public int Qty { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Amount { get; set; }
    }
}
