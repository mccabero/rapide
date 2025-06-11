namespace Rapide.DTO
{
    public class QuickSalesDTO : BaseDTO
    {
        public string? ReferenceNo { get; set; }

        public DateTime? TransactionDate { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public ParameterDTO PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public string? PaymentReferenceNo { get; set; }

        public UserDTO SalesPersonUser { get; set; }
        public int SalesPersonUserId { get; set; }

        public string? Summary { get; set; }

        public decimal SubTotal { get; set; }

        public decimal VAT12 { get; set; }

        public decimal Discount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Payment { get; set; }

        public decimal Change { get; set; }

        public virtual List<QuickSalesProductDTO> ProductList { get; set; }
    }
}