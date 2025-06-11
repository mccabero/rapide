namespace Rapide.DTO
{
    public class PackageProductDTO : BaseDTO
    {
        public PackageDTO Package { get; set; }
        public int PackageId { get; set; }

        public ProductDTO Product { get; set; }
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Amount { get; set; }
    }
}