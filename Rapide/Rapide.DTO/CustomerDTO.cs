namespace Rapide.DTO
{
    public class CustomerDTO : BaseDTO
    {
        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public string? CustomerCode { get; set; }

        public string? MobileNumber { get; set; }

        public string? Email { get; set; }

        public string? HomeAddress { get; set; }

        public string? Notes { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyAddress { get; set; }

        public string? CompanyNo { get; set; }

        public bool IsActive { get; set; }

        public decimal LaborDiscountRate { get; set; }

        public decimal ProductDiscountRate { get; set; }

        public bool IsVATExempt { get; set; }

        public bool IsAllowWithholidingTax { get; set; }
    }
}