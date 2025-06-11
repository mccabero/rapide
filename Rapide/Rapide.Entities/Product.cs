using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        public string? DisplayName { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? PartNo { get; set; }

        [Required]
        [ForeignKey("ProductGroupId")]
        public ProductGroup ProductGroup { get; set; }
        public int ProductGroupId { get; set; }

        [Required]
        [ForeignKey("ProductCategoryId")]
        public ProductCategory ProductCategory { get; set; }
        public int ProductCategoryId { get; set; }

        [Required]
        [ForeignKey("UnitOfMeasureId")]
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public int UnitOfMeasureId { get; set; }

        [Required]
        [ForeignKey("ManufacturerId")]
        public Manufacturer Manufacturer { get; set; }
        public int ManufacturerId { get; set; }

        [Required]
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }
        public int SupplierId { get; set; }

        public DateTime? ExpirationDateTime { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal PurchaseCost { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal MarkupRate { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal SellingPrice { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveSA { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveTech { get; set; }

        [StringLength(255)]
        public string? StorageLocation { get; set; }
    }
}