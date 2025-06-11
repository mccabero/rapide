namespace Rapide.Web.Models
{
    public class QuickSalesProductModel
    {
        public int Id { get; set; }

        public QuickSalesModel QuickSales { get; set; }
        public int QuickSalesId { get; set; }

        public ProductModel Product { get; set; }
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Amount { get; set; }
    }
}