using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class PackageProductModel
    {
        public int Id { get; set; }

        public PackageModel Package { get; set; }
        public int PackageId { get; set; }

        public ProductModel Product { get; set; }
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Amount { get; set; }
    }
}
