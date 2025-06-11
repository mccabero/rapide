namespace Rapide.DTO
{
    public class ProductDTO : BaseDTO
    {
        public string? Name { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public string? PartNo { get; set; }

        public ProductGroupDTO ProductGroup { get; set; }
        public int ProductGroupId { get; set; }

        public ProductCategoryDTO ProductCategory { get; set; }
        public int ProductCategoryId { get; set; }

        public UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public int UnitOfMeasureId { get; set; }

        public ManufacturerDTO Manufacturer { get; set; }
        public int ManufacturerId { get; set; }

        public SupplierDTO Supplier { get; set; }
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