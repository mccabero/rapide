namespace Rapide.DTO
{
    public class QuickSalesProductDTO : BaseDTO
    {
        public QuickSalesDTO QuickSales { get; set; }
        public int QuickSalesId { get; set; }

        public ProductDTO Product { get; set; }
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Amount { get; set; }
    }
}