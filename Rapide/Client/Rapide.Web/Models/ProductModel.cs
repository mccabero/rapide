using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public string? PartNo { get; set; }

        public ProductGroupModel ProductGroup { get; set; }
        public int ProductGroupId { get; set; }

        public ProductCategoryModel ProductCategory { get; set; }
        public int ProductCategoryId { get; set; }

        public UnitOfMeasureModel UnitOfMeasure { get; set; }
        public int UnitOfMeasureId { get; set; }

        public ManufacturerModel Manufacturer { get; set; }
        public int ManufacturerId { get; set; }

        public SupplierModel Supplier { get; set; }
        public int SupplierId { get; set; }

        public DateTime? ExpirationDateTime { get; set; }

        public decimal PurchaseCost { get; set; }

        public decimal MarkupRate { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }

        public string? StorageLocation { get; set; }
    }
}
